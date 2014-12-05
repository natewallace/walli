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

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static EditorSettings()
        {
            StreamResourceInfo highlight = Application.GetResourceStream(new Uri("Resources/Apex.xshd", UriKind.Relative));
            FontFamily fontFamily = new FontFamily("Consolas");
            double fontSize = 10d * 96d / 72d;
            ApexSettings = new EditorSettings(highlight.Stream, fontFamily, fontSize);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="highlight">A stream that contains xshd formatted highlight instructions.</param>
        /// <param name="fontFamily">FontFamily.</param>
        /// <param name="fontSize">FontSize.</param>
        public EditorSettings(Stream highlight, FontFamily fontFamily, double fontSize)
        {
            if (highlight == null)
                throw new ArgumentNullException("highlight");
            if (fontFamily == null)
                throw new ArgumentNullException("fontFamily");
            if (fontSize <= 0)
                throw new ArgumentException("fontSize must be greater than zero.", "fontSize");

            using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(highlight))
                HighlightDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);

            FontFamily = fontFamily;
            FontSize = fontSize;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The highlighting definition.
        /// </summary>
        public IHighlightingDefinition HighlightDefinition { get; private set; }

        /// <summary>
        /// The font to use.
        /// </summary>
        public FontFamily FontFamily { get; private set; }

        /// <summary>
        /// The font size to use.
        /// </summary>
        public double FontSize { get; private set; }

        #endregion
    }
}
