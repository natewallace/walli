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

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for SourceFilePropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public PropertiesWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a property that will be displayed on the grid.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void AddProperty(string name, string value)
        {
            int row = gridMain.RowDefinitions.Count;
            gridMain.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            TextBlock textBlockName = new TextBlock();
            textBlockName.Text = String.Format("{0}:", name);
            textBlockName.Margin = new Thickness(5);
            textBlockName.FontWeight = FontWeights.Bold;
            textBlockName.HorizontalAlignment = HorizontalAlignment.Right;
            textBlockName.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(textBlockName, row);
            Grid.SetColumn(textBlockName, 0);            
            gridMain.Children.Add(textBlockName);            

            TextBlock textBlockValue = new TextBlock();
            textBlockValue.Text = value;
            textBlockValue.Margin = new Thickness(5);
            textBlockValue.TextWrapping = TextWrapping.Wrap;
            textBlockValue.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(textBlockValue, row);
            Grid.SetColumn(textBlockValue, 1);
            gridMain.Children.Add(textBlockValue);            
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Close this dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event argumetns.</param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
