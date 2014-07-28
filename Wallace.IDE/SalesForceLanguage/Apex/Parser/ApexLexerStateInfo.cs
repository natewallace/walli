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

using System.Text;

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// Used to maintain state info.
    /// </summary>
    internal class ApexLexerStateInfo
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="startPosition">StartPosition.</param>
        /// <param name="startLine">StartLine.</param>
        /// <param name="startColumn">StartColumn.</param>
        /// <param name="discard">Discard.</param>
        public ApexLexerStateInfo(Tokens token, int startPosition, int startLine, int startColumn, bool discard)
        {
            Token = token;
            StartPosition = startPosition;
            StartLine = startLine;
            StartColumn = startColumn;
            Text = new StringBuilder();
            Discard = discard;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The token for this state.
        /// </summary>
        public Tokens Token { get; private set; }

        /// <summary>
        /// The start position for this state.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// The start line for this state.
        /// </summary>
        public int StartLine { get; private set; }

        /// <summary>
        /// The start column for this state.
        /// </summary>
        public int StartColumn { get; private set; }

        /// <summary>
        /// The text for this state.
        /// </summary>
        public StringBuilder Text { get; private set; }

        /// <summary>
        /// If set to true this state will be discarded and will not generate a token.
        /// </summary>
        public bool Discard { get; private set; }

        #endregion
    }
}
