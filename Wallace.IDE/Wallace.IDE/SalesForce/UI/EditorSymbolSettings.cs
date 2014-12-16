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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Editor settings for a symbol.
    /// </summary>
    public class EditorSymbolSettings : IXmlSerializable
    {
        #region Fields

        private Color? _originalForeground;

        private Color? _originalBackground;

        private FontWeight? _originalWeight;

        private FontStyle? _originalStyle;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor which is only to be used by the xml serialzier.
        /// </summary>
        public EditorSymbolSettings()
        {
            Name = null;
            Foreground = null;
            Background = null;
            Weight = null;
            Style = null;

            _originalForeground = null;
            _originalBackground = null;
            _originalWeight = null;
            _originalStyle = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        public EditorSymbolSettings(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or whitespace.", "name");

            Name = name;
            Foreground = null;
            Background = null;
            Weight = null;
            Style = null;

            _originalForeground = null;
            _originalBackground = null;
            _originalWeight = null;
            _originalStyle = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the symbol.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The foreground color.
        /// </summary>
        public Color? Foreground { get; set; }

        /// <summary>
        /// The string representation of the Foreground.
        /// </summary>
        public string ForegroundAsString
        {
            get
            {
                return Foreground.HasValue ? Foreground.Value.ToString() : "(none)";
            }
        }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color? Background { get; set; }

        /// <summary>
        /// The string representation of the Background.
        /// </summary>
        public string BackgroundAsString
        {
            get
            {
                return Background.HasValue ? Background.Value.ToString() : "(none)";
            }
        }

        /// <summary>
        /// The weight.
        /// </summary>
        public FontWeight? Weight { get; set; }

        /// <summary>
        /// Get/Set if this symbol is bold.
        /// </summary>
        public bool IsBold
        {
            get
            {
                return (Weight.HasValue && Weight.Value == FontWeights.Bold);
            }
            set
            {
                if (value)
                    Weight = FontWeights.Bold;
                else
                    Weight = null;
            }
        }

        /// <summary>
        /// The style.
        /// </summary>
        public FontStyle? Style { get; set; }

        /// <summary>
        /// Get/Set if this symbol is italic.
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return (Style.HasValue && Style.Value == FontStyles.Italic);
            }
            set
            {
                if (value)
                    Style = FontStyles.Italic;
                else
                    Style = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove any changes that were made since this symbol was loaded.
        /// </summary>
        public void Reset()
        {
            Foreground = _originalForeground;
            Background = _originalBackground;
            Weight = _originalWeight;
            Style = _originalStyle;
        }

        /// <summary>
        /// Commit the current values.
        /// </summary>
        public void Commit()
        {
            _originalForeground = Foreground;
            _originalBackground = Background;
            _originalWeight = Weight;
            _originalStyle = Style;
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
        /// Read in the values for this object.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            Name = reader["name"];
            if (String.IsNullOrWhiteSpace(Name))
                throw new Exception("Could not read in a valid Name.");

            string foregroundText = reader["foreground"];
            if (foregroundText != null)
                Foreground = (Color)ColorConverter.ConvertFromString(foregroundText);

            string backgroundText = reader["background"];
            if (backgroundText != null)
                Background = (Color)ColorConverter.ConvertFromString(backgroundText);

            IsBold = (reader["weight"] == "bold");
            IsItalic = (reader["style"] == "italic");

            reader.Read();
        }

        /// <summary>
        /// Write out the values for this object.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name);
            if (Foreground.HasValue)
                writer.WriteAttributeString("foreground", Foreground.Value.ToString());
            if (Background.HasValue)
                writer.WriteAttributeString("background", Background.Value.ToString());
            if (IsBold)
                writer.WriteAttributeString("weight", "bold");
            if (IsItalic)
                writer.WriteAttributeString("style", "italic");
        }

        #endregion
    }
}
