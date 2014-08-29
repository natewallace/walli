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
using System.Text;

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// A node in an Apex syntax tree.
    /// </summary>
    public class ApexSyntaxNode
    {
        #region Fields

        /// <summary>
        /// Supports the text property.
        /// </summary>
        private string _text;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="textSpan">TextSpan.</param>
        public ApexSyntaxNode(Tokens token, ApexTextSpan textSpan)
        {
            Token = token;
            TextSpan = textSpan;

            Nodes = new List<ApexSyntaxNode>();
            Text = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="textSpan">TextSpan.</param>
        /// <param name="nodes">Nodes.</param>
        public ApexSyntaxNode(Tokens token, ApexTextSpan textSpan, params ApexSyntaxNode[] nodes)
        {
            Token = token;
            TextSpan = textSpan;
            if (nodes == null)
                Nodes = new List<ApexSyntaxNode>();
            else
                Nodes = new List<ApexSyntaxNode>(nodes);

            Text = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="textSpan">TextSpan.</param>
        /// <param name="value">Value.</param>
        /// <param name="nodes">Nodes.</param>
        public ApexSyntaxNode(Tokens token, ApexTextSpan textSpan, string value, params ApexSyntaxNode[] nodes)
        {
            Token = token;
            TextSpan = textSpan;
            
            if (nodes == null)
                Nodes = new List<ApexSyntaxNode>();
            else
                Nodes = new List<ApexSyntaxNode>(nodes);

            Text = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The token that identifies the type of node this is.
        /// </summary>
        public Tokens Token { get; private set; }

        /// <summary>
        /// The text this nodes spans.
        /// </summary>
        public ApexTextSpan TextSpan { get; private set; }

        /// <summary>
        /// The text for this node if the token is a variable text type, otherwise it's set to null.
        /// </summary>
        public string Text
        {
            get
            {
                //if (Token == Tokens.ProductionNonReservedIdentifier)
                //    return GetText(Nodes[0].Token);
                //else
                    return _text;
            }
            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Text that can be used for display.
        /// </summary>
        public string DisplayText
        {
            get
            {
                switch (Token)
                {
                    case Tokens.IDENTIFIER:
                    case Tokens.LITERAL_DOUBLE:
                    case Tokens.LITERAL_INTEGER:
                    case Tokens.LITERAL_LONG:
                    case Tokens.LITERAL_STRING:
                        return Text;

                    default:
                        return GetText(Token);
                }
            }
        }

        /// <summary>
        /// Nodes that are branched from this node.
        /// </summary>
        public IReadOnlyList<ApexSyntaxNode> Nodes { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the token as the string.
        /// </summary>
        /// <returns>The token string value.</returns>
        public override string ToString()
        {
            return Token.ToString();
        }

        /// <summary>
        /// Convert the given token to a text value.
        /// </summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>The text representation of the given token.</returns>
        private static string GetText(Tokens token)
        {
            switch (token)
            {
                case Tokens.KEYWORD_ABSTRACT:
                    return "abstract";
                case Tokens.KEYWORD_AFTER:
                    return "after";
                case Tokens.KEYWORD_ANNOTATE:
                    return "@";
                case Tokens.KEYWORD_BEFORE:
                    return "before";
                case Tokens.KEYWORD_BLOB:
                    return "blob";
                case Tokens.KEYWORD_BOOLEAN:
                    return "boolean";
                case Tokens.KEYWORD_BREAK:
                    return "break";
                case Tokens.KEYWORD_CATCH:
                    return "catch";
                case Tokens.KEYWORD_CLASS:
                    return "class";
                case Tokens.KEYWORD_CONTINUE:
                    return "continue";
                case Tokens.KEYWORD_DATE:
                    return "date";
                case Tokens.KEYWORD_DATETIME:
                    return "datetime";
                case Tokens.KEYWORD_DECIMAL:
                    return "decimal";
                case Tokens.KEYWORD_DELETE:
                    return "delete";
                case Tokens.KEYWORD_DO:
                    return "do";
                case Tokens.KEYWORD_DOUBLE:
                    return "double";
                case Tokens.KEYWORD_ELSE:
                    return "else";
                case Tokens.KEYWORD_ENUM:
                    return "enum";
                case Tokens.KEYWORD_EXTENDS:
                    return "extends";
                case Tokens.KEYWORD_FINAL:
                    return "final";
                case Tokens.KEYWORD_FINALLY:
                    return "finally";
                case Tokens.KEYWORD_FOR:
                    return "for";
                case Tokens.KEYWORD_GET:
                    return "get";
                case Tokens.KEYWORD_GLOBAL:
                    return "global";
                case Tokens.KEYWORD_ID:
                    return "id";
                case Tokens.KEYWORD_IF:
                    return "if";
                case Tokens.KEYWORD_IMPLEMENTS:
                    return "implements";
                case Tokens.KEYWORD_INSERT:
                    return "insert";
                case Tokens.KEYWORD_INTERFACE:
                    return "interface";
                case Tokens.KEYWORD_INTEGER:
                    return "integer";
                case Tokens.KEYWORD_LONG:
                    return "long";
                case Tokens.KEYWORD_MERGE:
                    return "merge";
                case Tokens.KEYWORD_NEW:
                    return "new";
                case Tokens.KEYWORD_ON:
                    return "on";
                case Tokens.KEYWORD_OVERRIDE:
                    return "override";
                case Tokens.KEYWORD_PRIVATE:
                    return "private";
                case Tokens.KEYWORD_PROTECTED:
                    return "protected";
                case Tokens.KEYWORD_PUBLIC:
                    return "public";
                case Tokens.KEYWORD_RETURN:
                    return "return";
                case Tokens.KEYWORD_ROLLBACK:
                    return "rollback";
                case Tokens.KEYWORD_SET:
                    return "set";
                case Tokens.KEYWORD_STATIC:
                    return "static";
                case Tokens.KEYWORD_STRING:
                    return "string";
                case Tokens.KEYWORD_SUPER:
                    return "super";
                case Tokens.KEYWORD_SYSTEM:
                    return "system";
                case Tokens.KEYWORD_TESTMETHOD:
                    return "testmethod";
                case Tokens.KEYWORD_THIS:
                    return "this";
                case Tokens.KEYWORD_THROW:
                    return "throw";
                case Tokens.KEYWORD_TRIGGER:
                    return "trigger";
                case Tokens.KEYWORD_TRY:
                    return "try";
                case Tokens.KEYWORD_UNDELETE:
                    return "undelete";
                case Tokens.KEYWORD_UPDATE:
                    return "update";
                case Tokens.KEYWORD_UPSERT:
                    return "upsert";
                case Tokens.KEYWORD_VIRTUAL:
                    return "virtual";
                case Tokens.KEYWORD_VOID:
                    return "void";
                case Tokens.KEYWORD_WEBSERVICE:
                    return "webservice";
                case Tokens.KEYWORD_WHILE:
                    return "while";
                case Tokens.KEYWORD_WITHSHARING:
                    return "with sharing";
                case Tokens.KEYWORD_WITHOUTSHARING:
                    return "without sharing";
                case Tokens.LITERAL_TRUE:
                    return "true";
                case Tokens.LITERAL_FALSE:
                    return "false";
                case Tokens.LITERAL_NULL:
                    return "null";
                case Tokens.OPERATOR_ASSIGNMENT:
                    return "=";
                case Tokens.OPERATOR_ASSIGNMENT_MAP:
                    return "=>";
                case Tokens.OPERATOR_ASSIGNMENT_ADDITION:
                    return "+=";
                case Tokens.OPERATOR_ASSIGNMENT_MULTIPLICATION:
                    return "*=";
                case Tokens.OPERATOR_ASSIGNMENT_SUBTRACTION:
                    return "-=";
                case Tokens.OPERATOR_ASSIGNMENT_DIVISION:
                    return "/=";
                case Tokens.OPERATOR_ASSIGNMENT_OR:
                    return "|=";
                case Tokens.OPERATOR_ASSIGNMENT_AND:
                    return "&=";
                case Tokens.OPERATOR_ASSIGNMENT_EXCLUSIVE_OR:
                    return "^=";
                case Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_LEFT:
                    return "<<=";
                case Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT:
                    return ">>=";
                case Tokens.OPERATOR_ASSIGNMENT_BITWISE_SHIFT_RIGHT_UNSIGNED:
                    return ">>>=";
                case Tokens.OPERATOR_AND:
                    return "&&";
                case Tokens.OPERATOR_OR:
                    return "||";
                case Tokens.OPERATOR_EQUALITY:
                    return "==";
                case Tokens.OPERATOR_EQUALITY_EXACT:
                    return "===";
                case Tokens.OPERATOR_LESS_THAN:
                    return "<";
                case Tokens.OPERATOR_GREATER_THAN:
                case Tokens.OPERATOR_GREATER_THAN_A:
                case Tokens.OPERATOR_GREATER_THAN_B:
                case Tokens.OPERATOR_GREATER_THAN_C:
                    return ">";
                case Tokens.OPERATOR_LESS_THAN_OR_EQUAL:
                    return "<=";
                case Tokens.OPERATOR_GREATER_THAN_OR_EQUAL:
                    return ">=";
                case Tokens.OPERATOR_INEQUALITY:
                    return "!=";
                case Tokens.OPERATOR_INEQUALITY_ALT:
                    return "<>";
                case Tokens.OPERATOR_INEQUALITY_EXACT:
                    return "!==";
                case Tokens.OPERATOR_ADDITION:
                    return "+";
                case Tokens.OPERATOR_SUBTRACTION:
                    return "-";
                case Tokens.OPERATOR_MULTIPLICATION:
                    return "*";
                case Tokens.OPERATOR_DIVISION:
                    return "/";
                case Tokens.OPERATOR_LOGICAL_COMPLEMENT:
                    return "!";
                case Tokens.OPERATOR_INCREMENT:
                    return "++";
                case Tokens.OPERATOR_DECREMENT:
                    return "--";
                case Tokens.OPERATOR_BITWISE_AND:
                    return "&";
                case Tokens.OPERATOR_BITWISE_OR:
                    return "|";
                case Tokens.OPERATOR_BITWISE_EXCLUSIVE_OR:
                    return "^";
                case Tokens.OPERATOR_BITWISE_SHIFT_LEFT:
                    return "<<";
                case Tokens.OPERATOR_QUESTION_MARK:
                    return "?";
                case Tokens.OPERATOR_INSTANCEOF:
                    return "instanceof";
                case Tokens.SEPARATOR_PARENTHESES_LEFT:
                    return "(";
                case Tokens.SEPARATOR_PARENTHESES_RIGHT:
                    return ")";
                case Tokens.SEPARATOR_BRACE_LEFT:
                    return "{";
                case Tokens.SEPARATOR_BRACE_RIGHT:
                    return "}";
                case Tokens.SEPARATOR_BRACKET_LEFT:
                    return "[";
                case Tokens.SEPARATOR_BRACKET_RIGHT:
                    return "]";
                case Tokens.SEPARATOR_SEMICOLON:
                    return ";";
                case Tokens.SEPARATOR_COLON:
                    return ":";
                case Tokens.SEPARATOR_COMMA:
                    return ",";
                case Tokens.SEPARATOR_DOT:
                    return ".";
                case Tokens.SEPARATOR_BRACKET_EMPTY:
                    return "[]";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get all the nodes within this branch that have the given token.
        /// </summary>
        /// <param name="token">The token to get nodes for.</param>
        /// <returns>The nodes within this branch that have the given token.</returns>
        public ApexSyntaxNode[] GetAllNodesWithToken(Tokens token)
        {
            List<ApexSyntaxNode> nodes = new List<ApexSyntaxNode>();
            GetAllNodesWithToken(token, this, nodes);
            return nodes.ToArray();
        }

        /// <summary>
        /// Get all the nodes within this branch that have the given token.
        /// </summary>
        /// <param name="token">The token to get nodes for.</param>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="nodes">The list to add matching nodes to.</param>
        public void GetAllNodesWithToken(Tokens token, ApexSyntaxNode node, IList<ApexSyntaxNode> nodes)
        {
            if (node != null)
            {
                if (node.Token == token)
                    nodes.Add(node);

                foreach (ApexSyntaxNode n in node.Nodes)
                    GetAllNodesWithToken(token, n, nodes);
            }
        }

        /// <summary>
        /// Get the first nodes found within this branch that have the given token.
        /// </summary>
        /// <param name="token">The token to get nodes for.</param>
        /// <returns>The nodes within this branch that have the given token.</returns>
        public ApexSyntaxNode[] GetNodesWithToken(Tokens token)
        {
            List<ApexSyntaxNode> nodes = new List<ApexSyntaxNode>();
            GetNodesWithToken(token, this, nodes);
            return nodes.ToArray();
        }

        /// <summary>
        /// Get the first nodes found within this branch that have the given token.
        /// </summary>
        /// <param name="token">The token to get nodes for.</param>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="nodes">The list to add matching nodes to.</param>
        public void GetNodesWithToken(Tokens token, ApexSyntaxNode node, IList<ApexSyntaxNode> nodes)
        {
            if (node != null)
            {
                if (node.Token == token)
                {
                    nodes.Add(node);
                }
                else
                {
                    foreach (ApexSyntaxNode n in node.Nodes)
                        GetNodesWithToken(token, n, nodes);
                }
            }
        }

        /// <summary>
        /// Get the first node found within this branch that has the given token.
        /// </summary>
        /// <param name="token">The token to get a node for.</param>
        /// <returns>The node within this branch that has the given token.</returns>
        public ApexSyntaxNode GetNodeWithToken(Tokens token)
        {
            return GetNodeWithToken(token, this);
        }

        /// <summary>
        /// Get the first node found within this branch that has the given token.
        /// </summary>
        /// <param name="token">The token to get a node for.</param>
        /// <param name="node">The current node to evaluate.</param>
        /// <returns>The node within this branch that has the given token.</returns>
        public ApexSyntaxNode GetNodeWithToken(Tokens token, ApexSyntaxNode node)
        {
            if (node != null)
            {
                if (node.Token == token)
                {
                    return node;
                }
                else
                {
                    ApexSyntaxNode result = null;
                    foreach (ApexSyntaxNode n in node.Nodes)
                    {
                        result = GetNodeWithToken(token, n);
                        if (result != null)
                            return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the first node found within this nodes children that matches the given token.
        /// </summary>
        /// <param name="token">The token to get nodes for.</param>
        /// <returns>The first node found within this nodes children that matches the given token or null if not found.</returns>
        public ApexSyntaxNode GetChildNodeWithToken(Tokens token)
        {
            foreach (ApexSyntaxNode child in Nodes)
                if (child != null && child.Token == token)
                    return child;

            return null;
        }

        /// <summary>
        /// Gets the first node with text.
        /// </summary>
        /// <returns>The first node with text or null if none are found.</returns>
        public ApexSyntaxNode GetNodeWithText()
        {
            ApexSyntaxNode[] nodes = GetNodesWithText();
            return (nodes.Length > 0) ? nodes[0] : null;
        }

        /// <summary>
        /// Gets all the nodes that have the Text property set.
        /// </summary>
        /// <returns>All the nodes that have the Text property set.</returns>
        public ApexSyntaxNode[] GetNodesWithText()
        {
            List<ApexSyntaxNode> nodes = new List<ApexSyntaxNode>();
            GetNodesWithText(this, nodes);
            return nodes.ToArray();
        }

        /// <summary>
        /// Recursive method that gets all the nodes that have text.
        /// </summary>
        /// <param name="node">The current node to evaluate.</param>
        /// <param name="nodes">The list to added nodes to.</param>
        private void GetNodesWithText(ApexSyntaxNode node, IList<ApexSyntaxNode> nodes)
        {
            if (node != null)
            {
                if (node.Text != null)
                    nodes.Add(node);

                foreach (ApexSyntaxNode n in node.Nodes)
                    GetNodesWithText(n, nodes);
            }
        }

        /// <summary>
        /// Get the leaves of the branch from this node.
        /// </summary>
        /// <returns>The tokens that are the leaves of the branch from this node.</returns>
        public Tokens[] GetLeaves()
        {
            List<Tokens> result = new List<Tokens>();
            GetLeaves(this, result);
            return result.ToArray();
        }

        /// <summary>
        /// Get the leaves of the branch from the given node.
        /// </summary>
        /// <param name="leaves">The list to add the leaves to.</param>
        private void GetLeaves(ApexSyntaxNode node, IList<Tokens> leaves)
        {
            if (node != null)
            {
                if (node.Nodes.Count == 0)
                {
                    leaves.Add(node.Token);
                }
                else
                {
                    foreach (ApexSyntaxNode n in node.Nodes)
                        GetLeaves(n, leaves);
                }
            }
        }

        /// <summary>
        /// Get the leaves display text of the branch from the given node.
        /// </summary>
        /// <param name="sb">The string builder to add the display text to.</param>
        public string GetLeavesDisplayText()
        {
            StringBuilder sb = new StringBuilder();
            GetLeavesDisplayText(this, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Get the leaves display text of the branch from the given node.
        /// </summary>
        /// <param name="sb">The string builder to add the display text to.</param>
        private void GetLeavesDisplayText(ApexSyntaxNode node, StringBuilder sb)
        {
            if (node != null)
            {
                if (node.Nodes.Count == 0)
                {
                    sb.Append(node.DisplayText);
                }
                else
                {
                    foreach (ApexSyntaxNode n in node.Nodes)
                        GetLeavesDisplayText(n, sb);
                }
            }
        }

        #endregion
    }
}
