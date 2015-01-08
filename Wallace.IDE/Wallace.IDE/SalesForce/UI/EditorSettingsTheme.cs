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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// A theme that can be applied to the editor.
    /// </summary>
    public class EditorSettingsTheme
    {
        #region Fields

        /// <summary>
        /// Supports the Symbols property.
        /// </summary>
        private List<EditorSymbolSettings> _symbols;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        public EditorSettingsTheme(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or whitespace", "name");

            Name = name;
            _symbols = new List<EditorSymbolSettings>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the theme.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The font to use.
        /// </summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// The foreground color.
        /// </summary>
        public Color Foreground { get; set; }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// The selection foreground.
        /// </summary>
        public Color? SelectionForeground { get; set; }

        /// <summary>
        /// The selection background.
        /// </summary>
        public Color? SelectionBackground { get; set; }

        /// <summary>
        /// The find result background.
        /// </summary>
        public Color FindResultBackground { get; set; }

        /// <summary>
        /// The symbols settings.
        /// </summary>
        public IEnumerable<EditorSymbolSettings> Symbols
        {
            get { return _symbols; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a symbol to the Symbols collection.
        /// </summary>
        /// <param name="name">EditorSymbolSettings.Name</param>
        /// <param name="foreground">EditorSymbolSettings.Foreground</param>
        /// <param name="background">EditorSymbolSettings.Background</param>
        /// <param name="isBold">EditorSymbolSettings.IsBold</param>
        /// <param name="isItalic">EditorSymbolSettings.IsItalic</param>
        public void AddSymbol(string name, Color? foreground, Color? background, bool isBold, bool isItalic)
        {
            EditorSymbolSettings ess = new EditorSymbolSettings(name);
            ess.Foreground = foreground;
            ess.Background = background;
            ess.IsBold = isBold;
            ess.IsItalic = isItalic;
            _symbols.Add(ess);
        }

        /// <summary>
        /// Add a symbol to the Symbols collection.
        /// </summary>
        /// <param name="name">EditorSymbolSettings.Name</param>
        /// <param name="foreground">EditorSymbolSettings.Foreground</param>
        /// <param name="background">EditorSymbolSettings.Background</param>
        /// <param name="isBold">EditorSymbolSettings.IsBold</param>
        /// <param name="isItalic">EditorSymbolSettings.IsItalic</param>
        public void AddSymbol(string name, string foreground, string background, bool isBold, bool isItalic)
        {
            EditorSymbolSettings ess = new EditorSymbolSettings(name);
            if (foreground != null)
                ess.Foreground = (Color)ColorConverter.ConvertFromString(foreground);
            if (background != null)
                ess.Background = (Color)ColorConverter.ConvertFromString(background);
            ess.IsBold = isBold;
            ess.IsItalic = isItalic;
            _symbols.Add(ess);
        }

        /// <summary>
        /// Returns the Name property.
        /// </summary>
        /// <returns>The Name property.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
