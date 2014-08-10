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

using SalesForceLanguage.Apex.CodeModel;
using System.Collections.Generic;

namespace SalesForceLanguage.Apex.Parser
{
    /// <summary>
    /// This class is used to process productions generated by the parser.
    /// </summary>
    public class ApexParserFactory
    {
        #region Fields

        /// <summary>
        /// Holds properties that have been defined.
        /// </summary>
        private Stack<VisibilitySymbol> _properties;

        /// <summary>
        /// Holds properties that have been defined.
        /// </summary>
        private Stack<Constructor> _constructors;

        /// <summary>
        /// Holds methods that have been defined.
        /// </summary>
        private Stack<Method> _methods;

        /// <summary>
        /// Holds classes/interfaces that have been defined.
        /// </summary>
        private Stack<SymbolTable> _classes;

        /// <summary>
        /// Holds type references that have been found.
        /// </summary>
        private List<Symbol> _typeReferences;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApexParserFactory()
        {
            _properties = new Stack<VisibilitySymbol>();
            _constructors = new Stack<Constructor>();
            _methods = new Stack<Method>();
            _classes = new Stack<SymbolTable>();
            _typeReferences = new List<Symbol>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The symbols that were parsed.
        /// </summary>
        public SymbolTable Symbols { get; private set; }

        /// <summary>
        /// The type references that were parsed.
        /// </summary>
        public Symbol[] TypeReferences
        {
            get { return _typeReferences.ToArray(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the visibility based on the given syntax nodes.
        /// </summary>
        /// <param name="nodes">The nodes to look through.  Each one should be a single modifier node.</param>
        /// <returns>The visibility.</returns>
        private SymbolVisibility GetVisibility(ApexSyntaxNode[] nodes)
        {
            SymbolVisibility visibility = SymbolVisibility.Private;
            foreach (ApexSyntaxNode node in nodes)
            {
                switch (node.Nodes[0].Token)
                {
                    case Tokens.KEYWORD_PRIVATE:
                    case Tokens.KEYWORD_PROTECTED:
                        return SymbolVisibility.Private;

                    case Tokens.KEYWORD_PUBLIC:
                        return SymbolVisibility.Public;

                    case Tokens.KEYWORD_GLOBAL:
                        return SymbolVisibility.Global;

                    default:
                        break;
                }
            }

            return visibility;
        }

        /// <summary>
        /// Get the parameters based on the given syntax nodes.
        /// </summary>
        /// <param name="nodes">The nodes to look through.  Each one should be a single parameter node.</param>
        /// <returns>The parameters.</returns>
        private Parameter[] GetParameters(ApexSyntaxNode[] nodes)
        {
            List<Parameter> list = new List<Parameter>();
            foreach (ApexSyntaxNode node in nodes)
            {
                list.Add(new Parameter(
                    node.Nodes[0].GetLeavesDisplayText(), 
                    node.Nodes[1].Nodes[0].DisplayText));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the symbols on the given stack that are within the node.
        /// </summary>
        /// <typeparam name="TType">The symbol type.</typeparam>
        /// <param name="node">The node to get the collected symbols for.</param>
        /// <param name="stack">The stack to get the symbols from.</param>
        /// <returns>The symbols that belong to the node.</returns>
        private TType[] GetSymbols<TType>(ApexSyntaxNode node, Stack<TType> stack) where TType : Symbol
        {
            List<TType> result = new List<TType>();
            while (true)
            {
                if (stack.Count == 0 || !node.TextSpan.Contains(stack.Peek().Location))
                    break;

                result.Insert(0, stack.Pop());
            }

            return result.ToArray();
        }

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

            switch (node.Token)
            {
                // type reference
                case Tokens.grammar_type:
                    ApexSyntaxNode typeNode = node.GetNodeWithToken(Tokens.IDENTIFIER);
                    if (typeNode != null)
                        _typeReferences.Add(new Symbol(
                            new TextPosition(typeNode.TextSpan),
                            typeNode.GetLeavesDisplayText(),
                            null));

                    break;

                // field
                case Tokens.grammar_field_declaration:
                    SymbolVisibility fieldVisibility = GetVisibility(node.GetNodesWithToken(Tokens.grammar_modifier));
                    string fieldType = node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();
                    ApexSyntaxNode[] fieldDeclarators = node.GetNodesWithToken(Tokens.grammar_variable_declarator);

                    foreach (ApexSyntaxNode declarator in fieldDeclarators)
                    {
                        _properties.Push(new VisibilitySymbol(
                            new TextPosition(declarator.Nodes[0].TextSpan),
                            declarator.Nodes[0].GetLeavesDisplayText(),
                            fieldType,
                            fieldVisibility));
                    }

                    break;

                // property
                case Tokens.grammar_property_declaration:
                    SymbolVisibility propertyVisibility = GetVisibility(node.GetNodesWithToken(Tokens.grammar_modifier));
                    string propertyType = node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();
                    ApexSyntaxNode propertyName = node.GetChildNodeWithToken(Tokens.grammar_identifier);

                    _properties.Push(new VisibilitySymbol(
                        new TextPosition(propertyName.TextSpan),
                        propertyName.GetLeavesDisplayText(),
                        propertyType,
                        propertyVisibility));

                    break;

                // constructor
                case Tokens.grammar_constructor_declaration:
                    SymbolVisibility constructorVisibility = GetVisibility(node.GetNodesWithToken(Tokens.grammar_modifier));
                    ApexSyntaxNode constructorName = node.GetNodeWithToken(Tokens.grammar_constructor_declarator).Nodes[0];
                    Parameter[] constructorParameters = GetParameters(node.GetNodesWithToken(Tokens.grammar_fixed_parameter));

                    _constructors.Push(new Constructor(
                        new TextPosition(constructorName.TextSpan),
                        constructorName.GetLeavesDisplayText(),
                        null,
                        constructorVisibility,
                        constructorParameters));

                    break;

                // method
                case Tokens.grammar_method_header:
                    SymbolVisibility methodVisibility = GetVisibility(node.GetNodesWithToken(Tokens.grammar_modifier));
                    ApexSyntaxNode methodName = node.GetChildNodeWithToken(Tokens.grammar_identifier);
                    Parameter[] methodParameters = GetParameters(node.GetNodesWithToken(Tokens.grammar_fixed_parameter));
                    string methodReturnType = (node.GetChildNodeWithToken(Tokens.KEYWORD_VOID) != null) ?
                        "void" :
                        node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();

                    _methods.Push(new Method(
                        new TextPosition(methodName.TextSpan),
                        methodName.GetLeavesDisplayText(),
                        null,
                        methodVisibility,
                        methodParameters,
                        methodReturnType));

                    break;

                // interface method
                case Tokens.grammar_interface_method_declaration:
                    ApexSyntaxNode interfaceMethodName = node.GetChildNodeWithToken(Tokens.grammar_identifier);
                    Parameter[] interfaceMethodParameters = GetParameters(node.GetNodesWithToken(Tokens.grammar_fixed_parameter));
                    string interfaceMethodReturnType = (node.GetChildNodeWithToken(Tokens.KEYWORD_VOID) != null) ?
                        "void" :
                        node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();

                    _methods.Push(new Method(
                        new TextPosition(interfaceMethodName.TextSpan),
                        interfaceMethodName.GetLeavesDisplayText(),
                        null,
                        SymbolVisibility.Public,
                        interfaceMethodParameters,
                        interfaceMethodReturnType));

                    break;

                // class
                case Tokens.grammar_class_declaration:
                    ApexSyntaxNode classModifiers = node.GetChildNodeWithToken(Tokens.grammar_modifiers);
                    SymbolVisibility classVisibility = SymbolVisibility.Private;
                    if (classModifiers != null)
                        classVisibility = GetVisibility(classModifiers.GetNodesWithToken(Tokens.grammar_modifier));

                    ApexSyntaxNode className = node.GetChildNodeWithToken(Tokens.grammar_identifier);

                    ApexSyntaxNode classBase = node.GetChildNodeWithToken(Tokens.grammar_class_base);
                    List<string> classInterfaces = new List<string>();
                    if (classBase != null)
                        foreach (ApexSyntaxNode classInterface in classBase.GetNodesWithToken(Tokens.grammar_interface_type))
                            classInterfaces.Add(classInterface.GetLeavesDisplayText());

                    _classes.Push(new SymbolTable(
                        new TextPosition(className.TextSpan),
                        className.GetLeavesDisplayText(),
                        GetSymbols<Constructor>(node, _constructors),
                        GetSymbols<VisibilitySymbol>(node, _properties),
                        GetSymbols<Method>(node, _methods),
                        classInterfaces.ToArray(),
                        GetSymbols<SymbolTable>(node, _classes)));

                    break;

                // interface
                case Tokens.grammar_interface_declaration:
                    ApexSyntaxNode interfaceModifiers = node.GetChildNodeWithToken(Tokens.grammar_modifiers);
                    SymbolVisibility interfaceVisibility = SymbolVisibility.Private;
                    if (interfaceModifiers != null)
                        interfaceVisibility = GetVisibility(interfaceModifiers.GetNodesWithToken(Tokens.grammar_modifier));

                    ApexSyntaxNode interfaceName = node.GetChildNodeWithToken(Tokens.grammar_identifier);

                    ApexSyntaxNode interfaceBase = node.GetChildNodeWithToken(Tokens.grammar_interface_base);
                    List<string> interfaceBases = new List<string>();
                    if (interfaceBase != null)
                        foreach (ApexSyntaxNode interfaceInterface in interfaceBase.GetNodesWithToken(Tokens.grammar_interface_type))
                            interfaceBases.Add(interfaceInterface.GetLeavesDisplayText());

                    _classes.Push(new SymbolTable(
                        new TextPosition(interfaceName.TextSpan),
                        interfaceName.GetLeavesDisplayText(),
                        null,
                        null,
                        GetSymbols<Method>(node, _methods),
                        interfaceBases.ToArray(),
                        GetSymbols<SymbolTable>(node, _classes)));

                    break;

                default:
                    break;
            }

            return node;
        }

        #endregion
    }
}
