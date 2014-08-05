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

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A generic symbol.
    /// </summary>
    public class Symbol
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public Symbol(TextPosition location, string name, string type)
        {
            Location = location;
            Name = name ?? String.Empty;
            Type = type ?? string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The location of the symbol.
        /// </summary>
        public TextPosition Location { get; private set; }

        /// <summary>
        /// The name for the symbol.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type for the symbol.
        /// </summary>
        public string Type { get; private set; }

        #endregion
    }
}
