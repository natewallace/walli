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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for ColorSelectWindow.xaml
    /// </summary>
    public partial class ColorSelectWindow : Window
    {
        #region Fields

        /// <summary>
        /// Holds the last selection.
        /// </summary>
        private object _previousSelection;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ColorSelectWindow()
        {
            InitializeComponent();

            _previousSelection = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The selected color.
        /// </summary>
        public Color? SelectedColor
        {
            get
            {
                return GetItemColor(comboBoxInput.SelectedItem);
            }
            set
            {
                if (value != null)
                {
                    bool found = false;
                    foreach (object item in comboBoxInput.Items)
                    {
                        if (item == panelCustomColor)
                            continue;

                        Color? c = GetItemColor(item);
                        if (c.HasValue && c.Value == value)
                        {
                            found = true;
                            comboBoxInput.SelectedItem = item;
                            break;
                        }
                    }

                    if (!found)
                    {
                        object item = CreateItemColor(value.Value);
                        comboBoxInput.Items.Add(item);
                        comboBoxInput.SelectedItem = item;
                    }
                }
                else
                {
                    comboBoxInput.SelectedItem = null;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the color for the given item that is in the combo box item.
        /// </summary>
        /// <param name="item">The combobox item to get the color for.</param>
        /// <returns>The color for the given item or null if it can't be read.</returns>
        private Color? GetItemColor(object item)
        {
            if (item == null)
                return null;

            StackPanel panel = item as StackPanel;
            if (panel == null || panel.Children.Count == 0)
                return null;

            Border border = panel.Children[0] as Border;
            if (border == null)
                return null;

            SolidColorBrush brush = border.Background as SolidColorBrush;
            if (brush == null)
                return null;

            return brush.Color;
        }

        /// <summary>
        /// Create a combobox item for the given color.
        /// </summary>
        /// <param name="color">The color to create an item for.</param>
        /// <returns>The item for display in the combo box.</returns>
        private object CreateItemColor(Color color)
        {
            Border border = new Border();
            border.Height = 16;
            border.Width = 16;
            border.Background = new SolidColorBrush(color);

            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            textBlock.Text = color.ToString();

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(border);
            panel.Children.Add(textBlock);

            return panel;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Close the dialog with the selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Cancel the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedColor = null;
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Process a click on the custom color option.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxInput.SelectedItem == panelCustomColor)
                {
                    System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                        SelectedColor = color;
                    }
                    else
                    {
                        if (_previousSelection != panelCustomColor)
                            comboBoxInput.SelectedItem = _previousSelection;
                    }
                }
                else
                {
                    _previousSelection = comboBoxInput.SelectedItem;
                }

                buttonSelect.IsEnabled = (SelectedColor.HasValue);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
