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
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using SalesForceLanguage;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.CodeModel;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// This class is used to implement sytnax highlighting for apex documents.
    /// </summary>
    public class ApexDocumentColorizingTransformer : DocumentColorizingTransformer
    {
        #region Fields

        /// <summary>
        /// Type references marked in the text.
        /// </summary>
        private Dictionary<int, List<TextSpan>> _typeReferences = new Dictionary<int, List<TextSpan>>();

        /// <summary>
        /// Errors marked in the text.
        /// </summary>
        private Dictionary<int, List<LanguageError>> _errors = new Dictionary<int, List<LanguageError>>();

        /// <summary>
        /// Used to mark errors.
        /// </summary>
        private static TextDecorationCollection ERROR_DECORATIONS;

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

        #region Methods

        /// <summary>
        /// Sets the errors that are marked in the text.
        /// </summary>
        /// <param name="typeReferences">The type references to mark.</param>
        public void SetTypeReferences(IEnumerable<ReferenceTypeSymbol> typeReferences)
        {
            _typeReferences.Clear();
            if (typeReferences != null)
            {
                foreach (ReferenceTypeSymbol s in typeReferences)
                {
                    List<TextSpan> spans = null;
                    if (_typeReferences.ContainsKey(s.Location.Line))
                    {
                        spans = _typeReferences[s.Location.Line];
                    }
                    else
                    {
                        spans = new List<TextSpan>();
                        _typeReferences.Add(s.Location.Line, spans);
                    }

                    foreach (TextSpan span in s.Parts)
                        spans.Add(span);
                }
            }
        }

        /// <summary>
        /// Sets the errors that are marked in the text.
        /// </summary>
        /// <param name="errors">The errors to mark.</param>
        public void SetErrors(IEnumerable<LanguageError> errors)
        {
            _errors.Clear();
            if (errors != null)
            {
                foreach (LanguageError e in errors)
                {
                    if (_errors.ContainsKey(e.Location.StartPosition.Line))
                    {
                        _errors[e.Location.StartPosition.Line].Add(e);
                    }
                    else
                    {
                        List<LanguageError> list = new List<LanguageError>();
                        list.Add(e);
                        _errors.Add(e.Location.StartPosition.Line, list);
                    }
                }
            }
        }

        /// <summary>
        /// Color the given line.
        /// </summary>
        /// <param name="line">The line to color.</param>
        protected override void ColorizeLine(DocumentLine line)
        {
            // type reference markings
            if (_typeReferences.ContainsKey(line.LineNumber))
            {
                foreach (TextSpan span in _typeReferences[line.LineNumber])
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
                                element.TextRunProperties.SetForegroundBrush(Brushes.Teal);
                            });
                }
            }

            // error markings
            if (_errors.ContainsKey(line.LineNumber))
            {
                foreach (LanguageError error in _errors[line.LineNumber])
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

        #endregion
    }
}
