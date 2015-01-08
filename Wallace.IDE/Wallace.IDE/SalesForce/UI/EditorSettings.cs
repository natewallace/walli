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
        /// The apex settings that serve the entire application.
        /// </summary>
        public static EditorSettings ApexSettings { get; private set; }

        /// <summary>
        /// The SOQL settings that serve the entire application.
        /// </summary>
        public static EditorSettings SOQLSettings { get; private set; }

        /// <summary>
        /// The visual force settings that serve the entire application.
        /// </summary>
        public static EditorSettings VisualForceSettings { get; private set; }

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
            // apex
            StreamResourceInfo highlight = Application.GetResourceStream(new Uri("Resources/Apex.xshd", UriKind.Relative));
            ApexSettings = new EditorSettings("EditorSettingsApex", highlight.Stream);
            List<EditorSettingsTheme> themes = new List<EditorSettingsTheme>();

            EditorSettingsTheme eTheme = new EditorSettingsTheme("Eclipse");
            eTheme.FontFamily = new FontFamily("Consolas");
            eTheme.Foreground = Colors.Black;
            eTheme.Background = Colors.White;
            eTheme.SelectionForeground = null;
            eTheme.SelectionBackground = Colors.LightBlue;
            eTheme.FindResultBackground = Colors.DarkOrange;
            eTheme.AddSymbol("Alert", Colors.Red, null, true, false);
            eTheme.AddSymbol("ApexKeyword", "#7F0055", null, false, false);
            eTheme.AddSymbol("Comment", "#827513", null, false, false);
            eTheme.AddSymbol("Delimiter", Colors.Black, null, false, false);
            eTheme.AddSymbol("DocComment", "#827513", null, false, false);
            eTheme.AddSymbol("DocCommentText", "#827513", null, false, false);
            eTheme.AddSymbol("Region", "#7F0055", null, false, false);
            eTheme.AddSymbol("RegionName", Colors.Black, null, false, false);
            eTheme.AddSymbol("SOQLKeyword", Colors.Purple, null, false, false);
            eTheme.AddSymbol("String", Colors.Green, null, false, false);
            eTheme.AddSymbol("Type", Colors.DarkSlateBlue, null, false, false);
            eTheme.AddSymbol("Warning", "#EEE0E000", null, true, false);
            themes.Add(eTheme);

            EditorSettingsTheme mTheme = new EditorSettingsTheme("Monokai");
            mTheme.FontFamily = new FontFamily("Consolas");
            mTheme.Foreground = (Color)ColorConverter.ConvertFromString("#F8F8F2");
            mTheme.Background = (Color)ColorConverter.ConvertFromString("#272822");
            mTheme.SelectionForeground = null;
            mTheme.SelectionBackground = (Color)ColorConverter.ConvertFromString("#49483E");
            mTheme.FindResultBackground = (Color)ColorConverter.ConvertFromString("#FFE792");
            mTheme.AddSymbol("Alert", Colors.Red, null, true, false);
            mTheme.AddSymbol("ApexKeyword", "#F92672", null, false, false);
            mTheme.AddSymbol("Comment", "#75715E", null, false, false);
            mTheme.AddSymbol("Delimiter", "#F8F8F2", null, false, false);
            mTheme.AddSymbol("DocComment", "#75715E", null, false, false);
            mTheme.AddSymbol("DocCommentText", "#75715E", null, false, false);
            mTheme.AddSymbol("Region", "#F92672", null, false, false);
            mTheme.AddSymbol("RegionName", "#F8F8F2", null, false, false);
            mTheme.AddSymbol("SOQLKeyword", Colors.Gold, null, false, false);
            mTheme.AddSymbol("String", "#E6DB74", null, false, false);
            mTheme.AddSymbol("Type", "#A6E22E", null, false, false);
            mTheme.AddSymbol("Warning", (Color)ColorConverter.ConvertFromString("#EEE0E000"), null, true, false);
            themes.Add(mTheme);

            EditorSettingsTheme vsTheme = new EditorSettingsTheme("Visual Studio");
            vsTheme.FontFamily = new FontFamily("Consolas");
            vsTheme.Foreground = Colors.Black;
            vsTheme.Background = Colors.White;
            vsTheme.SelectionForeground = null;
            vsTheme.SelectionBackground = Colors.LightBlue;
            vsTheme.FindResultBackground = Colors.DarkOrange;
            vsTheme.AddSymbol("Alert", Colors.Red, null, true, false);
            vsTheme.AddSymbol("ApexKeyword", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("Comment", Colors.Green, null, false, false);
            vsTheme.AddSymbol("Delimiter", Colors.Black, null, false, false);
            vsTheme.AddSymbol("DocComment", Colors.Gray, null, false, false);
            vsTheme.AddSymbol("DocCommentText", Colors.Green, null, false, true);
            vsTheme.AddSymbol("Region", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("RegionName", Colors.Black, null, false, false);
            vsTheme.AddSymbol("SOQLKeyword", Colors.Purple, null, false, false);
            vsTheme.AddSymbol("String", Colors.DarkRed, null, false, false);
            vsTheme.AddSymbol("Type", Colors.Teal, null, false, false);
            vsTheme.AddSymbol("Warning", "#EEE0E000", null, true, false);
            themes.Add(vsTheme);

            ApexSettings.Themes = themes;

            // visual force
            highlight = Application.GetResourceStream(new Uri("Resources/VisualForce.xshd", UriKind.Relative));
            VisualForceSettings = new EditorSettings("EditorSettingsVisualForce", highlight.Stream);
            themes = new List<EditorSettingsTheme>();

            eTheme = new EditorSettingsTheme("Eclipse");
            eTheme.FontFamily = new FontFamily("Consolas");
            eTheme.Foreground = Colors.Black;
            eTheme.Background = Colors.White;
            eTheme.SelectionForeground = null;
            eTheme.SelectionBackground = Colors.LightBlue;
            eTheme.FindResultBackground = Colors.DarkOrange;
            eTheme.AddSymbol("Assignment", Colors.Black, null, false, false);
            eTheme.AddSymbol("Attributes", "#B04391", null, false, false);
            eTheme.AddSymbol("Comment", "#6085D5", null, false, false);
            eTheme.AddSymbol("Digits", Colors.Black, null, false, false);
            eTheme.AddSymbol("Entities", "#267F89", null, false, false);
            eTheme.AddSymbol("EntityReference", "#267F89", null, false, false);
            eTheme.AddSymbol("HtmlTag", "#267F89", null, false, false);
            eTheme.AddSymbol("JavaScriptTag", "#267F89", null, false, false);
            eTheme.AddSymbol("Slash", "#267F89", null, false, false);
            eTheme.AddSymbol("String", "#471BFF", null, false, false);
            eTheme.AddSymbol("Tags", "#267F89", null, false, false);
            themes.Add(eTheme);

            mTheme = new EditorSettingsTheme("Monokai");
            mTheme.FontFamily = new FontFamily("Consolas");
            mTheme.Foreground = (Color)ColorConverter.ConvertFromString("#F8F8F2");
            mTheme.Background = (Color)ColorConverter.ConvertFromString("#272822");
            mTheme.SelectionForeground = null;
            mTheme.SelectionBackground = (Color)ColorConverter.ConvertFromString("#49483E");
            mTheme.FindResultBackground = (Color)ColorConverter.ConvertFromString("#FFE792");
            mTheme.AddSymbol("Assignment", "#F92672", null, false, false);
            mTheme.AddSymbol("Attributes", "#A6E22E", null, false, false);
            mTheme.AddSymbol("Comment", "#75715E", null, false, false);
            mTheme.AddSymbol("Digits", "#E6DB74", null, false, false);
            mTheme.AddSymbol("Entities", "#2D96A2", null, false, false);
            mTheme.AddSymbol("EntityReference", "#2D96A2", null, false, false);
            mTheme.AddSymbol("HtmlTag", "#F8F8F2", null, false, false);
            mTheme.AddSymbol("JavaScriptTag", "#F92672", null, false, false);
            mTheme.AddSymbol("Slash", "#F8F8F2", null, false, false);
            mTheme.AddSymbol("String", "#E6DB74", null, false, false);
            mTheme.AddSymbol("Tags", "#F92672", null, false, false);
            themes.Add(mTheme);

            vsTheme = new EditorSettingsTheme("Visual Studio");
            vsTheme.FontFamily = new FontFamily("Consolas");
            vsTheme.Foreground = Colors.Black;
            vsTheme.Background = Colors.White;
            vsTheme.SelectionForeground = null;
            vsTheme.SelectionBackground = Colors.LightBlue;
            vsTheme.FindResultBackground = Colors.DarkOrange;
            vsTheme.AddSymbol("Assignment", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("Attributes", Colors.Red, null, false, false);
            vsTheme.AddSymbol("Comment", Colors.Green, null, false, false);
            vsTheme.AddSymbol("Digits", Colors.DarkBlue, null, false, false);
            vsTheme.AddSymbol("Entities", Colors.Red, null, false, false);
            vsTheme.AddSymbol("EntityReference", Colors.Red, null, false, false);
            vsTheme.AddSymbol("HtmlTag", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("JavaScriptTag", Colors.Black, null, false, false);
            vsTheme.AddSymbol("Slash", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("String", Colors.Blue, null, false, false);
            vsTheme.AddSymbol("Tags", Colors.DarkRed, null, false, false);
            themes.Add(vsTheme);

            VisualForceSettings.Themes = themes;

            // SOQL
            highlight = Application.GetResourceStream(new Uri("Resources/SOQL.xshd", UriKind.Relative));
            SOQLSettings = new EditorSettings("EditorSettingsSOQL", highlight.Stream);
            themes = new List<EditorSettingsTheme>();

            eTheme = new EditorSettingsTheme("Eclipse");
            eTheme.FontFamily = new FontFamily("Consolas");
            eTheme.Foreground = Colors.Black;
            eTheme.Background = Colors.White;
            eTheme.SelectionForeground = null;
            eTheme.SelectionBackground = Colors.LightBlue;
            eTheme.FindResultBackground = Colors.DarkOrange;
            eTheme.AddSymbol("SOQLKeyword", Colors.Purple, null, false, false);
            eTheme.AddSymbol("String", "#3A14FF", null, false, false);
            themes.Add(eTheme);

            mTheme = new EditorSettingsTheme("Monokai");
            mTheme.FontFamily = new FontFamily("Consolas");
            mTheme.Foreground = (Color)ColorConverter.ConvertFromString("#F8F8F2");
            mTheme.Background = (Color)ColorConverter.ConvertFromString("#272822");
            mTheme.SelectionForeground = null;
            mTheme.SelectionBackground = (Color)ColorConverter.ConvertFromString("#49483E");
            mTheme.FindResultBackground = (Color)ColorConverter.ConvertFromString("#FFE792");
            mTheme.AddSymbol("SOQLKeyword", Colors.Gold, null, false, false);
            mTheme.AddSymbol("String", "#E6DB74", null, false, false);
            themes.Add(mTheme);

            vsTheme = new EditorSettingsTheme("Visual Studio");
            vsTheme.FontFamily = new FontFamily("Consolas");
            vsTheme.Foreground = Colors.Black;
            vsTheme.Background = Colors.White;
            vsTheme.SelectionForeground = null;
            vsTheme.SelectionBackground = Colors.LightBlue;
            vsTheme.FindResultBackground = Colors.DarkOrange;
            vsTheme.AddSymbol("SOQLKeyword", Colors.Purple, null, false, false);
            vsTheme.AddSymbol("String", Colors.DarkRed, null, false, false);
            themes.Add(vsTheme);

            SOQLSettings.Themes = themes;
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

            FontFamily = new FontFamily("Consolas");
            FontSizeInPoints = 10;
            Foreground = Colors.Black;
            Background = Colors.White;
            SelectionForeground = null;
            SelectionBackground = Colors.LightBlue;
            FindResultBackground = Colors.DarkOrange;

            if (!Load())
            {
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

            Themes = new List<EditorSettingsTheme>();
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
        /// The foreground color.
        /// </summary>
        public Color Foreground { get; set; }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// The background color for selected text.
        /// </summary>
        public Color? SelectionForeground { get; set; }

        /// <summary>
        /// The foreground color for selected text.
        /// </summary>
        public Color? SelectionBackground { get; set; }

        /// <summary>
        /// The find result background color.
        /// </summary>
        public Color FindResultBackground { get; set; }

        /// <summary>
        /// The symbols settings.
        /// </summary>
        public IEnumerable<EditorSymbolSettings> Symbols { get; private set; }

        /// <summary>
        /// The themes that are supported.
        /// </summary>
        public IEnumerable<EditorSettingsTheme> Themes { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Update symbols with the given values.
        /// </summary>
        /// <param name="symbols">The symbols to update with.</param>
        public void UpdateSymbols(IEnumerable<EditorSymbolSettings> symbols)
        {
            if (symbols == null)
                return;

            foreach (EditorSymbolSettings essSource in symbols)
            {
                foreach (EditorSymbolSettings essTarget in Symbols)
                {
                    if (essSource.Name == essTarget.Name)
                    {
                        essTarget.Foreground = essSource.Foreground;
                        essTarget.Background = essSource.Background;
                        essTarget.IsBold = essSource.IsBold;
                        essTarget.IsItalic = essSource.IsItalic;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new header using the header text which may contain variables.
        /// </summary>
        /// <returns>A new header with the variables replaced with their values.</returns>
        public string CreateHeader()
        {
            if (Header != null)
            {
                StringBuilder sb = new StringBuilder(Header);
                sb.Replace("{!datetime}", DateTime.Now.ToString());
                sb.Replace("{!date}", DateTime.Now.ToShortDateString());
                sb.Replace("{!time}", DateTime.Now.ToShortTimeString());
                if (App.Instance.SalesForceApp.CurrentProject != null)
                    sb.Replace("{!author}", App.Instance.SalesForceApp.CurrentProject.Client.GetUserName());

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

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
                                        if (xml["foreground"] != null)
                                            Foreground = (Color)ColorConverter.ConvertFromString(xml["foreground"]);
                                        if (xml["background"] != null)
                                            Background = (Color)ColorConverter.ConvertFromString(xml["background"]);
                                        if (xml["selectionForeground"] != null)
                                            SelectionForeground = (Color)ColorConverter.ConvertFromString(xml["selectionForeground"]);
                                        else
                                            SelectionForeground = null;
                                        if (xml["selectionBackground"] != null)
                                            SelectionBackground = (Color)ColorConverter.ConvertFromString(xml["selectionBackground"]);
                                        else
                                            SelectionBackground = null;
                                        if (xml["findResultBackground"] != null)
                                            FindResultBackground = (Color)ColorConverter.ConvertFromString(xml["findResultBackground"]);
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
                    xml.WriteAttributeString("foreground", Foreground.ToString());
                    xml.WriteAttributeString("background", Background.ToString());
                    if (SelectionForeground.HasValue)
                        xml.WriteAttributeString("selectionForeground", SelectionForeground.Value.ToString());
                    if (SelectionBackground.HasValue)
                        xml.WriteAttributeString("selectionBackground", SelectionBackground.Value.ToString());
                    xml.WriteAttributeString("findResultBackground", FindResultBackground.ToString());
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
