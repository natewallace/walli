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
using System.Xml;
using System.Xml.Serialization;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A generic symbol.
    /// </summary>
    public class Symbol : IComparable, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public Symbol()
        {
            Name = String.Empty;
            Id = String.Empty;
            Location = new TextPosition(0, 0);
            Span = new TextSpan(Location, Location);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        public Symbol(TextPosition location, string name, TextSpan span)
        {
            Location = location;
            Name = name ?? String.Empty;
            Id = Name.ToLower();
            Span = span ?? new TextSpan(Location, new TextPosition(Location.Line, Location.Column + Name.Length));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name in all lowercase.
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// The location of the symbol.
        /// </summary>
        public TextPosition Location { get; private set; }

        /// <summary>
        /// The name for the symbol.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The entire span for the symbol.
        /// </summary>
        public TextSpan Span { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Check to see if the given position is contained within this symbol.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>true if the given position is contained within this symbol.</returns>
        public bool Contains(TextPosition position)
        {
            return Span.Contains(position);
        }

        /// <summary>
        /// Returns the Name property.
        /// </summary>
        /// <returns>The Name property.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Apply an offset to the line positions.
        /// </summary>
        /// <param name="offset">The offset to apply to the line positions.</param>
        public virtual void ApplyLineOffset(int offset)
        {
            Location = Location.CreateLineOffset(offset);
            Span = Span.CreateLineOffset(offset);
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: 
        /// Less than zero - This instance precedes obj in the sort order. 
        /// Zero - This instance occurs in the same position in the sort order as obj. 
        /// Greater than zero - This instance follows obj in the sort order.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            Symbol other = obj as Symbol;
            if (other == null)
                return -1;

            int result = this.GetType().Name.CompareTo(other.GetType().Name);
            if (result != 0)
                return result;

            return this.Name.CompareTo(other.Name);
        }

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
        public virtual void ReadXml(XmlReader reader)
        {
            Name = reader["name"] ?? String.Empty;
            Id = Name.ToLower();
        }

        /// <summary>
        /// Write this object out to an xml stream.
        /// </summary>
        /// <param name="writer">The xml stream to write to.</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name);
        }

        #endregion
    }
}
