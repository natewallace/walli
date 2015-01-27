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
    /// Interaction logic for UserSelectWindow.xaml
    /// </summary>
    public partial class UserSelectWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructors.
        /// </summary>
        public UserSelectWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The currently selected user.
        /// </summary>
        public User SelectedUser
        {
            get
            {
                if (listBoxResults.SelectedItem != null)
                    return listBoxResults.SelectedItem as User;
                else
                    return null;
            }
            set
            {
                listBoxResults.SelectedItem = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Do a user search.
        /// </summary>
        private void Search()
        {
            UserSearchEventArgs args = new UserSearchEventArgs(textBoxSearch.Text);
            OnUserSearch(args);

            listBoxResults.Items.Clear();
            if (args.Results.Length == 0)
            {
                textBlockResults.Text = "No users found";
                scrollViewerResults.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                textBlockResults.Text = "Search results:";
                foreach (User user in args.Results)
                    listBoxResults.Items.Add(user);
                scrollViewerResults.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Raises the UserSearch event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnUserSearch(UserSearchEventArgs e)
        {
            if (UserSearch != null)
                UserSearch(this, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks the user search button.
        /// </summary>
        public UserSearchEventHandler UserSearch;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Do a search.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (App.Wait("Search for users."))
                {
                    Search();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Enable or disable the search button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                buttonSearch.IsEnabled = !String.IsNullOrWhiteSpace(textBoxSearch.Text);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Listen for enter key to do a search.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(textBoxSearch.Text))
                {
                    using (App.Wait("Search for users."))
                    {
                        Search();
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Enable or disable the select button based on there being a selection.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                buttonSelect.IsEnabled = (SelectedUser != null);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the dialog with an ok result.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedUser != null)
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

        /// <summary>
        /// Close the dialog with an ok result.
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

        #endregion
    }
}
