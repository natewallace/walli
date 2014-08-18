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
            TableType = tableType;
            VariableScopes = variableScopes ?? new VariableScope[0];
            Fields = fields ?? new Field[0];
            Constructors = constructors ?? new Constructor[0];
            Properties = properties ?? new Property[0];
            Methods = methods ?? new Method[0];
            Extends = extends;
            Interfaces = interfaces ?? new string[0];
            InnerClasses = innerClasses ?? new SymbolTable[0];
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

        #endregion

        #region Methods

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

                        case "innerClasses":
                            reader.Read();
                            while (reader.IsStartElement("class"))
                            {
                                SymbolTable st = new SymbolTable();
                                st.ReadXml(reader);
                                innerClasses.Add(st);
                                reader.Read();
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
