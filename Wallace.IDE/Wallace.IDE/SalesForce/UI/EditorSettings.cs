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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Class that is used to manage all highlighting definitions.
    /// </summary>
    public class EditorSettings
    {
        #region Fields

        /// <summary>
        /// The apex settings that serves the entire application.
        /// </summary>
        public static EditorSettings ApexSettings { get; private set; }

        /// <summary>
        /// The name for the setting.
        /// </summary>
        private string _settingName;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static EditorSettings()
        {
            StreamResourceInfo highlight = Application.GetResourceStream(new Uri("Resources/Apex.xshd", UriKind.Relative));
            ApexSettings = new EditorSettings("EditorSettingsApex", highlight.Stream);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="highlight">A stream that contains xshd formatted highlight instructions.</param>
        public EditorSettings(string settingName, Stream highlight)
        {
            if (String.IsNullOrWhiteSpace(settingName))
                throw new ArgumentException("settingName is null or whitespace.", "settingName");
            if (highlight == null)
                throw new ArgumentNullException("highlight");

            _settingName = settingName;

            using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(highlight))
                HighlightDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);

            Symbols = new List<EditorSymbolSettings>();

            if (!Load())
            {
                FontFamily = new FontFamily("Consolas");
                FontSizeInPoints = 10;

                List<EditorSymbolSettings> symbols = new List<EditorSymbolSettings>();
                foreach (HighlightingColor hc in HighlightDefinition.NamedHighlightingColors)
                {
                    EditorSymbolSettings ess = new EditorSymbolSettings(hc.Name);
                    ess.Foreground = (hc.Foreground != null) ? hc.Foreground.GetColor(null) : null;
                    ess.Background = (hc.Background != null) ? hc.Background.GetColor(null) : null;
                    ess.Weight = hc.FontWeight;
                    ess.Style = hc.FontStyle;
                    ess.Commit();
                    symbols.Add(ess);
                }
                Symbols = symbols;
            }
            else
            {
                ApplySymbolUpdates();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The highlighting definition.
        /// </summary>
        public IHighlightingDefinition HighlightDefinition { get; private set; }

        /// <summary>
        /// A header that gets added to new files.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The font to use.
        /// </summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// The font size to use in pixels.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// The font size to use in points.
        /// </summary>
        public double FontSizeInPoints
        {
            get
            {
                return FontSize * 72d / 96d;
            }
            set
            {
                FontSize = value * 96d / 72d;
            }
        }

        /// <summary>
        /// The symbols settings.
        /// </summary>
        public IEnumerable<EditorSymbolSettings> Symbols { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reset any changes made to the symbols.
        /// </summary>
        public void ResetSymbols()
        {
            foreach (EditorSymbolSettings ess in Symbols)
                ess.Reset();

            ApplySymbolUpdates();
        }

        /// <summary>
        /// Apply changes made to symbols to the highlighting definition.
        /// </summary>
        private void ApplySymbolUpdates()
        {
            Type brushType = System.Reflection.Assembly.GetAssembly(typeof(HighlightingBrush)).GetType(
                            "ICSharpCode.AvalonEdit.Highlighting.SimpleHighlightingBrush");
            if (brushType == null)
                throw new Exception("Could not find type: ICSharpCode.AvalonEdit.Highlighting.SimpleHighlightingBrush");

            foreach (EditorSymbolSettings ess in Symbols)
            {
                foreach (HighlightingColor hc in HighlightDefinition.NamedHighlightingColors)
                {
                    if (ess.Name == hc.Name)
                    {
                        hc.Foreground = (ess.Foreground.HasValue) ?
                            Activator.CreateInstance(
                                brushType,
                                new object[] { ess.Foreground.Value }) as HighlightingBrush :
                            null;

                        hc.Background = (ess.Background.HasValue) ?
                            Activator.CreateInstance(
                                brushType,
                                new object[] { ess.Background.Value }) as HighlightingBrush :
                            null;

                        hc.FontWeight = ess.Weight;
                        hc.FontStyle = ess.Style;

                        ess.Commit();

                        break;
                    }
                }
            }     
        }

        /// <summary>
        /// Load settings from file.
        /// </summary>
        /// <returns>true if settings were loaded, false if they were not.</returns>
        private bool Load()
        {
            string text = Properties.Settings.Default[_settingName] as string;
            if (String.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            else
            {
                try
                {
                    using (StringReader reader = new StringReader(text))
                    {
                        using (XmlReader xml = XmlReader.Create(reader))
                        {
                            if (!xml.ReadToDescendant("settings"))
                                throw new Exception("settings element is missing.");

                            xml.Read();
                            while (xml.NodeType == XmlNodeType.Element)
                            {
                                switch (xml.LocalName)
                                {
                                    case "header":
                                        Header = xml.ReadElementContentAsString();
                                        break;

                                    case "font":
                                        if (xml["family"] != null)
                                            FontFamily = new FontFamily(xml["family"]);
                                        if (xml["size"] != null)
                                            FontSizeInPoints = double.Parse(xml["size"]);
                                        xml.Read();
                                        break;

                                    case "symbols":
                                        List<EditorSymbolSettings> symbols = new List<EditorSymbolSettings>();
                                        xml.Read();
                                        while (xml.NodeType == XmlNodeType.Element)
                                        {
                                            EditorSymbolSettings ess = new EditorSymbolSettings();
                                            ess.ReadXml(xml);
                                            symbols.Add(ess);
                                        }
                                        Symbols = symbols;
                                        xml.Read();
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (Symbols.Count() == 0)
                        return false;
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Save any changes made to the settings.
        /// </summary>
        public void Save()
        {
            // save to file
            StringBuilder text = new StringBuilder();
            using (StringWriter writer = new StringWriter(text))
            {
                using (XmlWriter xml = XmlWriter.Create(writer))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("settings");

                    xml.WriteStartElement("header");
                    xml.WriteString(Header);
                    xml.WriteEndElement();

                    xml.WriteStartElement("font");
                    xml.WriteAttributeString("family", FontFamily.Source);
                    xml.WriteAttributeString("size", FontSizeInPoints.ToString());
                    xml.WriteEndElement();

                    xml.WriteStartElement("symbols");
                    foreach (EditorSymbolSettings ess in Symbols)
                    {
                        xml.WriteStartElement("symbol");
                        ess.WriteXml(xml);
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();

                    xml.WriteEndElement();
                }
            }

            Properties.Settings.Default[_settingName] = text.ToString();
            Properties.Settings.Default.Save();

            // update highlight definition with symbol settings
            ApplySymbolUpdates();
        }

        #endregion
    }
}
