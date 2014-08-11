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
using System.Xml;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A class or interface.
    /// </summary>
    public class SymbolTable : Symbol
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public SymbolTable()
        {
            Constructors = new Constructor[0];
            Properties = new VisibilitySymbol[0];
            Methods = new Method[0];
            Interfaces = new string[0];
            InnerClasses = new SymbolTable[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="constructors">Constructors</param>
        /// <param name="properties">Properties</param>
        /// <param name="methods">Methods</param>
        /// <param name="interfaces">Interfaces</param>
        /// <param name="innerClasses">InnerClasses</param>
        public SymbolTable(
            TextPosition location,
            string name,
            TextSpan span,
            Constructor[] constructors,
            VisibilitySymbol[] properties,
            Method[] methods,
            string[] interfaces,
            SymbolTable[] innerClasses)
            : base(location, name, span)
        {
            Constructors = constructors ?? new Constructor[0];
            Properties = properties ?? new VisibilitySymbol[0];
            Methods = methods ?? new Method[0];
            Interfaces = interfaces ?? new string[0];
            InnerClasses = innerClasses ?? new SymbolTable[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Constructors for the table.
        /// </summary>
        public Constructor[] Constructors { get; set; }

        /// <summary>
        /// Properties for the table.
        /// </summary>
        public VisibilitySymbol[] Properties { get; private set; }

        /// <summary>
        /// Methods for the table.
        /// </summary>
        public Method[] Methods { get; set; }

        /// <summary>
        /// Interfaces or the table.
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// Inner classes or interfaces for the table.
        /// </summary>
        public SymbolTable[] InnerClasses { get; set; }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Read in this object from the xml stream.
        /// </summary>
        /// <param name="reader">The xml stream to read from.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

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
                            break;
                    }
                }
            }

            Constructors = constructors.ToArray();
            Properties = properties.ToArray();
            Methods = methods.ToArray();
            Interfaces = interfaces.ToArray();
            InnerClasses = innerClasses.ToArray();
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

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
