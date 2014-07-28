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
using System.Windows.Media.Imaging;

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Helper methods for dealing with the visual controls.
    /// </summary>
    public static class VisualHelper
    {
        #region Methods

        /// <summary>
        /// Gets the first ancestor found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type to look for.</typeparam>
        /// <param name="element">The element to start the search from.</param>
        /// <returns>The first ancestor found with the given type or null if one isn't found.</returns>
        public static TType GetAncestor<TType>(DependencyObject element) where TType : DependencyObject
        {
            if (element == null)
                return null;

            if (element is TType)
                return element as TType;

            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            while (parent != null)
            {
                if (parent is TType)
                    return parent as TType;
                else
                    parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            return null;
        }

        /// <summary>
        /// Return the first child found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type to look for.</typeparam>
        /// <param name="element">The element to search from.</param>
        /// <returns>The first child found with the given type or null if one isn't found.</returns>
        public static TType GetChild<TType>(DependencyObject element) where TType : DependencyObject
        {
            if (element == null)
                return null;

            if (element is TType)
                return element as TType;

            if (element is Panel)
            {
                Panel panel = element as Panel;

                // check the last child in a dockpanel first as the element that is added last often is set to
                // fill which would make it more likely to be the desired element.
                if (panel is DockPanel &&
                    panel.Children.Count > 0 &&
                    panel.Children[panel.Children.Count - 1] is TType)
                    return panel.Children[panel.Children.Count - 1] as TType;

                foreach (UIElement child in panel.Children)
                {
                    TType result = GetChild<TType>(child);
                    if (result != null)
                        return result;
                }
            }
            else if (element is ItemsControl)
            {
                foreach (object child in (element as ItemsControl).Items)
                {
                    TType result = GetChild<TType>(child as DependencyObject);
                    if (result != null)
                        return result;
                }
            }
            else if (element is ContentControl)
            {
                return GetChild<TType>((element as ContentControl).Content as DependencyObject);
            }
            else if (element is Decorator)
            {
                return GetChild<TType>((element as Decorator).Child);
            }

            return null;
        }

        /// <summary>
        /// Load the given bitmap.
        /// </summary>
        /// <param name="name">The name of the bitmap to load.</param>
        /// <returns>The loaded bitmap.</returns>
        public static BitmapImage LoadBitmap(string name)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(String.Format(@"pack://application:,,,/Resources/{0}", name));
            bitmap.EndInit();

            return bitmap;
        }

        /// <summary>
        /// Create a header that has an icon.
        /// </summary>
        /// <param name="text">The text to display in the header.</param>
        /// <param name="iconUrl">The icon to display in the header.</param>
        /// <returns>The newly created header.</returns>
        public static object CreateIconHeader(string text, string iconUrl)
        {
            return CreateIconHeader(text, iconUrl, new Thickness(2), null);
        }

        /// <summary>
        /// Create a header that has an icon.
        /// </summary>
        /// <param name="text">The text to display in the header.</param>
        /// <param name="iconUrl">The icon to display in the header.</param>
        /// <param name="padding">The padding to apply around the entire icon header.</param>
        /// <returns>The newly created header.</returns>
        public static object CreateIconHeader(string text, string iconUrl, Thickness padding)
        {
            return CreateIconHeader(text, iconUrl, padding, null);
        }

        /// <summary>
        /// Create a header that has an icon.
        /// </summary>
        /// <param name="text">The text to display in the header.</param>
        /// <param name="iconUrl">The icon to display in the header.</param>
        /// <param name="trailingText">Text that follows the displayed text parameter.</param>
        /// <returns>The newly created header.</returns>
        public static object CreateIconHeader(string text, string iconUrl, string trailingText)
        {
            return CreateIconHeader(text, iconUrl, new Thickness(2), trailingText);
        }

        /// <summary>
        /// Create a header that has an icon.
        /// </summary>
        /// <param name="text">The text to display in the header.</param>
        /// <param name="iconUrl">The icon to display in the header.</param>
        /// <param name="padding">The padding to apply around the entire icon header.</param>
        /// <param name="trailingText">Text that follows the displayed text parameter.</param>
        /// <returns>The newly created header.</returns>
        public static object CreateIconHeader(string text, string iconUrl, Thickness padding, string trailingText)
        {
            Border border = new Border();
            border.Padding = padding;

            DockPanel panel = new DockPanel();
            panel.LastChildFill = true;

            if (iconUrl != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(String.Format("Resources/{0}", iconUrl), UriKind.Relative);
                bitmap.EndInit();

                Image image = new Image();
                image.Width = 16;
                image.Height = 16;
                image.Source = LoadBitmap(iconUrl);
                DockPanel.SetDock(image, Dock.Left);
                panel.Children.Add(image);
            }

            if (trailingText != null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = trailingText;
                textBlock.Margin = new System.Windows.Thickness(0, 0, 3, 0);
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                DockPanel.SetDock(textBlock, Dock.Right);
                panel.Children.Add(textBlock);
            }

            if (text != null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                textBlock.Margin = new System.Windows.Thickness(3, 0, 3, 0);
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                panel.Children.Add(textBlock);
            }

            border.Child = panel;
            return border;
        }

        #endregion
    }
}
