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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Color lines in a diff document.
    /// </summary>
    public class DiffDocumentColorizingTransformer : DocumentColorizingTransformer
    {
        #region Fields

        /// <summary>
        /// The lines that were added.  1-based.
        /// </summary>
        private HashSet<int> _lineAdds;

        /// <summary>
        /// The lines that were deleted.  1-based.
        /// </summary>
        private HashSet<int> _lineDeletes;

        /// <summary>
        /// The brush to paint backgrounds for deletes with.
        /// </summary>
        public static SolidColorBrush DELETE_BRUSH_BACKGROUND = new SolidColorBrush(Color.FromRgb(255, 221, 221));

        /// <summary>
        /// The brush to paint foregrounds for deletes with.
        /// </summary>
        public static SolidColorBrush DELETE_BRUSH_FOREGROUND = new SolidColorBrush(Color.FromRgb(116, 0, 0));

        /// <summary>
        /// The brush to paint a backgrounds for adds with.
        /// </summary>
        public static SolidColorBrush ADD_BRUSH_BACKGROUND = new SolidColorBrush(Color.FromRgb(219, 255, 219));

        /// <summary>
        /// The brush to paint foregrounds for adds with.
        /// </summary>
        public static SolidColorBrush ADD_BRUSH_FOREGROUND = Brushes.Black;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lineAdds">The lines that were added.  1-based.</param>
        /// <param name="lineDeletes">The lines that were deleted.  1-based.</param>
        public DiffDocumentColorizingTransformer(HashSet<int> lineAdds, HashSet<int> lineDeletes)
        {
            _lineAdds = lineAdds ?? new HashSet<int>();
            _lineDeletes = lineDeletes ?? new HashSet<int>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Color the given line.
        /// </summary>
        /// <param name="line">The line to color.</param>
        protected override void ColorizeLine(DocumentLine line)
        {
            if (_lineAdds.Contains(line.LineNumber))
            {
                ChangeLinePart(line.Offset,
                               line.EndOffset,
                               (element) =>
                               {
                                   element.TextRunProperties.SetForegroundBrush(ADD_BRUSH_FOREGROUND);
                                   element.TextRunProperties.SetBackgroundBrush(ADD_BRUSH_BACKGROUND);
                               });
            }
            else if (_lineDeletes.Contains(line.LineNumber))
            {
                ChangeLinePart(line.Offset,
                               line.EndOffset,
                               (element) =>
                               {
                                   element.TextRunProperties.SetForegroundBrush(DELETE_BRUSH_FOREGROUND);
                                   element.TextRunProperties.SetBackgroundBrush(DELETE_BRUSH_BACKGROUND);
                               });
            }
        }

        #endregion
    }
}
