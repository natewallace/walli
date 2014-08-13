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
    /// A constructor symbol.
    /// </summary>
    public class Constructor : ModifiedSymbol
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public Constructor()
        {
            Parameters = new Parameter[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="modifier">Modifier.</param>
        /// <param name="parameters">Parameters.</param>
        public Constructor(TextPosition location, string name, TextSpan span, SymbolModifier modifier, Parameter[] parameters)
            : base(location, name, span, modifier)
        {
            Parameters = parameters ?? new Parameter[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parameters that belong to this symbol.
        /// </summary>
        public Parameter[] Parameters { get; private set; }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Read in this object from the xml stream.
        /// </summary>
        /// <param name="reader">The xml stream to read from.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            List<Parameter> parameters = new List<Parameter>();
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                if (reader.IsStartElement("parameters"))
                {
                    reader.Read();
                    while (reader.IsStartElement("parameter"))
                    {
                        Parameter p = new Parameter();
                        p.ReadXml(reader);
                        parameters.Add(p);
                        reader.Read();
                    }

                    reader.Read();
                }
            }

            Parameters = parameters.ToArray();
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (Parameters.Length > 0)
            {
                writer.WriteStartElement("parameters");

                foreach (Parameter p in Parameters)
                {
                    writer.WriteStartElement("parameter");
                    p.WriteXml(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
