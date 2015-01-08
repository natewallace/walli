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
    /// Interaction logic for SelectValueWindow.xaml
    /// </summary>
    public partial class SelectValueWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SelectValueWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The input label displayed.
        /// </summary>
        public string InputLabel
        {
            get { return textBlockInput.Text; }
            set { textBlockInput.Text = value; }
        }

        /// <summary>
        /// The action label displayed.
        /// </summary>
        public string ActionLabel
        {
            get { return buttonSelect.Content as string; }
            set { buttonSelect.Content = value; }
        }

        /// <summary>
        /// The value entered by the user.
        /// </summary>
        public object SelectedValue
        {
            get { return listBoxInput.SelectedItem; }
            set { listBoxInput.SelectedItem = value; }
        }

        /// <summary>
        /// The items the user can select from.
        /// </summary>
        public System.Collections.IList Items
        {
            get { return listBoxInput.Items; }
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
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the UI.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                buttonSelect.IsEnabled = (listBoxInput.SelectedItem != null);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Treat double click as a select click.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                try
                {
                    if (buttonSelect.IsEnabled)
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
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
