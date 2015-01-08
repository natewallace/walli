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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for TabOverflowButton.xaml
    /// </summary>
    public partial class TabOverflowButton : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabOverflowButton()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Display context menu.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control control = sender as Control;
                control.ContextMenu.Items.Clear();

                TabControl tabControl = VisualHelper.GetAncestor<TabControl>(control);
                if (tabControl != null)
                {
                    List<MenuItem> menuItems = new List<MenuItem>();
                    foreach (TabItem tabItem in tabControl.Items)
                    {
                        TextBlock textBlock = VisualHelper.GetChild<TextBlock>(tabItem.Header as DependencyObject);
                        Image image = VisualHelper.GetChild<Image>(tabItem.Header as DependencyObject);

                        if (textBlock != null && !String.IsNullOrWhiteSpace(textBlock.Text))
                        {
                            object header = textBlock.Text;
                            if (image != null)
                            {
                                StackPanel stack = new StackPanel();
                                stack.Orientation = Orientation.Horizontal;
                                stack.Children.Add(new Image()
                                {
                                    Source = image.Source,
                                    Height = image.Height,
                                    Width = image.Width,
                                    Margin = new Thickness(2)
                                });
                                stack.Children.Add(new TextBlock()
                                {
                                    Text = textBlock.Text,
                                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                                });

                                header = stack;
                            }

                            menuItems.Add(new MenuItem()
                            {
                                Header = header,
                                Tag = tabItem
                            });
                        }
                    }

                    foreach (MenuItem menuItem in menuItems.OrderBy(mi => mi.Header.ToString()))
                        control.ContextMenu.Items.Add(menuItem);
                }

                if (control.ContextMenu.Items.Count > 0)
                {
                    control.ContextMenu.IsEnabled = true;
                    control.ContextMenu.PlacementTarget = control;
                    control.ContextMenu.Placement = PlacementMode.Bottom;
                    control.ContextMenu.IsOpen = true;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Display the clicked tab item.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Arguments for the event.</param>
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = e.OriginalSource as MenuItem;
                TabItem tabItem = menuItem.Tag as TabItem;

                TabControl tabControl = VisualHelper.GetAncestor<TabControl>(tabItem);
                if (tabControl != null)
                {
                    int index = tabControl.Items.IndexOf(tabItem);
                    if (index != -1)
                    {
                        tabControl.Items.RemoveAt(index);
                        tabControl.Items.Insert(0, tabItem);
                        tabControl.SelectedItem = tabItem;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
