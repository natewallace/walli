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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for WindowIconImage.xaml
    /// </summary>
    public partial class WindowIconImage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowIconImage()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set icon displayed.
        /// </summary>
        /// <param name="oldParent">Not used.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.Icon != null)
                {
                    BitmapFrame frame = parentWindow.Icon as BitmapFrame;
                    if (frame != null)
                    {
                        BitmapFrame imageFrame = GetFrame(frame, 24, 24, 32);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 24, 24, 24);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 24, 24, -1);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 32, 32, 32);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 32, 32, 24);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 32, 32, -1);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 16, 16, 32);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 16, 16, 24);
                        if (imageFrame == null)
                            imageFrame = GetFrame(frame, 16, 16, -1);

                        imageIcon.Source = imageFrame;
                    }
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Get the frame requested.
        /// </summary>
        /// <param name="image">The image to get the frame from.</param>
        /// <param name="height">The height o the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="bitsPerPixel">The resolution of the frame.</param>
        /// <returns>The requested frame or null if it doesn't exist.</returns>
        private BitmapFrame GetFrame(BitmapFrame image, int height, int width, int bitsPerPixel)
        {
            return image.Decoder.Frames.First(f => f.PixelHeight == height &&
                                                   f.PixelWidth == width &&
                                                   (bitsPerPixel == -1 || f.Format.BitsPerPixel == bitsPerPixel));
        }

        #endregion
    }
}
