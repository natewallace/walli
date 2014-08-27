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
    /// Interaction logic for TestManagerWindow.xaml
    /// </summary>
    public partial class TestManagerWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructors.
        /// </summary>
        public TestManagerWindow()
        {
            InitializeComponent();
            UpdateView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The test names that a user can select from.
        /// </summary>
        public IEnumerable<string> TestNames
        {
            get
            {
                List<string> result = new List<string>();
                foreach (object item in listBoxTestNames.Items)
                {
                    if (item is CheckBox)
                        result.Add((item as CheckBox).Content as string);
                }

                return result;
            }
            set
            {
                if (value == null)
                {
                    listBoxTestNames.Items.Clear();
                }
                else
                {
                    foreach (string testName in value)
                        listBoxTestNames.Items.Add(new CheckBox() { Content = testName });
                }
            }
        }

        /// <summary>
        /// Get the test names that have been selected by the user.
        /// </summary>
        public IEnumerable<string> SelectedTestNames
        {
            get
            {
                List<string> result = new List<string>();
                foreach (object item in listBoxTestNames.Items)
                {
                    if (item is CheckBox && (item as CheckBox).IsChecked.Value)
                        result.Add((item as CheckBox).Content as string);
                }

                return result;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the view states.
        /// </summary>
        private void UpdateView()
        {
            buttonRun.IsEnabled = SelectedTestNames.Count() > 0;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Check all of the tests.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCheckAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (object item in listBoxTestNames.Items)
                {
                    if (item is CheckBox)
                        (item as CheckBox).IsChecked = true;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Unheck all of the tests.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (object item in listBoxTestNames.Items)
                {
                    if (item is CheckBox)
                        (item as CheckBox).IsChecked = false;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Set the enabled state of the Run button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void checkBox_CheckChange(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateView();
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
        /// Close the dialog with an OK.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonRun_Click(object sender, RoutedEventArgs e)
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

        #endregion
    }
}
