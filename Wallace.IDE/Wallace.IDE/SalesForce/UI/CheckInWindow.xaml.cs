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

using SalesForceData;
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
    /// Interaction logic for CheckInWindow.xaml
    /// </summary>
    public partial class CheckInWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CheckInWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Comments entered by user.
        /// </summary>
        public string Comment
        {
            get { return textBoxComment.Text; }
            set { textBoxComment.Text = value; }
        }

        /// <summary>
        /// The checked out files.
        /// </summary>
        public CheckoutFile[] Files
        {
            get
            {
                List<CheckoutFile> files = new List<CheckoutFile>();
                foreach (CheckBox cb in listBoxFiles.Items)
                    files.Add(cb.Tag as CheckoutFile);

                return files.ToArray();
            }
            set
            {
                listBoxFiles.Items.Clear();
                if (value != null)
                {
                    foreach (CheckoutFile file in value.OrderBy(f => f.FullEntityName))
                    {
                        CheckBox checkbox = new CheckBox();
                        checkbox.Tag = file;
                        checkbox.Content = file.FullEntityName;
                        listBoxFiles.Items.Add(checkbox);
                    }
                }
            }
        }

        /// <summary>
        /// The files that are currently selected.
        /// </summary>
        public CheckoutFile[] SelectedFiles
        {
            get
            {
                List<CheckoutFile> files = new List<CheckoutFile>();
                foreach (CheckBox cb in listBoxFiles.Items)
                {
                    if (cb.IsChecked.HasValue && cb.IsChecked.Value == true)
                        files.Add(cb.Tag as CheckoutFile);
                }

                return files.ToArray();
            }
            set
            {
                HashSet<string> ids = new HashSet<string>();
                if (value != null)
                    foreach (CheckoutFile file in value)
                        ids.Add(file.EntityId);

                foreach (CheckBox cb in listBoxFiles.Items)
                    cb.IsChecked = ids.Contains((cb.Tag as CheckoutFile).EntityId);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the enabled state of the commit button.
        /// </summary>
        private void UpdateViewState()
        {
            buttonCommit.IsEnabled = !(String.IsNullOrWhiteSpace(Comment) || SelectedFiles.Length == 0);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Select all of the files.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (CheckBox cb in listBoxFiles.Items)
                    cb.IsChecked = true;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Select none of the files.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (CheckBox cb in listBoxFiles.Items)
                    cb.IsChecked = false;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the dialog with a cancel result.
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
        /// Close the dialog with an OK result.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCommit_Click(object sender, RoutedEventArgs e)
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
        /// Update the view state.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UpdateViewState();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the view state.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void checkBox_CheckChange(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateViewState();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
