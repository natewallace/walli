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

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// A span of text.  Both line and column are one based.
    /// </summary>
    public class ApexTextSpan : TextLocation, IMerge<ApexTextSpan>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApexTextSpan()
            : base()
        {
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
            : base(startPosition, endPosition, startLine, startColumn, endLine, endColumn)
        {
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
    }
}
