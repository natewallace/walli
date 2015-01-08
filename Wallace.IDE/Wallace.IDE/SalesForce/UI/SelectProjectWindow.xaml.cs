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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for SelectProjectWindow.xaml
    /// </summary>
    public partial class SelectProjectWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SelectProjectWindow()
        {
            InitializeComponent();
            buttonSelect.IsEnabled = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project names to display.
        /// </summary>
        public string[] ProjectNames
        {
            get
            {
                return listBoxProjectNames.Tag as string[];
            }
            set
            {
                listBoxProjectNames.Items.Clear();
                listBoxProjectNames.Tag = value;
                if (value != null)
                {
                    foreach (string name in value)
                    {
                        listBoxProjectNames.Items.Add(new TextBlock()
                        {
                            Text = name,
                            Padding = new Thickness(3)
                        });
                    }
                }
            }
        }

        /// <summary>
        /// The currently selected project name if there is one.
        /// </summary>
        public string SelectedProjectName
        {
            get
            {
                TextBlock tb = listBoxProjectNames.SelectedItem as TextBlock;
                if (tb == null)
                    return null;
                else
                    return tb.Text;
            }
        }

        /// <summary>
        /// The label for the select button.
        /// </summary>
        public string SelectLabel
        {
            get { return buttonSelect.Content as string; }
            set { buttonSelect.Content = value; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the enabled state of the Select button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxProjectNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                buttonSelect.IsEnabled = (SelectedProjectName != null);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Set the dialog result and close the window.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
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
        /// Set the dialog result and close the window.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Set the dialog result and close the window.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxProjectNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedProjectName != null)
                {
                    DialogResult = true;
                    Close();
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
