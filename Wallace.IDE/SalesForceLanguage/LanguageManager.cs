﻿/*
 * Copyright (c) 2014 Nathaniel Wallace
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.CodeModel;
using SalesForceLanguage.Apex.Parser;

namespace SalesForceLanguage
{
    /// <summary>
    /// Class used to manage language functions.
    /// </summary>
    public class LanguageManager
    {
        #region Fields

        /// <summary>
        /// Holds predefined classes.
        /// </summary>
        private static Dictionary<string, SymbolTable> _predefinedClasses;

        /// <summary>
        /// Holds all classes that have been parsed.
        /// </summary>
        private Dictionary<string, SymbolTable> _classes;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LanguageManager()
        {
            _predefinedClasses = new Dictionary<string, SymbolTable>();
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SalesForceLanguage.Resources.SystemSymbols.xml"))
            {
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.IgnoreComments = true;
                xmlSettings.IgnoreWhitespace = true;
                using (XmlReader reader = XmlReader.Create(stream, xmlSettings))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(SymbolTable[]));
                    SymbolTable[] symbols = ser.Deserialize(reader) as SymbolTable[];
                    foreach (SymbolTable symbol in symbols)
                        _predefinedClasses.Add(symbol.Id, symbol);
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LanguageManager(string symbolsFolder)
        {
            SymbolsFolder = symbolsFolder;
            _classes = new Dictionary<string, SymbolTable>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The folder where symbols are saved to if set.
        /// </summary>
        public string SymbolsFolder { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the symbols for the class with the given name.
        /// </summary>
        /// <param name="type">The name of the type to get symbols for.</param>
        /// <returns>The requested symbols or null if they aren't found.</returns>
        public SymbolTable GetSymbols(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            type = type.ToLower();

            if (_classes.ContainsKey(type))
                return _classes[type];
            if (_predefinedClasses.ContainsKey(type))
                return _predefinedClasses[type];

            if (!type.Contains('.'))
            {
                type = "system." + type;
                if (_classes.ContainsKey(type))
                    return _classes[type];
                if (_predefinedClasses.ContainsKey(type))
                    return _predefinedClasses[type];
            }

            return null;
        }

        /// <summary>
        /// Parse the text and give possible symbols that can be added to the end of the text.
        /// </summary>
        /// <param name="text">The text to get code completions for.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Valid symbols that can be used for the code completion.</returns>
        public Symbol[] GetCodeCompletions(string text, string className, TextPosition position)
        {
            List<Symbol> result = new List<Symbol>();

            // empty string
            if (String.IsNullOrWhiteSpace(text))
                return result.ToArray();

            text = text.ToLower();

            // filter lines
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == null)
                    continue;

                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("//") || lines[i].StartsWith("/*"))
                    lines[i] = null;
            }

            // create parts
            List<string> parts = new List<string>();
            foreach (string line in lines)
                if (line != null)
                    parts.AddRange(line.Split(new char[] { ' ', '.', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // combine and filter out parts
            for (int i = 0; i < parts.Count; i++)
            {
                if (i + 1 < parts.Count && (parts[i + 1] == "()" || parts[i + 1] == "[]"))
                {
                    parts[i] = parts[i] + parts[i + 1];
                    parts.RemoveAt(i + 1);
                }

                if (parts[i] == "if()")
                {
                    parts.RemoveAt(i);
                    i--;
                }
            }

            // match parts to types
            SymbolTable classSymbol = GetSymbols(className);
            TypedSymbol matchedSymbol = null;

            if (classSymbol != null)
            {
                bool partFound = false;

                for (int i = 0; i < parts.Count; i++)
                {
                    string part = parts[i];
                    partFound = false;

                    // method
                    if (parts[i].EndsWith("()"))
                    {
                        string methodName = part.Substring(0, part.IndexOf('('));
                        SymbolTable externalClass = (i == 0) ? classSymbol : GetSymbols(matchedSymbol.Type);
                        if (externalClass != null)
                        {
                            foreach (Method m in externalClass.Methods)
                            {
                                if (m.Id == methodName)
                                {
                                    if (m.Type != "void")
                                    {
                                        matchedSymbol = m;
                                        partFound = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    // list
                    else if (part.EndsWith("[]"))
                    {
                        //TODO:
                    }
                    // variables
                    else
                    {
                        SymbolTable externalClass = (i == 0) ? classSymbol : GetSymbols(matchedSymbol.Type);

                        if (i == 0)
                        {
                            // this keyword
                            if (part == "this")
                            {
                                matchedSymbol = classSymbol;
                                partFound = true;
                            }
                            else
                            {
                                // look for method parameters
                                foreach (Method method in classSymbol.Methods)
                                {
                                    if (method.Contains(position))
                                    {
                                        foreach (Parameter parameter in method.Parameters)
                                        {
                                            if (parameter.Id == part)
                                            {
                                                matchedSymbol = parameter;
                                                partFound = true;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                // look for local variable
                                if (!partFound)
                                {
                                    foreach (VariableScope scope in classSymbol.VariableScopes)
                                    {
                                        if (scope.Span.Contains(position))
                                        {
                                            foreach (Field variable in scope.Variables)
                                            {
                                                if (variable.Id == part && variable.Location.CompareTo(position) < 0)
                                                {
                                                    matchedSymbol = variable;
                                                    partFound = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (partFound)
                                            break;
                                    }
                                }
                            }
                        }

                        if (externalClass != null)
                        {
                            // look for fields
                            if (!partFound)
                            {
                                foreach (Field field in externalClass.Fields)
                                {
                                    if (field.Id == part)
                                    {
                                        matchedSymbol = field;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }

                            // look for properties
                            if (!partFound)
                            {
                                foreach (Property property in externalClass.Properties)
                                {
                                    if (property.Id == part)
                                    {
                                        matchedSymbol = property;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // check that the part was found
                    if (!partFound)
                    {
                        matchedSymbol = null;
                        break;
                    }
                }

                // build completions from matched symbol
                if (matchedSymbol != null)
                {
                    SymbolTable symbols = GetSymbols(matchedSymbol.Type);
                    if (symbols != null)
                    {
                        result.AddRange(symbols.Fields.GroupBy(s => s.Name).Select(g => g.First()).ToList());
                        result.AddRange(symbols.Constructors.GroupBy(s => s.Name).Select(g => g.First()).ToList());
                        result.AddRange(symbols.Properties.GroupBy(s => s.Name).Select(g => g.First()).ToList());
                        result.AddRange(symbols.Methods.GroupBy(s => s.Name).Select(g => g.First()).ToList());

                        //TODO: extends
                    }
                }
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Update the symbols in the manager.
        /// </summary>
        /// <param name="symbols">The symbols to update.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        public void UpdateSymbols(SymbolTable symbols, bool replace, bool save)
        {
            UpdateSymbols(new SymbolTable[] { symbols }, replace, save);
        }

        /// <summary>
        /// Update the symbols in the manager.
        /// </summary>
        /// <param name="symbols">The symbols to update.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        public void UpdateSymbols(IEnumerable<SymbolTable> symbols, bool replace, bool save)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");

            lock (_classes)
            {
                foreach (SymbolTable st in symbols)
                {
                    // update symbols in memory
                    if (replace)
                    {
                        _classes.Remove(st.Id);
                        _classes.Add(st.Id, st);
                    }
                    else
                    {
                        if (!_classes.ContainsKey(st.Id))
                            _classes.Add(st.Id, st);
                        else
                            save = false;
                    }

                    // save to local cache
                    if (save && !String.IsNullOrWhiteSpace(SymbolsFolder))
                    {
                        string fileName = Path.Combine(SymbolsFolder, st.Name.ToLower());
                        using (FileStream fs = File.Open(fileName, FileMode.Create))
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(SymbolTable));
                            ser.Serialize(fs, st);

                            fs.Flush();
                            fs.Close();
                        }
                    }

                    // process inner classes
                    foreach (SymbolTable innerClass in st.InnerClasses)
                    {
                        UpdateSymbols(innerClass, replace, save);
                    }
                }
            }
        }

        /// <summary>
        /// Remove the given symbols.
        /// </summary>
        /// <param name="name">The class name of the symbols to remove.</param>
        public void RemoveSymbols(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            lock (_classes)
            {
                _classes.Remove(name.ToLower());

                string fileName = Path.Combine(SymbolsFolder, name.ToLower());
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        /// <summary>
        /// Parse the given text for an apex document.
        /// </summary>
        /// <param name="stream">The stream to read text from to parse.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(Stream stream, bool replace, bool save)
        {
            if (stream == null)
                return new ParseResult(null, null, new LanguageError[0]);

            ApexParser parser = new ApexParser(new ApexLexer(stream));

            ParseResult result = parser.ParseApex();

            if (result.Symbols != null)
                UpdateSymbols(new SymbolTable[] { result.Symbols }, replace, save);

            return result;
        }

        /// <summary>
        /// Parse the given text for an apex document.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(string text, bool replace, bool save)
        {
            if (String.IsNullOrWhiteSpace(text))
                return new ParseResult(null, null, new LanguageError[0]);

            using (MemoryStream reader = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                return ParseApex(reader, replace, save);
            }
        }

        #endregion
    }
}
