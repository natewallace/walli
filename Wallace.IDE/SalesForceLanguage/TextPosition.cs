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

using SalesForceLanguage.Apex.Parser;
using System;

namespace SalesForceLanguage
{
    /// <summary>
    /// A position in text.
    /// </summary>
    public struct TextPosition : IComparable
    {
        #region Fields

        /// <summary>
        /// Supports the Line property.
        /// </summary>
        private int _line;

        /// <summary>
        /// Supports the Column property.
        /// </summary>
        private int _column;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line.</param>
        /// <param name="column">Column.</param>
        public TextPosition(int line, int column)
        {
            _line = line;
            _column = column;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="textSpan">A text span whose start line and column will be read in.</param>
        public TextPosition(ApexTextSpan textSpan)
        {
            _line = textSpan.StartLine;
            _column = textSpan.StartColumn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The line the symbol appears on.
        /// </summary>
        public int Line
        {
            get
            {
                return _line;
            }
        }

        /// <summary>
        /// The column the symbol appears on.
        /// </summary>
        public int Column
        {
            get
            {
                return _column;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shift the line positions by the given offset.
        /// </summary>
        /// <param name="offset">The offset to apply to line positions.</param>
        /// <returns>The new text position with the given offset.</returns>
        public TextPosition CreateLineOffset(int offset)
        {
            return new TextPosition(Line + offset, Column);
        }

        /// <summary>
        /// Returns a human readible string for this object.
        /// </summary>
        /// <returns>A human readible string for this object.</returns>
        public override string ToString()
        {
            return String.Format("{0}, {1}", Line, Column);
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: 
        /// Less than zero - This instance precedes obj in the sort order. 
        /// Zero - This instance occurs in the same position in the sort order as obj. 
        /// Greater than zero - This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (!(obj is TextPosition))
                return -1;

            TextPosition other = (TextPosition)obj;

            if (this.Line == other.Line)
            {
                if (this.Column == other.Column)
                    return 0;
                else
                    return (this.Column - other.Column);
            }
            else
            {
                return (this.Line - other.Line);
            }
        }

        #endregion
    }
}
