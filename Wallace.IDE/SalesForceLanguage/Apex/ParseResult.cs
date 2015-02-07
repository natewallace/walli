/*
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

using System.Collections.Generic;
using SalesForceLanguage.Apex.CodeModel;

namespace SalesForceLanguage.Apex
{
    /// <summary>
    /// The result of parsing a class.
    /// </summary>
    public class ParseResult
    {
        #region Fields

        /// <summary>
        /// Supports the ErrorsByLine property.
        /// </summary>
        private Dictionary<int, List<LanguageError>> _errorsByLine;

        /// <summary>
        /// Supports the TypeReferencesByLine property.
        /// </summary>
        private Dictionary<int, List<TextSpan>> _typeReferencesByLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        /// <param name="errors">Errors.</param>
        public ParseResult(SymbolTable symbols, ReferenceTypeSymbol[] typeReferences, LanguageError[] errors)
        {
            Symbols = symbols;
            TypeReferences = typeReferences ?? new ReferenceTypeSymbol[0];
            Errors = errors ?? new LanguageError[0];

            _errorsByLine = null;
            _typeReferencesByLine = null;
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
        public ReferenceTypeSymbol[] TypeReferences { get; private set; }

        /// <summary>
        /// Get the type references organzied by line number.
        /// </summary>
        public Dictionary<int, List<TextSpan>> TypeReferencesByLine
        {
            get
            {
                if (_typeReferencesByLine == null)
                {
                    _typeReferencesByLine = new Dictionary<int, List<TextSpan>>();
                    if (TypeReferences != null)
                    {
                        foreach (ReferenceTypeSymbol s in TypeReferences)
                        {
                            List<TextSpan> spans = null;
                            if (_typeReferencesByLine.ContainsKey(s.Location.Line))
                            {
                                spans = _typeReferencesByLine[s.Location.Line];
                            }
                            else
                            {
                                spans = new List<TextSpan>();
                                _typeReferencesByLine.Add(s.Location.Line, spans);
                            }

                            foreach (TextSpan span in s.Parts)
                                spans.Add(span);
                        }
                    }
                }

                return _typeReferencesByLine;
            }
        }

        /// <summary>
        /// Any language errors that occured.
        /// </summary>
        public LanguageError[] Errors { get; private set; }

        /// <summary>
        /// Get the errors organized by line number.
        /// </summary>
        public Dictionary<int, List<LanguageError>> ErrorsByLine
        {
            get
            {
                if (_errorsByLine == null)
                {
                    _errorsByLine = new Dictionary<int, List<LanguageError>>();
                    if (Errors != null)
                    {
                        foreach (LanguageError e in Errors)
                        {
                            if (_errorsByLine.ContainsKey(e.Location.StartPosition.Line))
                            {
                                _errorsByLine[e.Location.StartPosition.Line].Add(e);
                            }
                            else
                            {
                                List<LanguageError> list = new List<LanguageError>();
                                list.Add(e);
                                _errorsByLine.Add(e.Location.StartPosition.Line, list);
                            }
                        }
                    }
                }

                return _errorsByLine;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shift the lines of all text positions by the given offset.
        /// </summary>
        /// <param name="offset">The offset to apply to all text position lines.</param>
        public void ApplyLineOffset(int offset)
        {
            // type references
            foreach (ReferenceTypeSymbol symbol in TypeReferences)
                symbol.ApplyLineOffset(offset);
            _typeReferencesByLine = null;

            // errors
            foreach (LanguageError error in Errors)
                error.ApplyLineOffset(offset);
            _errorsByLine = null;

            // symbols
            if (Symbols != null)
                Symbols.ApplyLineOffset(offset);
        }

        #endregion
    }
}
