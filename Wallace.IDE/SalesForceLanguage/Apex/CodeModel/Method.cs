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
using System.Text;
using System.Xml;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A method symbol.
    /// </summary>
    public class Method : TypedSymbol
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public Method()
        {
            Parameters = new Parameter[0];
            Signature = String.Empty;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="modifier">Modifier.</param>
        /// <param name="type">Type.</param>
        /// <param name="parameters">Parameters.</param>
        public Method(
            TextPosition location, 
            string name, 
            TextSpan span,
            SymbolModifier modifier, 
            string type,
            Parameter[] parameters)
            : base(location, name, span, modifier, type)
        {
            Parameters = parameters ?? new Parameter[0];
            BuildSignature();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parameters that belong to this symbol.
        /// </summary>
        public Parameter[] Parameters { get; private set; }

        /// <summary>
        /// The method signature.
        /// </summary>
        public string Signature { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the signature for this method.
        /// </summary>
        private void BuildSignature()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}(", Id);

            for (int i = 0; i < Parameters.Length; i++)
            {
                string methodType = Parameters[i].Type ?? String.Empty;
                methodType = methodType.ToLower();
                if (!methodType.Contains("."))
                    methodType = String.Format("system.{0}", methodType);

                sb.AppendFormat("{0},", methodType);
            }

            if (Parameters.Length > 0)
                sb.Length--;

            sb.Append(")");

            Signature = sb.ToString();
        }

        /// <summary>
        /// A string representation that includes the return type.
        /// </summary>
        /// <returns>A string representation that includes the return type.</returns>
        public string ToStringWithReturnType()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} ", Type);
            sb.AppendFormat("{0}(", Name);
            foreach (Parameter p in Parameters)
                sb.AppendFormat("{0} {1}, ", p.FullType, p.Name);
            if (Parameters.Length > 0)
                sb.Length = sb.Length - 2;
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Return a string that represents this method.
        /// </summary>
        /// <returns>A string that represents this method.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}(", Name);
            foreach (Parameter p in Parameters)
                sb.AppendFormat("{0} {1}, ", p.FullType, p.Name);
            if (Parameters.Length > 0)
                sb.Length = sb.Length - 2;
            sb.Append(")");

            return sb.ToString();
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

            BuildSignature();
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
