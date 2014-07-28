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

namespace SalesForceLanguage
{
    /// <summary>
    /// Class used to manage language functions.
    /// </summary>
    public class LanguageManager
    {
        #region Methods

        /// <summary>
        /// Parse the given text for an apex document.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult ParseApex(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return new ParseResult(
                    new TextSymbolDocument(),
                    new LanguageError[0],
                    new Apex.CodeModel.ICodeElement[0]);
            }

            using (MemoryStream reader = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                TextSymbolDocument symbolDocument = new TextSymbolDocument();

                ApexParser parser = new ApexParser(
                    new ApexLexer(reader, (TextSymbolDocument)null),
                    symbolDocument);

                parser.ParseApex();

                return new ParseResult(
                    symbolDocument,
                    parser.ParserErrors,
                    parser.Elements);
            }
        }

        #endregion
    }
}
