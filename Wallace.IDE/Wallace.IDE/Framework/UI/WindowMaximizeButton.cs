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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// The maximize button.
    /// </summary>
    public class WindowMaximizeButton : WindowCaptionButton
    {
        #region Methods

        /// <summary>
        /// Set visibility based on ResizeMode.
        /// </summary>
        protected override void Init()
        {
            base.Init();

            if (ParentWindow != null)
            {
                switch (ParentWindow.ResizeMode)
                {
                    case ResizeMode.CanMinimize:
                    case ResizeMode.NoResize:
                        Visibility = System.Windows.Visibility.Collapsed;
                        break;

                    default:
                        Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
        }

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

            if (ParentWindow != null && ParentWindow.WindowState != WindowState.Maximized)
            {
                int sideLength = squareLength - (shapePadding * 2) + 2;
                if (sideLength < 0)
                    sideLength = 0;

                Rectangle rect = new Rectangle();
                rect.Width = sideLength;
                rect.Height = sideLength;
                Canvas.SetLeft(rect, squareLeft + shapePadding - 1);
                Canvas.SetTop(rect, squareTop + shapePadding);
                rect.Stroke = Foreground;
                rect.StrokeThickness = 1;
                RenderOptions.SetEdgeMode(rect, EdgeMode.Aliased);
                canvas.Children.Add(rect);

                Line line = new Line();
                line.X1 = squareLeft + shapePadding - 1;
                line.Y1 = squareTop + shapePadding + 2;
                line.X2 = squareRight - shapePadding + 1;
                line.Y2 = squareTop + shapePadding + 2;
                line.Stroke = Foreground;
                line.StrokeThickness = 3;
                line.StrokeEndLineCap = PenLineCap.Flat;
                RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
                canvas.Children.Add(line);
            }
            else
            {
                int sideLength = squareLength - (shapePadding * 2);
                if (sideLength < 0)
                    sideLength = 0;

                Rectangle rect = new Rectangle();
                rect.Width = sideLength;
                rect.Height = sideLength;
                Canvas.SetLeft(rect, squareLeft + shapePadding + 2);
                Canvas.SetTop(rect, squareTop + shapePadding - 2);
                rect.Stroke = Foreground;
                rect.StrokeThickness = 1;
                RenderOptions.SetEdgeMode(rect, EdgeMode.Aliased);
                canvas.Children.Add(rect);

                Line line = new Line();
                line.X1 = squareLeft + shapePadding + 2;
                line.Y1 = squareTop + shapePadding + 2 - 2;
                line.X2 = squareRight - shapePadding + 2;
                line.Y2 = squareTop + shapePadding + 2 - 2;
                line.Stroke = Foreground;
                line.StrokeThickness = 3;
                line.StrokeEndLineCap = PenLineCap.Flat;
                RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
                canvas.Children.Add(line);

                rect = new Rectangle();
                rect.Width = sideLength;
                rect.Height = sideLength;
                Canvas.SetLeft(rect, squareLeft + shapePadding - 2);
                Canvas.SetTop(rect, squareTop + shapePadding + 3);
                rect.Stroke = Foreground;
                rect.StrokeThickness = 1;
                RenderOptions.SetEdgeMode(rect, EdgeMode.Aliased);
                canvas.Children.Add(rect);

                line = new Line();
                line.X1 = squareLeft + shapePadding - 2;
                line.Y1 = squareTop + shapePadding + 2 + 3;
                line.X2 = squareRight - shapePadding - 2;
                line.Y2 = squareTop + shapePadding + 2 + 3;
                line.Stroke = Foreground;
                line.StrokeThickness = 3;
                line.StrokeEndLineCap = PenLineCap.Flat;
                RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Handle the button press.
        /// </summary>
        protected override void ButtonPress()
        {
            if (ParentWindow != null)
            {
                switch (ParentWindow.WindowState)
                {
                    case WindowState.Maximized:
                        ParentWindow.WindowState = WindowState.Normal;
                        break;

                    default:
                        ParentWindow.WindowState = WindowState.Maximized;
                        break;
                }
            }
        }

        /// <summary>
        /// Get the max size allowed for this window based on the current screen.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The max size allowed for this window based on the current screen.</returns>
        private Win32.MINMAXINFO GetMaxInfo(IntPtr hwnd)
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(hwnd);
            Win32.MINMAXINFO info = new Win32.MINMAXINFO();
            info.ptMaxPosition.x = Math.Abs(screen.WorkingArea.Left - screen.Bounds.Left);
            info.ptMaxPosition.y = Math.Abs(screen.WorkingArea.Top - screen.Bounds.Top);
            info.ptMaxSize.x = screen.WorkingArea.Width;
            info.ptMaxSize.y = screen.WorkingArea.Height;            

            return info;
        }

        /// <summary>
        /// Process the message that requests the size for a max window.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="msg">The message to process.</param>
        /// <param name="wParam">A parameter.</param>
        /// <param name="lParam">A parameter.</param>
        /// <param name="handled">A value that indicates if the message was handled.</param>
        /// <returns>IntPtr.Zero.</returns>
        protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {            
            switch (msg)
            {
                case Win32.Constant.WM_GETMINMAXINFO:

                    Win32.MINMAXINFO info = (Win32.MINMAXINFO)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));
                    Win32.MINMAXINFO maxInfo = GetMaxInfo(hwnd);

                    info.ptMaxPosition.x = maxInfo.ptMaxPosition.x;
                    info.ptMaxPosition.y = maxInfo.ptMaxPosition.y;
                    info.ptMaxSize.x = maxInfo.ptMaxSize.x;
                    info.ptMaxSize.y = maxInfo.ptMaxSize.y;

                    System.Runtime.InteropServices.Marshal.StructureToPtr(info, lParam, true);
                    return IntPtr.Zero;

                default:
                    return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Set border thickness based on the maximize state.
        /// </summary>
        /// <param name="e">Size info.</param>
        protected override void SizeChange(SizeChangedEventArgs e)
        {
            base.SizeChange(e);

            if (ParentWindow != null)
            {
                if (ParentWindow.WindowState == WindowState.Maximized)
                {
                    ParentWindow.BorderThickness = new Thickness(0);
                }
                else
                {
                    Win32.MINMAXINFO maxInfo = GetMaxInfo(ParentWindowHwnd);
                    if (e.NewSize.Height == maxInfo.ptMaxSize.y && ParentWindow.Top == 0)
                    {
                        ParentWindow.BorderThickness = new Thickness(0);
                    }
                    else
                    {
                        ParentWindow.BorderThickness = new Thickness(4);
                    }
                }
            }
        }

        #endregion
    }
}
