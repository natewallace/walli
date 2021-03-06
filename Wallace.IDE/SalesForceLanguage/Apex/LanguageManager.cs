﻿/*
 * Copyright (c) 2015 Nathaniel Wallace
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
using SalesForceLanguage;
using SalesForceLanguage.Apex.CodeModel;
using SalesForceLanguage.Apex.Parser;

namespace SalesForceLanguage.Apex
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

        /// <summary>
        /// Holds all triggers that have been parsed.
        /// </summary>
        private Dictionary<string, SymbolTable> _triggers;

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
                    {
                        _predefinedClasses.Add(symbol.Id, symbol);
                    }
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
            _triggers = new Dictionary<string, SymbolTable>();
            Completion = new LanguageCompletion(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The folder where symbols are saved to if set.
        /// </summary>
        public string SymbolsFolder { get; private set; }

        /// <summary>
        /// Code completion functions for this language manager.
        /// </summary>
        public LanguageCompletion Completion { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all of the symbols.
        /// </summary>
        /// <returns>All symbols</returns>
        public SymbolTable[] GetAllSymbols()
        {
            List<SymbolTable> result = new List<SymbolTable>();
            result.AddRange(_classes.Values);
            result.AddRange(_predefinedClasses.Values);
            return result.Where(s => s.Name != "WalliSnippetPlaceHolder52724597325").ToArray();
        }

        /// <summary>
        /// Look for the given symbol.
        /// </summary>
        /// <param name="type">The symbol to look for.</param>
        /// <returns>The symbols if found, null if not found.</returns>
        private SymbolTable LookupSymbol(string type)
        {
            SymbolTable result = null;

            // normalize name
            TypedSymbol typedSymbol = new TypedSymbol(new TextPosition(0, 0), null, null, SymbolModifier.None, type);
            type = typedSymbol.Type.ToLower();

            // do simple lookup
            if (_classes.ContainsKey(type))
            {
                result = _classes[type];
            }
            else if (_predefinedClasses.ContainsKey(type))
            {
                result = _predefinedClasses[type];
            }
            // check for system types
            else if (!type.Contains('.'))
            {
                type = "system." + type;
                if (_classes.ContainsKey(type))
                    result = _classes[type];
                else if (_predefinedClasses.ContainsKey(type))
                    result = _predefinedClasses[type];
            }

            if (result != null && typedSymbol.TemplateParameters.Length > 0)
                return result.GenerateFromTemplate(typedSymbol);
            else
                return result;
        }

        /// <summary>
        /// Get the symbols for the class with the given name.
        /// </summary>
        /// <param name="type">The name of the type to get symbols for.</param>
        /// <returns>The requested symbols or null if they aren't found.</returns>
        public SymbolTable GetSymbols(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            // do initial lookup
            SymbolTable result = LookupSymbol(type);
            if (result != null)
                return result;

            type = type.ToLower();

            // check for inner class reference
            if (type.Contains('.'))
            {
                string[] parts = type.Split('.');
                if (parts.Length > 0)
                {
                    result = LookupSymbol(parts[0]);
                    if (result == null)
                        return null;

                    bool found = false;
                    for (int i = 1; i < parts.Length; i++)
                    {
                        found = false;
                        foreach (SymbolTable innerClass in result.InnerClasses)
                        {
                            if (innerClass.Id == parts[i])
                            {
                                found = true;
                                result = innerClass;
                                break;
                            }
                        }

                        if (!found)
                            return null;
                    }

                    return result;
                }
            }

            // look for a trigger with the same name
            if (_triggers.ContainsKey(type))
                return _triggers[type];

            return null;
        }

        /// <summary>
        /// Resets the symbol cache.
        /// </summary>
        public void ResetSymbols()
        {
            lock (_classes)
            {
                _classes.Clear();
                foreach (string file in Directory.GetFiles(SymbolsFolder))
                    File.Delete(file);
            }
        }

        /// <summary>
        /// Update the symbols in the manager.
        /// </summary>
        /// <param name="symbols">The symbols to update.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        /// <param name="merge">If set to true, inner classes will be merged into existing classes when available.</param>
        public void UpdateSymbols(SymbolTable symbols, bool replace, bool save, bool merge)
        {
            UpdateSymbols(new SymbolTable[] { symbols }, replace, save, merge);
        }

        /// <summary>
        /// Update the symbols in the manager.
        /// </summary>
        /// <param name="symbols">The symbols to update.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        /// <param name="merge">If set to true, inner classes will be merged into existing classes when available.</param>
        public void UpdateSymbols(IEnumerable<SymbolTable> symbols, bool replace, bool save, bool merge)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");

            lock (_classes)
            {
                foreach (SymbolTable st in symbols)
                {
                    if (st == null)
                        continue;

                    // store triggers separate from classes
                    if (st.TableType == SymbolTableType.Trigger)
                    {
                        _triggers.Remove(st.Id);
                        _triggers.Add(st.Id, st);
                        continue;
                    }

                    // update symbols in memory
                    if (merge)
                    {
                        if (_classes.ContainsKey(st.Id))
                        {
                            SymbolTable container = _classes[st.Id];
                            foreach (SymbolTable innerClass in st.InnerClasses)
                                container.MergeInnerClass(innerClass);
                        }
                        else
                        {
                            _classes.Add(st.Id, st);
                        }
                    }
                    else if (replace)
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
                        UpdateSymbols(innerClass, replace, save, false);
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
        /// <param name="merge">If set to true, inner classes will be merged into existing classes when available.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(Stream stream, bool replace, bool save, bool merge)
        {
            if (stream == null)
                return new ParseResult(null, null, new LanguageError[0]);

            ApexParser parser = new ApexParser(new ApexLexer(stream));

            ParseResult result = parser.ParseApex();

            if (result.Symbols != null)
                UpdateSymbols(new SymbolTable[] { result.Symbols }, replace, save, merge);

            return result;
        }

        /// <summary>
        /// Parse the given text for an apex document.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        /// <param name="save">If set to true the symbols will be saved to file if a SymbolsFolder has been set.</param>
        /// <param name="merge">If set to true, inner classes will be merged into existing classes when available.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(string text, bool replace, bool save, bool merge)
        {
            if (String.IsNullOrWhiteSpace(text))
                return new ParseResult(null, null, new LanguageError[0]);

            using (MemoryStream reader = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                return ParseApex(reader, replace, save, merge);
            }
        }

        #endregion
    }
}
