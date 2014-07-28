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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// The close button.
    /// </summary>
    public class WindowCloseButton : WindowCaptionButton
    {
        #region Methods

        /// <summary>
        /// Draws the icon to display.
        /// </summary>
        /// <param name="arrangeBounds">The bounds to draw in.</param>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="centerX">The center along the x axis.</param>
        /// <param name="centerY">The center along the y axis.</param>
        /// <param name="squareLength">The length of the centered square.</param>
        /// <param name="squareHalfLength">The half length of the centered square.</param>
        /// <param name="squareLeft">The left position of the centered square.</param>
        /// <param name="squareRight">The right position of the centered square.</param>
        /// <param name="squareTop">The top position of the centered square.</param>
        /// <param name="squareBottom">The bottom position of the centered square.</param>
        protected override void DrawIcon(
            Size arrangeBounds,
            Canvas canvas,
            int centerX,
            int centerY,
            int squareLength,
            int squareHalfLength,
            int squareLeft,
            int squareRight,
            int squareTop,
            int squareBottom)
        {
            int shapePadding = 9;

            Line line = new Line();
            line.X1 = squareLeft + shapePadding;
            line.Y1 = squareTop + shapePadding;
            line.X2 = squareRight - shapePadding;
            line.Y2 = squareBottom - shapePadding;
            line.Stroke = Foreground;
            line.StrokeThickness = 2;
            line.StrokeEndLineCap = PenLineCap.Flat;
            canvas.Children.Add(line);

            line = new Line();
            line.X1 = squareRight - shapePadding;
            line.Y1 = squareTop + shapePadding;
            line.X2 = squareLeft + shapePadding;
            line.Y2 = squareBottom - shapePadding;
            line.Stroke = Foreground;
            line.StrokeThickness = 2;
            line.StrokeEndLineCap = PenLineCap.Flat;
            canvas.Children.Add(line);
        }

        /// <summary>
        /// Handle the button press.
        /// </summary>
        protected override void ButtonPress()
        {
            if (ParentWindow != null)
                ParentWindow.Close();
        }

        #endregion
    }
}
