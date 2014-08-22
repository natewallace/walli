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

using QUT.Gppg;
using System;

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// A span of text.  Both line and column are one based.
    /// </summary>
    public class ApexTextSpan : IMerge<ApexTextSpan>, IComparable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApexTextSpan()
        {
            StartPosition = 0;
            EndPosition = 0;
            StartLine = 0;
            StartColumn = 0;
            EndLine = 0;
            EndColumn = 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startPosition">StartPosition.</param>
        /// <param name="endPosition">EndPosition.</param>
        /// <param name="startLine">StartLine.</param>
        /// <param name="startColumn">StartColumn.</param>
        /// <param name="endLine">EndLine.</param>
        /// <param name="endColumn">EndColumn.</param>
        public ApexTextSpan(int startPosition, int endPosition, int startLine, int startColumn, int endLine, int endColumn)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        #endregion

        #region IMerge<ApexTextSpan> Members

        /// <summary>
        /// Create a text location which spans from the 
        /// start of "this" to the end of the argument "last"
        /// </summary>
        /// <param name="last">The last location in the result span</param>
        /// <returns>The merged span</returns>
        public ApexTextSpan Merge(ApexTextSpan last)
        {
            return new ApexTextSpan(StartPosition, last.EndPosition, StartLine, StartColumn, last.EndLine, last.EndColumn);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The position at which the text span starts.  0 based.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// The position of the first character beyond the text span.  0 based.
        /// </summary>
        public int EndPosition { get; private set; }

        /// <summary>
        /// The line at which the text starts. 1 based.
        /// </summary>
        public int StartLine { get; private set; }

        /// <summary>
        /// The column at which the text starts. 1 based.
        /// </summary>
        public int StartColumn { get; private set; }

        /// <summary>
        /// The line on which the text ends. 1 based.
        /// </summary>
        public int EndLine { get; private set; }

        /// <summary>
        /// The column of the first character beyond the end of the text. 1 based.
        /// </summary>
        public int EndColumn { get; private set; }

        /// <summary>
        /// The length of the text location.
        /// </summary>
        public int Length
        {
            get
            {
                return EndPosition - StartPosition;
            }
        }

        /// <summary>
        /// If this symbol spans more than one line this property is true.
        /// </summary>
        public bool IsMultiline
        {
            get
            {
                return (StartLine != EndLine);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check to see if the given position is contained within this text span.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>true if the given position is within this text span, false if it isn't.</returns>
        internal bool Contains(TextPosition position)
        {
            if (StartLine > position.Line)
                return false;

            if (StartLine == position.Line)
                return (EndLine > StartLine ||
                        StartColumn <= position.Column && EndColumn >= position.Column);

            if (StartLine < position.Line)
            {
                if (EndLine == position.Line)
                    return (EndColumn >= position.Column);
                else
                    return (EndLine > position.Line);
            }

            return false;
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
            if (obj is TextSpan)
            {
                TextSpan other = obj as TextSpan;

                if (this.StartLine == other.StartPosition.Line)
                {
                    if (this.StartColumn == other.StartPosition.Column)
                        return 0;
                    else
                        return (this.StartColumn - other.StartPosition.Column);
                }
                else
                {
                    return (this.StartLine - other.StartPosition.Line);
                }
            }
            else if (obj is ApexTextSpan)
            {
                ApexTextSpan other = obj as ApexTextSpan;

                if (this.StartLine == other.StartLine)
                {
                    if (this.StartColumn == other.StartColumn)
                        return 0;
                    else
                        return (this.StartColumn - other.StartColumn);
                }
                else
                {
                    return (this.StartLine - other.StartLine);
                }
            }
            else
            {
                return -1;
            }
        }

        #endregion
    }
}
