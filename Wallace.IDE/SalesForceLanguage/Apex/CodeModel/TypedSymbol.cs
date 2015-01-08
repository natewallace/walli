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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A symbol that has a type.
    /// </summary>
    public class TypedSymbol : ModifiedSymbol
    {
        #region Fields

        /// <summary>
        /// Supports the Type property.
        /// </summary>
        private string _type;

        #endregion

        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public TypedSymbol()
        {
            Type = String.Empty;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="modifier">Modifier.</param>
        /// <param name="type">Type.</param>
        public TypedSymbol(TextPosition location, string name, TextSpan span, SymbolModifier modifier, string type)
            : base(location, name, span, modifier)
        {
            Type = type ?? String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The full type name value including template parameters.
        /// </summary>
        public string FullType { get; set; }

        /// <summary>
        /// The type for the symbol.
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                FullType = value;

                if (String.IsNullOrWhiteSpace(value))
                {
                    _type = String.Empty;
                    TemplateParameters = new string[0];
                }
                else if (value.EndsWith("]"))
                {
                    int index = value.LastIndexOf('[');
                    if (index == -1)
                        throw new Exception("Invalid type: " + value);

                    _type = "List";
                    TemplateParameters = new string[] { value.Substring(0, index) };
                }
                else
                {
                    int index = value.IndexOf('<');
                    if (index == -1)
                    {
                        _type = value;
                        TemplateParameters = new string[0];
                    }
                    else
                    {
                        _type = value.Substring(0, index);
                        List<string> templateParameters = new List<string>();

                        int openDelimiter = 0;
                        StringBuilder sb = new StringBuilder();
                        for (int i = index + 1; i < value.Length - 1; i++)
                        {
                            switch (value[i])
                            {
                                case '<':
                                    sb.Append(value[i]);
                                    openDelimiter++;
                                    break;

                                case '>':
                                    sb.Append(value[i]);
                                    openDelimiter--;
                                    break;

                                case ',':
                                    if (openDelimiter == 0)
                                    {
                                        templateParameters.Add(sb.ToString());
                                        sb.Length = 0;
                                    }
                                    else
                                    {
                                        sb.Append(value[i]);
                                    }
                                    break;

                                default:
                                    sb.Append(value[i]);
                                    break;
                            }
                        }

                        if (sb.Length > 0)
                            templateParameters.Add(sb.ToString());

                        TemplateParameters = templateParameters.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Template parameters if there are any.
        /// </summary>
        public string[] TemplateParameters { get; private set; }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Read in this object from the xml stream.
        /// </summary>
        /// <param name="reader">The xml stream to read from.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Type = reader["type"];
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("type", Type);
        }

        #endregion
    }
}
