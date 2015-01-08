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

using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using SalesForceLanguage;
using SalesForceLanguage.Apex;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// This class is used to implement sytnax highlighting for apex documents.
    /// </summary>
    public class ApexDocumentColorizingTransformer : DocumentColorizingTransformer
    {
        #region Fields

        /// <summary>
        /// Used to mark errors.
        /// </summary>
        private static TextDecorationCollection ERROR_DECORATIONS;

        /// <summary>
        /// The brush to use for type symbol foreground.
        /// </summary>
        private Brush _typeForeground;

        /// <summary>
        /// The brush to use for type symbol background.
        /// </summary>
        private Brush _typeBackground;

        /// <summary>
        /// The typeface to use for type symbols.
        /// </summary>
        private Typeface _typeTypeface;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ApexDocumentColorizingTransformer()
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = Geometry.Parse("M 0,1 C 1,0 2,2 3,1");
            path.Stroke = Brushes.Red;
            path.StrokeThickness = 0.4;
            path.StrokeEndLineCap = PenLineCap.Square;
            path.StrokeStartLineCap = PenLineCap.Square;

            VisualBrush brush = new VisualBrush();
            brush.Visual = path;
            brush.Viewbox = new Rect(0, 0, 3, 2);
            brush.ViewboxUnits = BrushMappingMode.Absolute;
            brush.Viewport = new Rect(0, 0.8, 6, 4);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.TileMode = TileMode.Tile;

            TextDecoration decoration = new TextDecoration();
            decoration.Pen = new Pen(brush, 6);

            ERROR_DECORATIONS = new TextDecorationCollection();
            ERROR_DECORATIONS.Add(decoration);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parse result data to color for.
        /// </summary>
        public ParseResult ParseData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reset the symbol settings used to color symbols.
        /// </summary>
        public void ResetSymbolSettings()
        {
            _typeTypeface = null;

            foreach (EditorSymbolSettings ess in EditorSettings.ApexSettings.Symbols)
            {
                if (ess.Name == "Type")
                {
                    if (ess.Foreground.HasValue)
                        _typeForeground = new SolidColorBrush(ess.Foreground.Value);
                    else
                        _typeForeground = Brushes.Teal;

                    if (ess.Background.HasValue)
                        _typeBackground = new SolidColorBrush(ess.Background.Value);
                    else
                        _typeBackground = null;

                    if (ess.Weight.HasValue || ess.Style.HasValue)
                    {
                        FontWeight weight = FontWeights.Normal;
                        if (ess.Weight.HasValue)
                            weight = ess.Weight.Value;

                        FontStyle style = FontStyles.Normal;
                        if (ess.Style.HasValue)
                            style = ess.Style.Value;

                        _typeTypeface = new Typeface(
                            EditorSettings.ApexSettings.FontFamily,
                            style,
                            weight,
                            FontStretches.Normal);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Color the given line.
        /// </summary>
        /// <param name="line">The line to color.</param>
        protected override void ColorizeLine(DocumentLine line)
        {
            if (ParseData != null)
            {
                // type reference markings
                if (ParseData.TypeReferencesByLine.ContainsKey(line.LineNumber))
                {
                    foreach (TextSpan span in ParseData.TypeReferencesByLine[line.LineNumber])
                    {
                        int startOffset = line.Offset + span.StartPosition.Column - 1;
                        int endOffset = line.Offset + span.EndPosition.Column - 1;
                        if (startOffset == endOffset)
                            endOffset++;

                        if (endOffset > startOffset &&
                            startOffset >= line.Offset &&
                            startOffset <= line.EndOffset &&
                            endOffset >= line.Offset &&
                            endOffset <= line.EndOffset)
                            ChangeLinePart(
                                startOffset,
                                endOffset,
                                (element) =>
                                {
                                    if (_typeForeground != null)
                                        element.TextRunProperties.SetForegroundBrush(_typeForeground);
                                    if (_typeBackground != null)
                                        element.TextRunProperties.SetBackgroundBrush(_typeBackground);
                                    if (_typeTypeface != null)
                                        element.TextRunProperties.SetTypeface(_typeTypeface);
                                });
                    }
                }

                // error markings
                if (ParseData.ErrorsByLine.ContainsKey(line.LineNumber))
                {
                    foreach (LanguageError error in ParseData.ErrorsByLine[line.LineNumber])
                    {
                        int startOffset = line.Offset + error.Location.StartPosition.Column - 1;
                        int endOffset = line.Offset + error.Location.EndPosition.Column - 1;
                        if (startOffset == endOffset)
                            endOffset++;

                        if (endOffset > startOffset &&
                            startOffset >= line.Offset &&
                            startOffset <= line.EndOffset &&
                            endOffset >= line.Offset &&
                            endOffset <= line.EndOffset)
                            ChangeLinePart(
                                startOffset,
                                endOffset,
                                (element) =>
                                {
                                    element.TextRunProperties.SetTextDecorations(ERROR_DECORATIONS);
                                });
                    }
                }
            }
        }

        #endregion
    }
}
