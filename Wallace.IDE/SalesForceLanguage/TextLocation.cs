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
using System.Collections.Generic;

namespace SalesForceLanguage
{
    /// <summary>
    /// The location of a piece of text.
    /// </summary>
    public class TextLocation : IComparable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        internal TextLocation()
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
        internal TextLocation(int startPosition, int endPosition, int startLine, int startColumn, int endLine, int endColumn)
            : this()
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
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
        /// Split this location into multiple locations based on the newline characters that appear in the text.
        /// </summary>
        /// <param name="text">The text which contains newline characters to split on.</param>
        /// <returns>The locations split into multiple lines.</returns>
        public TextLocation[] Split(string text)
        {
            List<TextLocation> result = new List<TextLocation>();

            if (!IsMultiline || text == null)
            {
                result.Add(this);
                return result.ToArray();
            }

            string[] lines = text.Split(new char[] { '\n' });
            int offset = StartPosition;
            for (int i = 0; i < lines.Length; i++)
            {
                result.Add(new TextLocation(
                    offset,
                    offset + lines[i].Length + 1,
                    StartLine + i,
                    (i == 0) ? StartColumn : 1,
                    StartLine + i,
                    (i == lines.Length - 1) ? EndColumn : lines[i].Length + 2));

                offset += lines[i].Length + 1;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Check to see if the given location is contained within this location.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns>true if the given location is contained entirely within this location.</returns>
        public bool ContainsLocation(TextLocation location)
        {
            if (location == null)
                return false;

            return (this.StartPosition <= location.StartPosition &&
                    this.EndPosition >= location.EndPosition);
        }

        /// <summary>
        /// Checks to see if the given offset is contained within this location.
        /// </summary>
        /// <param name="offset">The offset to check for.</param>
        /// <returns>true if the given offset is contained within this location.</returns>
        public bool ContainsOffset(int offset)
        {
            return (StartPosition <= offset && EndPosition >= offset);
        }

        /// <summary>
        /// Create a segment from this location.
        /// </summary>
        /// <param name="startPosition">The start position to use.</param>
        /// <param name="endPosition">The end position to use.</param>
        /// <returns>The segment if there is overlap, otherwise null.</returns>
        public TextLocation CreateSegment(int startPosition, int endPosition)
        {            
            int start = Math.Max(startPosition, this.StartPosition);
            int end = Math.Min(endPosition, this.EndPosition);

            if (start > end)
                return null;
            else
                return new TextLocation(start, end, -1, -1, -1, -1);
        }

        /// <summary>
        /// Returns a human readable string that represents this object.
        /// </summary>
        /// <returns>A human readable string that represents this object.</returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}) -> ({2}, {3})", StartLine, StartColumn, EndLine, EndColumn);
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
            TextLocation other = obj as TextLocation;
            if (other == null)
                return -1;

            if (this.StartPosition == other.StartPosition)
            {
                if (this.EndPosition == other.EndPosition)
                    return 0;
                else
                    return (this.Length - other.Length);
            }
            else
            {
                return (this.StartPosition - other.StartPosition);
            }
        }

        #endregion
    }
}
