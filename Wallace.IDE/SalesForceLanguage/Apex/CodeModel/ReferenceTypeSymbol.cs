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

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A reference type symbol.
    /// </summary>
    public class ReferenceTypeSymbol : Symbol
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="parts">Parts.</param>
        public ReferenceTypeSymbol(TextPosition location, string name, TextSpan span, TextSpan[] parts)
            : base(location, name, span)
        {
            Parts = parts ?? new TextSpan[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The individual parts of the type which should be highlighted.
        /// </summary>
        public TextSpan[] Parts { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Apply an offset to the line positions.
        /// </summary>
        /// <param name="offset">The offset to apply to the line positions.</param>
        public override void ApplyLineOffset(int offset)
        {
            base.ApplyLineOffset(offset);

            for (int i = 0; i < Parts.Length; i++)
                Parts[i] = Parts[i].CreateLineOffset(offset);
        }

        #endregion
    }
}
