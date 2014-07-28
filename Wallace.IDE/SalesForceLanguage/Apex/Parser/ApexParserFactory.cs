﻿/*
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

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// This class is used to process productions generated by the parser.
    /// </summary>
    public class ApexParserFactory
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbolDocument">SymbolDocument.</param>
        public ApexParserFactory(TextSymbolDocument symbolDocument)
        {
            SymbolDocument = symbolDocument;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The document to add symbols to.
        /// </summary>
        public TextSymbolDocument SymbolDocument { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Process the the given token.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <param name="span">The text span of the given token.</param>
        /// <param name="nodes">The nodes that reduce to the given token.</param>
        /// <returns>The resulting node from the reduce operation.</returns>
        public ApexSyntaxNode Process(Tokens token, ApexTextSpan span, ApexSyntaxNode[] nodes)
        {
            ApexSyntaxNode node = new ApexSyntaxNode(token, span, nodes);

            if (SymbolDocument != null)
            {
                switch (node.Token)
                {
                    // mark type references
                    case Tokens.ProductionReferenceType:
                        foreach (ApexSyntaxNode n in node.GetNodesWithText())
                            SymbolDocument.Add(new TextSymbol(TextSymbolType.TypeReference, n.TextSpan));
                        break;

                    // mark class and interface names
                    case Tokens.ProductionClassDeclaration:
                    case Tokens.ProductionInterfaceDeclaration:
                        SymbolDocument.Add(new TextSymbol(
                            TextSymbolType.TypeReference,
                            node.GetChildNodeWithToken(Tokens.ProductionSimpleName).TextSpan));
                        break;

                    default:
                        break;
                }
            }

            return node;
        }

        #endregion
    }
}
