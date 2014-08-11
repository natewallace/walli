/*
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
using System.IO;
using System.Text;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.Parser;
using System.Collections.Generic;
using SalesForceLanguage.Apex.CodeModel;

namespace SalesForceLanguage
{
    /// <summary>
    /// Class used to manage language functions.
    /// </summary>
    public class LanguageManager
    {
        #region Fields

        /// <summary>
        /// Holds all classes that have been parsed.
        /// </summary>
        private Dictionary<string, SymbolTable> _classes;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public LanguageManager()
        {
            _classes = new Dictionary<string, SymbolTable>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the symbols in the manager.
        /// </summary>
        /// <param name="symbols">The symbols to update.</param>
        /// <param name="replace">If true, existing symbols will be replaced.  If false, existing symbols will not be replaced.</param>
        public void UpdateSymbols(IEnumerable<SymbolTable> symbols, bool replace)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");

            lock (_classes)
            {
                foreach (SymbolTable st in symbols)
                {
                    string key = st.Name.ToLower();
                    if (replace)
                    {
                        _classes.Remove(key);
                        _classes.Add(key, st);
                    }
                    else
                    {
                        if (!_classes.ContainsKey(key))
                            _classes.Add(key, st);
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

            _classes.Remove(name.ToLower());
        }

        /// <summary>
        /// Parse the given text for an apex document.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return new ParseResult(null, null, new LanguageError[0]);

            using (MemoryStream reader = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                ApexParser parser = new ApexParser(new ApexLexer(reader));

                parser.ParseApex();

                if (parser.Symbols != null)
                    UpdateSymbols(new SymbolTable[] { parser.Symbols }, true);

                return new ParseResult(
                    parser.Symbols,
                    parser.TypeReferences,
                    parser.ParserErrors);
            }
        }

        #endregion
    }
}
