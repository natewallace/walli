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
using System.Linq;
using System.Xml;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A class or interface.
    /// </summary>
    public class SymbolTable : TypedSymbol
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public SymbolTable()
        {
            Attributes = new string[0];
            TableType = SymbolTableType.Class;
            VariableScopes = new VariableScope[0];
            Fields = new Field[0];
            Constructors = new Constructor[0];
            Properties = new Property[0];
            Methods = new Method[0];
            Extends = null;
            Interfaces = new string[0];
            InnerClasses = new SymbolTable[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="modifier">Modifier.</param>
        /// <param name="tableType">TableType.</param>
        /// <param name="variableScopes">VariableScopes.</param>
        /// <param name="fields">Fields.</param>
        /// <param name="constructors">Constructors</param>
        /// <param name="properties">Properties</param>
        /// <param name="methods">Methods</param>
        /// <param name="extends">Extends.</param>
        /// <param name="interfaces">Interfaces</param>
        /// <param name="innerClasses">InnerClasses</param>
        public SymbolTable(
            TextPosition location,
            string name,
            TextSpan span,
            string[] attributes,
            SymbolModifier modifier,
            SymbolTableType tableType,
            VariableScope[] variableScopes,
            Field[] fields,
            Constructor[] constructors,
            Property[] properties,
            Method[] methods,
            string extends,
            string[] interfaces,
            SymbolTable[] innerClasses)
            : base(location, name, span, modifier, name)
        {
            Attributes = attributes ?? new string[0];
            TableType = tableType;
            VariableScopes = variableScopes ?? new VariableScope[0];
            Fields = fields ?? new Field[0];
            Constructors = constructors ?? new Constructor[0];
            Properties = properties ?? new Property[0];
            Methods = methods ?? new Method[0];
            Extends = extends;
            Interfaces = interfaces ?? new string[0];
            InnerClasses = innerClasses ?? new SymbolTable[0];

            // change class type to a namespace if the only thing in the class are inner classes
            if (TableType == SymbolTableType.Class &&
                InnerClasses.Length > 0 &&
                Fields.Length == 0 &&
                Constructors.Length == 0 &&
                Properties.Length == 0 &&
                Methods.Length == 0 &&
                Interfaces.Length == 0 &&
                String.IsNullOrWhiteSpace(Extends))
                TableType = SymbolTableType.Namespace;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of table this is.
        /// </summary>
        public SymbolTableType TableType { get; private set; }

        /// <summary>
        /// Local variables organzied by the scope they are defined in.
        /// </summary>
        public VariableScope[] VariableScopes { get; private set; }

        /// <summary>
        /// The attributes on this symbol table.
        /// </summary>
        public string[] Attributes { get; private set; }

        /// <summary>
        /// Fields for the table.
        /// </summary>
        public Field[] Fields { get; private set; }

        /// <summary>
        /// Constructors for the table.
        /// </summary>
        public Constructor[] Constructors { get; private set; }

        /// <summary>
        /// Properties for the table.
        /// </summary>
        public Property[] Properties { get; private set; }

        /// <summary>
        /// Methods for the table.
        /// </summary>
        public Method[] Methods { get; private set; }

        /// <summary>
        /// Interfaces or the table.
        /// </summary>
        public string[] Interfaces { get; private set; }

        /// <summary>
        /// The class this class extends if any.  If the class doesn't extend another class it will be null.
        /// </summary>
        public string Extends { get; private set; }

        /// <summary>
        /// Inner classes or interfaces for the table.
        /// </summary>
        public SymbolTable[] InnerClasses { get; private set; }

        /// <summary>
        /// Checks to see if this is a test class.
        /// </summary>
        public bool IsTest
        {
            get { return HasAttribute("istest"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate a concrete type from a template.
        /// </summary>
        /// <param name="symbol">The symbol to create a concrete type for.</param>
        /// <returns>The concrete type.</returns>
        public SymbolTable GenerateFromTemplate(TypedSymbol symbol)
        {
            List<Field> fields = new List<Field>();
            foreach (Field f in Fields)
            {
                fields.Add(new Field(f.Location, f.Name, f.Span, f.Modifier, ResolveType(f.Type, f.TemplateParameters)));
            }

            List<Constructor> constructors = new List<Constructor>();
            foreach (Constructor c in Constructors)
            {
                List<Parameter> parameters = new List<Parameter>();
                foreach (Parameter p in c.Parameters)
                    parameters.Add(new Parameter(p.Location, p.Name, p.Span, ResolveType(p.Type, symbol.TemplateParameters)));

                constructors.Add(new Constructor(c.Location, c.Name, c.Span, c.Modifier, parameters.ToArray()));
            }

            List<Property> properties = new List<Property>();
            foreach (Property p in Properties)
            {
                properties.Add(new Property(p.Location, p.Name, p.Span, p.Modifier, ResolveType(p.Type, symbol.TemplateParameters)));
            }

            List<Method> methods = new List<Method>();
            foreach (Method m in Methods)
            {
                List<Parameter> parameters = new List<Parameter>();
                foreach (Parameter p in m.Parameters)
                    parameters.Add(new Parameter(p.Location, p.Name, p.Span, ResolveType(p.Type, symbol.TemplateParameters)));

                methods.Add(new Method(m.Location, m.Name, m.Span, m.Modifier, ResolveType(m.Type, symbol.TemplateParameters), parameters.ToArray()));
            }

            return new SymbolTable(
                Location,
                Name,
                Span,
                Attributes,
                Modifier,
                TableType,
                VariableScopes,
                fields.ToArray(),
                constructors.ToArray(),
                properties.ToArray(),
                methods.ToArray(),
                Extends,
                Interfaces,
                InnerClasses);
        }

        /// <summary>
        /// Resolves the typeName using the typeParameters.
        /// </summary>
        /// <param name="typeName">The type name to resolve.</param>
        /// <param name="typeParameters">The parameters to use.</param>
        /// <returns>The resolved type name.</returns>
        private string ResolveType(string typeName, string[] typeParameters)
        {
            if (String.IsNullOrWhiteSpace(typeName) || typeName[0] != 'T')
                return typeName;

            int index = -1;
            if (!int.TryParse(typeName.Substring(1), out index))
                return typeName;

            index = index - 1;
            if (index < 0 || index >= typeParameters.Length)
                return typeName;

            return typeParameters[index];
        }

        /// <summary>
        /// Check to see if this symbol table has the given attribute.
        /// </summary>
        /// <param name="name">The name of the attribute to check for.</param>
        /// <returns>true if the attribute is present, false if it isn't.</returns>
        public bool HasAttribute(string name)
        {
            foreach (string a in Attributes)
                if (String.Compare(a, name, true) == 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Get Fields that have the given modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to filter by.</param>
        /// <returns>Fields that have the given modifier.</returns>
        public Field[] GetFieldsWithModifiers(SymbolModifier modifiers)
        {
            return Fields.Where(m => m.Modifier.HasFlag(modifiers)).ToArray();
        }

        /// <summary>
        /// Get Constructors that have the given modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to filter by.</param>
        /// <returns>Constructors that have the given modifier.</returns>
        public Constructor[] GetConstructorsWithModifiers(SymbolModifier modifiers)
        {
            return Constructors.Where(m => m.Modifier.HasFlag(modifiers)).ToArray();
        }

        /// <summary>
        /// Get Properties that have the given modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to filter by.</param>
        /// <returns>Properties that have the given modifier.</returns>
        public Property[] GetPropertiesWithModifiers(SymbolModifier modifiers)
        {
            return Properties.Where(m => m.Modifier.HasFlag(modifiers)).ToArray();
        }

        /// <summary>
        /// Get Methods that have the given modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to filter by.</param>
        /// <returns>Methods that have the given modifier.</returns>
        public Method[] GetMethodsWithModifiers(SymbolModifier modifiers)
        {
            return Methods.Where(m => m.Modifier.HasFlag(modifiers)).ToArray();
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Read in this object from the xml stream.
        /// </summary>
        /// <param name="reader">The xml stream to read from.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            string tableType = reader["tableType"] as string;
            if (tableType != null)
                TableType = (SymbolTableType)Enum.Parse(typeof(SymbolTableType), tableType);
            else
                TableType = SymbolTableType.Class;

            Extends = reader["extends"] as string;

            List<Field> fields = new List<Field>();
            List<Constructor> constructors = new List<Constructor>();
            List<Property> properties = new List<Property>();
            List<Method> methods = new List<Method>();
            List<string> interfaces = new List<string>();
            List<SymbolTable> innerClasses = new List<SymbolTable>();
            List<string> attributes = new List<string>();

            if (!reader.IsEmptyElement)
            {
                reader.Read();

                while (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "fields":
                            reader.Read();
                            while (reader.IsStartElement("field"))
                            {
                                Field f = new Field();
                                f.ReadXml(reader);
                                fields.Add(f);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "constructors":
                            reader.Read();
                            while (reader.IsStartElement("constructor"))
                            {
                                Constructor c = new Constructor();
                                c.ReadXml(reader);
                                constructors.Add(c);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "properties":
                            reader.Read();
                            while (reader.IsStartElement("property"))
                            {
                                Property p = new Property();
                                p.ReadXml(reader);
                                properties.Add(p);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "methods":
                            reader.Read();
                            while (reader.IsStartElement("method"))
                            {
                                Method m = new Method();
                                m.ReadXml(reader);
                                methods.Add(m);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "interfaces":
                            reader.Read();
                            while (reader.IsStartElement("interface"))
                            {
                                interfaces.Add(reader["name"]);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "attributes":
                            reader.Read();
                            while (reader.IsStartElement("attribute"))
                            {
                                attributes.Add(reader["name"]);
                                reader.Read();
                            }
                            reader.Read();
                            break;

                        case "innerClasses":
                            reader.Read();
                            while (reader.IsStartElement("class"))
                            {
                                SymbolTable st = new SymbolTable();
                                st.ReadXml(reader);
                                innerClasses.Add(st);
                            }
                            reader.Read();
                            break;

                        default:
                            reader.Read();
                            break;
                    }
                }
            }

            reader.Read();

            Fields = fields.ToArray();
            Constructors = constructors.ToArray();
            Properties = properties.ToArray();
            Methods = methods.ToArray();
            Interfaces = interfaces.ToArray();
            InnerClasses = innerClasses.ToArray();
            Attributes = attributes.ToArray();

            Id = Type.ToLower();
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("tableType", TableType.ToString());
            if (!String.IsNullOrWhiteSpace(Extends))
                writer.WriteAttributeString("extends", Extends);

            if (Fields.Length > 0)
            {
                writer.WriteStartElement("fields");
                foreach (Field f in Fields)
                {
                    writer.WriteStartElement("field");
                    f.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Constructors.Length > 0)
            {
                writer.WriteStartElement("constructors");
                foreach (Constructor c in Constructors)
                {
                    writer.WriteStartElement("constructor");
                    c.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Properties.Length > 0)
            {
                writer.WriteStartElement("properties");
                foreach (Property p in Properties)
                {
                    writer.WriteStartElement("property");
                    p.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Methods.Length > 0)
            {
                writer.WriteStartElement("methods");
                foreach (Method m in Methods)
                {
                    writer.WriteStartElement("method");
                    m.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Interfaces.Length > 0)
            {
                writer.WriteStartElement("interfaces");
                foreach (string i in Interfaces)
                {
                    writer.WriteStartElement("interface");
                    writer.WriteAttributeString("name", i);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Attributes.Length > 0)
            {
                writer.WriteStartElement("attributes");
                foreach (string a in Attributes)
                {
                    writer.WriteStartElement("attribute");
                    writer.WriteAttributeString("name", a);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (InnerClasses.Length > 0)
            {
                writer.WriteStartElement("innerClasses");
                foreach (SymbolTable st in InnerClasses)
                {
                    writer.WriteStartElement("class");
                    st.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
