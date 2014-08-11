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
using System.Xml;
using System.Xml.Serialization;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A parameter.
    /// </summary>
    public class Parameter : IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public Parameter()
        {
            Type = String.Empty;
            Name = String.Empty;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="name">Name.</param>
        public Parameter(string type, string name)
        {
            Name = name ?? String.Empty;
            Type = type ?? String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of the property.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Not used.
        /// </summary>
        /// <returns>null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read in this object from the xml stream.
        /// </summary>
        /// <param name="reader">The xml stream to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            Type = reader["type"];
            Name = reader["name"];
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("name", Name);
        }

        #endregion
    }
}
