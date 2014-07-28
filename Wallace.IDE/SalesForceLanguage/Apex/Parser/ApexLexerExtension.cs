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
        private Queue<ApexSyntaxNode> _pushBackQueue;

        /// <summary>
        /// Stack used for state info.
        /// </summary>
        private Stack<ApexLexerStateInfo> _stateStack;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The file to scan from.</param>
        /// <param name="symbolDocument">SymbolDocument.</param>
        public ApexLexer(Stream file, TextSymbolDocument symbolDocument)
            : this(file)
        {
            SymbolDocument = symbolDocument;
            _pushBackQueue = new Queue<ApexSyntaxNode>();
            _stateStack = new Stack<ApexLexerStateInfo>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the last node that was scanned.
        /// </summary>
        public ApexSyntaxNode PreviousNode { get; private set; }

        /// <summary>
        /// The document that holds symbols that have been read in.
        /// </summary>
        public TextSymbolDocument SymbolDocument { get; private set; }

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

            if (SymbolDocument != null)
                SymbolDocument.AddRange(CreateSymbols(yylval));

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
        /// Creates a new symbol using the given node.
        /// </summary>
        /// <param name="node">The node to create a symbol for.</param>
        /// <returns>The newly created symbol.</returns>
        private TextSymbol[] CreateSymbols(ApexSyntaxNode node)
        {
            TextSymbolType symbolType = TextSymbolType.None;
            switch (node.Token)
            {
                case Tokens.WHITESPACE:
                    symbolType = TextSymbolType.Whitespace;
                    break;

                case Tokens.COMMENT_BLOCK:                
                case Tokens.COMMENT_DOCUMENTATION:
                case Tokens.COMMENT_INLINE:
                    symbolType = TextSymbolType.Comment;
                    break;

                case Tokens.COMMENT_DOC:
                    symbolType = TextSymbolType.CommentDoc;
                    break;

                case Tokens.LITERAL_STRING:
                    symbolType = TextSymbolType.String;
                    break;

                case Tokens.SOQL:
                    symbolType = TextSymbolType.SOQL;
                    break;

                case Tokens.SOSL:
                    symbolType = TextSymbolType.SOSL;
                    break;

                default:
                    if ((int)node.Token > (int)Tokens.COMMENT_DOCUMENTATION && (int)node.Token < (int)Tokens.LITERAL_TRUE)
                        symbolType = TextSymbolType.Keyword;
                    break;
            }

            if (node.TextSpan.IsMultiline)
            {
                List<TextSymbol> result = new List<TextSymbol>();
                foreach (TextLocation location in node.TextSpan.Split(node.Text))
                    result.Add(new TextSymbol(symbolType, location));

                return result.ToArray();
            }
            else
            {
                return new TextSymbol[] { new TextSymbol(symbolType, node.TextSpan) };
            }
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

            if (SymbolDocument != null)
                SymbolDocument.AddRange(CreateSymbols(yylval));

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

            if (SymbolDocument != null)
            {
                SymbolDocument.AddRange(CreateSymbols(yylval));
                foreach (ApexSyntaxNode n in _pushBackQueue)
                    SymbolDocument.AddRange(CreateSymbols(n));
            }

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

            if (SymbolDocument != null)
            {
                SymbolDocument.AddRange(CreateSymbols(yylval));
                foreach (ApexSyntaxNode n in _pushBackQueue)
                    SymbolDocument.AddRange(CreateSymbols(n));
            }

            return (int)Tokens.OPERATOR_GREATER_THAN_A;
        }

        #endregion
    }
}
