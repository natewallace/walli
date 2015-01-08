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
using System.IO;

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// Extension to ApexLexer.
    /// </summary>
    public partial class ApexLexer
    {
        #region Fields

        /// <summary>
        /// Used for special handling of certain tokens.
        /// </summary>
        private Queue<ApexSyntaxNode> _pushBackQueue = new Queue<ApexSyntaxNode>();

        /// <summary>
        /// Stack used for state info.
        /// </summary>
        private Stack<ApexLexerStateInfo> _stateStack = new Stack<ApexLexerStateInfo>();

        /// <summary>
        /// When set to true, comments and whitespace tokens are returned.
        /// </summary>
        private bool _includeCommentsAndWhitespace = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="includeCommentsAndWhitespace">When set to true, comments and whitespace tokens are returned.</param>
        public ApexLexer(Stream text, bool includeCommentsAndWhitespace)
            : this(text)
        {
            _includeCommentsAndWhitespace = includeCommentsAndWhitespace;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the lexer to the given state.
        /// </summary>
        /// <param name="state">The state start.</param>
        /// <param name="token">The token for the state.</param>
        /// <param name="discard">true if this state will be discarded.</param>
        private void PushState(int state, Tokens token, bool discard)
        {
            yy_push_state(state);
            _stateStack.Push(new ApexLexerStateInfo(
                token,
                tokPos,
                tokLin,
                tokCol,
                discard));

            UpdateState();
        }

        /// <summary>
        /// Pop the current state.  If the state info shouldn't be discarded yylloc and yylval will be set.
        /// </summary>
        /// <returns>true if the state should not be discarded, false if it should.</returns>
        private bool PopState()
        {
            yy_pop_state();
            ApexLexerStateInfo info = _stateStack.Pop();
            if (info.Discard)
                return false;

            yylloc = new ApexTextSpan(
                info.StartPosition,
                tokEPos,
                info.StartLine,
                info.StartColumn + 1,
                tokELin,
                tokECol + 1); 

            yylval = new ApexSyntaxNode(info.Token, yylloc, info.Text.ToString());

            return true;
        }

        /// <summary>
        /// Update the state with the current text.
        /// </summary>
        private void UpdateState()
        {
            ApexLexerStateInfo info = _stateStack.Peek();
            info.Text.Append(yytext);
        }

        /// <summary>
        /// Get the current token that is set.
        /// </summary>
        /// <returns>The current token that is set.</returns>
        private int GetCurrentToken()
        {
            return (yylval == null) ? 0 : (int)yylval.Token;
        }

        /// <summary>
        /// Creates a new node using the given token and the current lexer state.
        /// </summary>
        /// <param name="token">The token to create a node for.</param>
        /// <returns>The newly created node.</returns>
        private ApexSyntaxNode CreateNode(Tokens token)
        {
            ApexSyntaxNode node = null;
            ApexTextSpan textSpan = new ApexTextSpan(
                tokPos,
                tokEPos,
                tokLin,
                tokCol + 1,
                tokELin,
                tokECol + 1);

            switch (token)
            {
                case Tokens.IDENTIFIER:
                case Tokens.LITERAL_DOUBLE:
                case Tokens.LITERAL_INTEGER:
                case Tokens.LITERAL_LONG:
                case Tokens.LITERAL_STRING:
                case Tokens.COMMENT_BLOCK:
                case Tokens.COMMENT_DOC:
                case Tokens.COMMENT_INLINE:
                case Tokens.WHITESPACE:
                    node = new ApexSyntaxNode(token, textSpan, yytext);
                    break;

                default:
                    node = new ApexSyntaxNode(token, textSpan);
                    break;
            }

            return node;
        }

        /// <summary>
        /// Process the given token.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>The symbol to use for the given token.</returns>
        private int Symbol(Tokens token)
        {
            yylval = CreateNode(token);
            yylloc = yylval.TextSpan;

            return (int)token;
        }

        /// <summary>
        /// Returns symbols for the right shift operation.
        /// </summary>
        /// <returns>Symbols for the right shift operation.</returns>
        private int SymbolShiftRight()
        {
            yylloc = new ApexTextSpan(tokPos, tokEPos - 1, tokLin, tokCol + 1, tokELin, tokECol);
            yylval = new ApexSyntaxNode(Tokens.OPERATOR_GREATER_THAN_A, yylloc);

            _pushBackQueue.Enqueue(new ApexSyntaxNode(
                Tokens.OPERATOR_GREATER_THAN_B,
                new ApexTextSpan(tokPos + 1, tokEPos, tokLin, tokCol + 2, tokELin, tokECol + 1)));

            return (int)Tokens.OPERATOR_GREATER_THAN_A;
        }

        /// <summary>
        /// Returns symbols for the right shift unsigned operation.
        /// </summary>
        /// <returns>Symbols for the right shift unsigned operation.</returns>
        private int SymbolShiftRightUnsigned()
        {
            yylloc = new ApexTextSpan(tokPos, tokEPos - 2, tokLin, tokCol + 1, tokELin, tokECol - 1);
            yylval = new ApexSyntaxNode(Tokens.OPERATOR_GREATER_THAN_A, yylloc);

            _pushBackQueue.Enqueue(new ApexSyntaxNode(
                Tokens.OPERATOR_GREATER_THAN_B,
                new ApexTextSpan(tokPos + 1, tokEPos - 1, tokLin, tokCol + 2, tokELin, tokECol)));

            _pushBackQueue.Enqueue(new ApexSyntaxNode(
                Tokens.OPERATOR_GREATER_THAN_C,
                new ApexTextSpan(tokPos + 2, tokEPos, tokLin, tokCol + 3, tokELin, tokECol + 1)));

            return (int)Tokens.OPERATOR_GREATER_THAN_A;
        }

        #endregion
    }
}
