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

using SalesForceLanguage.Apex.CodeModel;

namespace SalesForceLanguage.Apex
{
    /// <summary>
    /// The result of parsing a class.
    /// </summary>
    public class ParseResult
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        /// <param name="errors">Errors.</param>
        public ParseResult(SymbolTable symbols, Symbol[] typeReferences, LanguageError[] errors)
        {
            Symbols = symbols;
            TypeReferences = typeReferences ?? new Symbol[0];
            Errors = errors ?? new LanguageError[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The resulting symbols.
        /// </summary>
        public SymbolTable Symbols { get; private set; }

        /// <summary>
        /// The type references in the document.
        /// </summary>
        public Symbol[] TypeReferences { get; private set; }

        /// <summary>
        /// Any language errors that occured.
        /// </summary>
        public LanguageError[] Errors { get; private set; }

        #endregion
    }
}
