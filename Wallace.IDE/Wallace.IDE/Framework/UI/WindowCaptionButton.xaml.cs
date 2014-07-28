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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for WindowCaptionButton.xaml
    /// </summary>
    public abstract partial class WindowCaptionButton : UserControl
    {
        #region Fields

        /// <summary>
        /// When the foreground is changed the original value is held in this field.
        /// </summary>
        private Brush _foregroundOriginal;

        /// <summary>
        /// When the background is changed the original value is held in this field.
        /// </summary>
        private Brush _backgroundOriginal;

        /// <summary>
        /// Set to true after the original colors are captured.
        /// </summary>
        private bool _originalColorsCaptured;

        /// <summary>
        /// Set to true on a mouse down event.
        /// </summary>
        private bool _isButtonPressed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowCaptionButton()
        {
            InitializeComponent();
            ParentWindow = null;
            _foregroundOriginal = null;
            _backgroundOriginal = null;
            _originalColorsCaptured = false;
            _isButtonPressed = false;
            HotTrackBackground = null;
            HotTrackForeground = null;
            PressedBackground = null;
            PressedForeground = null;            
        }

        #endregion

        #region Properties

        /// <summary>
        /// The current parent window.
        /// </summary>
        protected Window ParentWindow { get; private set; }

        /// <summary>
        /// The Hwnd for the current parent window.
        /// </summary>
        protected IntPtr ParentWindowHwnd
        {
            get
            {
                if (ParentWindow == null)
                    return IntPtr.Zero;

                return (System.Windows.Interop.HwndSource.FromVisual(ParentWindow) as System.Windows.Interop.HwndSource).Handle;
            }
        }

        /// <summary>
        /// The foreground color when the mouse is over the button.
        /// </summary>
        public Brush HotTrackForeground { get; set; }

        /// <summary>
        /// The background color when the mouse is over the button.
        /// </summary>
        public Brush HotTrackBackground { get; set; }

        /// <summary>
        /// The foreground color when the button is being pressed.
        /// </summary>
        public Brush PressedForeground { get; set; }

        /// <summary>
        /// The background color when the button is being pressed.
        /// </summary>
        public Brush PressedBackground { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the colors of the control based on the current state.
        /// </summary>
        private void SetColors()
        {
            if (!_originalColorsCaptured)
            {
                _foregroundOriginal = Foreground;
                _backgroundOriginal = Background;
                _originalColorsCaptured = true;
            }

            if (IsMouseOver)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && _isButtonPressed)
                {
                    if (PressedForeground != null)
                        Foreground = PressedForeground;
                    if (PressedBackground != null)
                        Background = PressedBackground;
                }
                else
                {
                    if (HotTrackForeground != null)
                        Foreground = HotTrackForeground;
                    if (HotTrackBackground != null)
                        Background = HotTrackBackground;
                }
            }
            else
            {
                Foreground = _foregroundOriginal;
                Background = _backgroundOriginal;
            }
        }        

        /// <summary>
        /// Attach event handers to events on parent window.
        /// </summary>
        private void AttachWindowEventHandlers()
        {
            DetachWindowEventHandlers();
            if (ParentWindow != null)
            {
                ParentWindow.StateChanged += parentWindow_StateChanged;
                ParentWindow.SizeChanged += parentWindow_SizeChanged;
                (System.Windows.Interop.HwndSource.FromVisual(ParentWindow) as System.Windows.Interop.HwndSource).AddHook(WindowProc);
            }
        }

        /// <summary>
        /// Remove event handlers from events on parent window.
        /// </summary>
        private void DetachWindowEventHandlers()
        {
            if (ParentWindow != null)
            {
                ParentWindow.StateChanged -= parentWindow_StateChanged;
                ParentWindow.SizeChanged -= parentWindow_SizeChanged;
                (System.Windows.Interop.HwndSource.FromVisual(ParentWindow) as System.Windows.Interop.HwndSource).RemoveHook(WindowProc);
            }
        }

        /// <summary>
        /// Attach event handlers to the current window.
        /// </summary>
        /// <param name="oldParent">Not used.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            ParentWindow = Window.GetWindow(this);
            AttachWindowEventHandlers();
            Init();
        }

        /// <summary>
        /// Draw the icon.
        /// </summary>
        /// <param name="arrangeBounds">The size for the icon.</param>
        /// <returns>The size used.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            DrawIcon(size, canvas);
            SetColors();
            return size;
        }

        /// <summary>
        /// Set highlights.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _isButtonPressed = true;
            SetColors();
        }

        /// <summary>
        /// Process click.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            ButtonPress();

            _isButtonPressed = false;
            SetColors();
        }

        /// <summary>
        /// Set the foreground color.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            SetColors();
        }

        /// <summary>
        /// Set the foreground color.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isButtonPressed = false;
            SetColors();
        }

        /// <summary>
        /// Do setup and then call the virtual DrawIcon method.
        /// </summary>
        /// <param name="arrangeBounds">The bounds to draw in.</param>
        /// <param name="canvas">The canvas to draw on.</param>
        private void DrawIcon(Size arrangeBounds, Canvas canvas)
        {
            canvas.Children.Clear();

            // create background so clicks register for entire control
            Rectangle back = new Rectangle();
            back.Fill = Brushes.Transparent;
            back.Width = arrangeBounds.Width;
            back.Height = arrangeBounds.Height;
            canvas.Children.Add(back);

            // calcuate measurements for centered square within the control
            int centerX = (arrangeBounds.Width == 0) ? 0 : (int)(arrangeBounds.Width / 2);
            int centerY = (arrangeBounds.Height == 0) ? 0 : (int)(arrangeBounds.Height / 2);
            int squareLength = Math.Min((int)arrangeBounds.Width, (int)arrangeBounds.Height);
            int squareHalfLength = (squareLength == 0) ? 0 : (int)(squareLength / 2);
            int squareLeft = centerX - squareHalfLength;
            int squareRight = centerX + squareHalfLength;
            int squareTop = centerY - squareHalfLength;
            int squareBottom = centerY + squareHalfLength;

            DrawIcon(arrangeBounds,
                     canvas,
                     centerX,
                     centerY,
                     squareLength,
                     squareHalfLength,
                     squareLeft,
                     squareRight,
                     squareTop,
                     squareBottom);
        }

        /// <summary>
        /// Classes that inherit from this class can override this method to initialize when attached to a window.
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// Classes that inherit from this class can override this method to draw an icon.
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
        protected virtual void DrawIcon(
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
        }

        /// <summary>
        /// Classes that inherit from this class can override this method to handle a button press.
        /// </summary>
        protected virtual void ButtonPress()
        {
        }

        /// <summary>
        /// Classes that inherit from this class can override this method to handle size changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void SizeChange(SizeChangedEventArgs e)
        {
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
        protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update button display.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void parentWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                DrawIcon(new Size(ActualWidth, ActualHeight), canvas);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Handle size changes.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void parentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                SizeChange(e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
