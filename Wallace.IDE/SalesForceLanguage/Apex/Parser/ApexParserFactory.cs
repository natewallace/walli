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
        /// Holds local variables that have been defined.
        /// </summary>
        private Stack<Field> _variables;

        /// <summary>
        /// Holds the local variable scopes that have been defined.
        /// </summary>
        private List<VariableScope> _variableScopes;

        /// <summary>
        /// Holds fields that have been defined.
        /// </summary>
        private Stack<Field> _fields;

        /// <summary>
        /// Holds fields on an enumeration.
        /// </summary>
        private Stack<Field> _enumFields;

        /// <summary>
        /// Holds properties that have been defined.
        /// </summary>
        private Stack<Property> _properties;

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
        private List<ReferenceTypeSymbol> _typeReferences;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApexParserFactory()
        {
            Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The symbols that were parsed.
        /// </summary>
        public SymbolTable Symbols
        {
            get
            {
                if (_classes.Count > 0)
                    return _classes.Peek();
                else
                    return null;
            }
        }

        /// <summary>
        /// The type references that were parsed.
        /// </summary>
        public ReferenceTypeSymbol[] TypeReferences
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
        private SymbolModifier GetModifiers(ApexSyntaxNode[] nodes)
        {
            int modifier = 0;
            foreach (ApexSyntaxNode node in nodes)
            {
                switch (node.Nodes[0].Token)
                {
                    case Tokens.KEYWORD_PUBLIC:
                        modifier |= (int)SymbolModifier.Public;
                        break;

	                case Tokens.KEYWORD_PROTECTED:
                        modifier |= (int)SymbolModifier.Protected;
                        break;

	                case Tokens.KEYWORD_PRIVATE:
                        modifier |= (int)SymbolModifier.Private;
                        break;

	                case Tokens.KEYWORD_ABSTRACT:
                        modifier |= (int)SymbolModifier.Abstract;
                        break;

	                case Tokens.KEYWORD_STATIC:
                        modifier |= (int)SymbolModifier.Static;
                        break;

	                case Tokens.KEYWORD_GLOBAL:
                        modifier |= (int)SymbolModifier.Global;
                        break;

	                case Tokens.KEYWORD_OVERRIDE:
                        modifier |= (int)SymbolModifier.Override;
                        break;

	                case Tokens.KEYWORD_VIRTUAL:
                        modifier |= (int)SymbolModifier.Virtual;
                        break;

	                case Tokens.KEYWORD_TESTMETHOD:
                        modifier |= (int)SymbolModifier.TestMethod;
                        break;

	                case Tokens.KEYWORD_TRANSIENT:
                        modifier |= (int)SymbolModifier.Transient;
                        break;

	                case Tokens.KEYWORD_WITHSHARING:
                        modifier |= (int)SymbolModifier.WithSharing;
                        break;

	                case Tokens.KEYWORD_WITHOUTSHARING:
                        modifier |= (int)SymbolModifier.WithoutSharing;
                        break;

	                case Tokens.KEYWORD_WEBSERVICE:
                        modifier |= (int)SymbolModifier.WebService;
                        break;

                    case Tokens.KEYWORD_FINAL:
                        modifier |= (int)SymbolModifier.Final;
                        break;

                    default:
                        break;
                }
            }

            return (SymbolModifier)modifier;
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
                    new TextPosition(node.TextSpan),
                    node.Nodes[1].Nodes[0].GetLeavesDisplayText(),
                    null,
                    node.Nodes[0].GetLeavesDisplayText()));
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
        /// Clear out all previously collected data.
        /// </summary>
        public void Clear()
        {
            _variableScopes = new List<VariableScope>();
            _variables = new Stack<Field>();
            _fields = new Stack<Field>();
            _enumFields = new Stack<Field>();
            _properties = new Stack<Property>();
            _constructors = new Stack<Constructor>();
            _methods = new Stack<Method>();
            _classes = new Stack<SymbolTable>();
            _typeReferences = new List<ReferenceTypeSymbol>();
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
                    ApexSyntaxNode[] identifierNodes = node.GetNodesWithToken(Tokens.grammar_identifier);
                    if (identifierNodes.Length > 0)
                    {
                        List<TextSpan> parts = new List<TextSpan>();
                        foreach (ApexSyntaxNode part in identifierNodes)
                            if (part.Nodes[0].Token != Tokens.grammar_non_reserved_identifier)
                                parts.Add(new TextSpan(part.TextSpan));

                        if (parts.Count > 0)
                            _typeReferences.Add(new ReferenceTypeSymbol(
                                new TextPosition(node.TextSpan),
                                node.GetLeavesDisplayText(),
                                new TextSpan(node.TextSpan),
                                parts.ToArray()));
                    }

                    break;

                // local variable
                case Tokens.grammar_local_variable_declaration:
                    string variableType = node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();
                    ApexSyntaxNode[] variableDeclarators = node.GetNodesWithToken(Tokens.grammar_local_variable_declarator);

                    foreach (ApexSyntaxNode declarator in variableDeclarators)
                    {
                        _variables.Push(new Field(
                            new TextPosition(declarator.Nodes[0].TextSpan),
                            declarator.Nodes[0].GetLeavesDisplayText(),
                            null,
                            SymbolModifier.Private,
                            variableType));
                    }

                    break;

                // variable scope
                case Tokens.grammar_block:
                case Tokens.grammar_embedded_statement:
                    _variableScopes.Add(new VariableScope(
                        new TextSpan(node.TextSpan),
                        GetSymbols<Field>(node, _variables)));

                    break;

                // field
                case Tokens.grammar_field_declaration:
                    SymbolModifier fieldVisibility = GetModifiers(node.GetNodesWithToken(Tokens.grammar_modifier));
                    string fieldType = node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();
                    ApexSyntaxNode[] fieldDeclarators = node.GetNodesWithToken(Tokens.grammar_variable_declarator);

                    foreach (ApexSyntaxNode declarator in fieldDeclarators)
                    {
                        _fields.Push(new Field(
                            new TextPosition(declarator.Nodes[0].TextSpan),
                            declarator.Nodes[0].GetLeavesDisplayText(),
                            null,
                            fieldVisibility,
                            fieldType));
                    }

                    break;

                // enum member
                case Tokens.grammar_enum_member_declaration:
                    _enumFields.Push(new Field(
                        new TextPosition(node.TextSpan),
                        node.GetLeavesDisplayText(),
                        null,
                        SymbolModifier.Final | SymbolModifier.Public | SymbolModifier.Static,
                        "System.Object"));

                    break;

                // property
                case Tokens.grammar_property_declaration:
                    SymbolModifier propertyVisibility = GetModifiers(node.GetNodesWithToken(Tokens.grammar_modifier));
                    string propertyType = node.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();
                    ApexSyntaxNode propertyName = node.GetChildNodeWithToken(Tokens.grammar_identifier);

                    _properties.Push(new Property(
                        new TextPosition(propertyName.TextSpan),
                        propertyName.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        propertyVisibility,
                        propertyType));

                    break;

                // constructor
                case Tokens.grammar_constructor_declaration:
                    SymbolModifier constructorVisibility = GetModifiers(node.GetNodesWithToken(Tokens.grammar_modifier));
                    ApexSyntaxNode constructorName = node.GetNodeWithToken(Tokens.grammar_constructor_declarator).Nodes[0];
                    Parameter[] constructorParameters = GetParameters(node.GetNodesWithToken(Tokens.grammar_fixed_parameter));

                    _constructors.Push(new Constructor(
                        new TextPosition(constructorName.TextSpan),
                        constructorName.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        constructorVisibility,
                        constructorParameters));

                    break;

                // method
                case Tokens.grammar_method_declaration:
                    ApexSyntaxNode methodHeader = node.GetNodeWithToken(Tokens.grammar_method_header);
                    SymbolModifier methodVisibility = GetModifiers(methodHeader.GetNodesWithToken(Tokens.grammar_modifier));
                    ApexSyntaxNode methodName = methodHeader.GetChildNodeWithToken(Tokens.grammar_identifier);
                    Parameter[] methodParameters = GetParameters(methodHeader.GetNodesWithToken(Tokens.grammar_fixed_parameter));
                    string methodReturnType = (methodHeader.GetChildNodeWithToken(Tokens.KEYWORD_VOID) != null) ?
                        "void" :
                        methodHeader.GetChildNodeWithToken(Tokens.grammar_type).GetLeavesDisplayText();

                    _methods.Push(new Method(
                        new TextPosition(methodName.TextSpan),
                        methodName.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        methodVisibility,
                        methodReturnType,
                        methodParameters));

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
                        new TextSpan(node.TextSpan),
                        SymbolModifier.Public,
                        interfaceMethodReturnType,
                        interfaceMethodParameters));

                    break;

                // class
                case Tokens.grammar_class_declaration:
                    ApexSyntaxNode classModifiers = node.GetChildNodeWithToken(Tokens.grammar_modifiers);
                    SymbolModifier classVisibility = SymbolModifier.Private;
                    if (classModifiers != null)
                        classVisibility = GetModifiers(classModifiers.GetNodesWithToken(Tokens.grammar_modifier));

                    ApexSyntaxNode className = node.GetChildNodeWithToken(Tokens.grammar_identifier);
                    _typeReferences.Add(new ReferenceTypeSymbol(
                        new TextPosition(className.TextSpan), 
                        className.GetLeavesDisplayText(), 
                        null,
                        new TextSpan[] { new TextSpan(className.TextSpan) }));

                    ApexSyntaxNode classBase = node.GetChildNodeWithToken(Tokens.grammar_class_base);
                    List<string> classInterfaces = new List<string>();
                    string classExtends = null;
                    if (classBase != null)
                    {
                        foreach (ApexSyntaxNode classInterface in classBase.GetNodesWithToken(Tokens.grammar_interface_type))
                        {
                            string name = classInterface.GetLeavesDisplayText();
                            classInterfaces.Add(name);
                            _typeReferences.Add(new ReferenceTypeSymbol(
                                new TextPosition(classInterface.TextSpan), 
                                name, 
                                null, 
                                new TextSpan[] { new TextSpan(classInterface.TextSpan) }));
                        }

                        ApexSyntaxNode extends = classBase.GetChildNodeWithToken(Tokens.grammar_class_type);
                        if (extends != null)
                        {
                            classExtends = extends.GetLeavesDisplayText();
                            _typeReferences.Add(new ReferenceTypeSymbol(
                                new TextPosition(extends.TextSpan),
                                classExtends,
                                null,
                                new TextSpan[] { new TextSpan(extends.TextSpan) }));
                        }
                    }

                    List<string> attributeList = new List<string>();
                    ApexSyntaxNode attributes = node.GetChildNodeWithToken(Tokens.grammar_attributes);
                    if (attributes != null)
                    {
                        foreach (ApexSyntaxNode attributeSection in attributes.GetNodesWithToken(Tokens.grammar_attribute_section))
                        {
                            attributeList.Add(attributeSection.GetChildNodeWithToken(Tokens.grammar_identifier).GetLeavesDisplayText());
                        }
                    }

                    _classes.Push(new SymbolTable(
                        new TextPosition(className.TextSpan),
                        className.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        attributeList.ToArray(),
                        classVisibility,
                        SymbolTableType.Class,
                        _variableScopes.ToArray(),
                        GetSymbols<Field>(node, _fields),
                        GetSymbols<Constructor>(node, _constructors),
                        GetSymbols<Property>(node, _properties),
                        GetSymbols<Method>(node, _methods),
                        classExtends,
                        classInterfaces.ToArray(),
                        GetSymbols<SymbolTable>(node, _classes)));

                    break;

                // interface
                case Tokens.grammar_interface_declaration:
                    ApexSyntaxNode interfaceModifiers = node.GetChildNodeWithToken(Tokens.grammar_modifiers);
                    SymbolModifier interfaceVisibility = SymbolModifier.Private;
                    if (interfaceModifiers != null)
                        interfaceVisibility = GetModifiers(interfaceModifiers.GetNodesWithToken(Tokens.grammar_modifier));

                    ApexSyntaxNode interfaceName = node.GetChildNodeWithToken(Tokens.grammar_identifier);
                    _typeReferences.Add(new ReferenceTypeSymbol(new TextPosition(interfaceName.TextSpan), interfaceName.GetLeavesDisplayText(), null, null));

                    ApexSyntaxNode interfaceBase = node.GetChildNodeWithToken(Tokens.grammar_interface_base);
                    List<string> interfaceBases = new List<string>();
                    string interfaceExtends = null;
                    if (interfaceBase != null)
                    {
                        foreach (ApexSyntaxNode interfaceInterface in interfaceBase.GetNodesWithToken(Tokens.grammar_interface_type))
                        {
                            string name = interfaceInterface.GetLeavesDisplayText();
                            interfaceBases.Add(interfaceInterface.GetLeavesDisplayText());
                            _typeReferences.Add(new ReferenceTypeSymbol(
                                new TextPosition(interfaceInterface.TextSpan), 
                                name, 
                                null, 
                                new TextSpan[] { new TextSpan(interfaceInterface.TextSpan) }));
                        }

                        ApexSyntaxNode extends = interfaceBase.GetChildNodeWithToken(Tokens.grammar_class_type);
                        if (extends != null)
                        {
                            interfaceExtends = extends.GetLeavesDisplayText();
                            _typeReferences.Add(new ReferenceTypeSymbol(
                                new TextPosition(extends.TextSpan),
                                interfaceExtends,
                                null,
                                new TextSpan[] { new TextSpan(extends.TextSpan) }));
                        }
                    }

                    _classes.Push(new SymbolTable(
                        new TextPosition(interfaceName.TextSpan),
                        interfaceName.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        null,
                        interfaceVisibility,
                        SymbolTableType.Interface,
                        null,
                        null,
                        null,
                        null,
                        GetSymbols<Method>(node, _methods),
                        interfaceExtends,
                        interfaceBases.ToArray(),
                        GetSymbols<SymbolTable>(node, _classes)));

                    break;

                // enum
                case Tokens.grammar_enum_declaration:
                    ApexSyntaxNode enumModifiers = node.GetChildNodeWithToken(Tokens.grammar_modifiers);
                    SymbolModifier enumVisibility = SymbolModifier.Private;
                    if (enumModifiers != null)
                        enumVisibility = GetModifiers(enumModifiers.GetNodesWithToken(Tokens.grammar_modifier));

                    ApexSyntaxNode enumName = node.GetChildNodeWithToken(Tokens.grammar_identifier);
                    _typeReferences.Add(new ReferenceTypeSymbol(
                        new TextPosition(enumName.TextSpan),
                        enumName.GetLeavesDisplayText(), 
                        null,
                        new TextSpan[] { new TextSpan(enumName.TextSpan) }));

                    List<string> enumAttributeList = new List<string>();
                    ApexSyntaxNode enumAttributes = node.GetChildNodeWithToken(Tokens.grammar_attributes);
                    if (enumAttributes != null)
                    {
                        foreach (ApexSyntaxNode attributeSection in enumAttributes.GetNodesWithToken(Tokens.grammar_attribute_section))
                        {
                            enumAttributeList.Add(attributeSection.GetChildNodeWithToken(Tokens.grammar_identifier).GetLeavesDisplayText());
                        }
                    }

                    _classes.Push(new SymbolTable(
                        new TextPosition(enumName.TextSpan),
                        enumName.GetLeavesDisplayText(),
                        new TextSpan(node.TextSpan),
                        enumAttributeList.ToArray(),
                        enumVisibility,
                        SymbolTableType.Enum,
                        null,
                        GetSymbols<Field>(node, _enumFields),
                        null,
                        null,
                        null,
                        null,
                        null,
                        null));

                    break;

                default:
                    break;
            }

            return node;
        }

        #endregion
    }
}
