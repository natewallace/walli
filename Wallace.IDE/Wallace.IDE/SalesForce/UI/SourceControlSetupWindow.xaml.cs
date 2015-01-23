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
    /// Interaction logic for SourceControlSetupWindow.xaml
    /// </summary>
    public partial class SourceControlSetupWindow : Window
    {
        #region Fields

        /// <summary>
        /// Supports the IsCheckoutEnabled property.
        /// </summary>
        private bool _isCheckoutEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceControlSetupWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set the checkout enabled indicator.
        /// </summary>
        public bool IsCheckoutEnabled
        {
            get
            {
                return _isCheckoutEnabled;
            }
            set
            {
                _isCheckoutEnabled = value;
                if (value)
                {
                    textBoxGitRepositoryUrl.Visibility = Visibility.Visible;
                    textBoxUsername.Visibility = Visibility.Visible;
                    passwordBoxPassword.Visibility = Visibility.Visible;
                    textBoxBranchName.Visibility = Visibility.Visible;

                    textBlockGitRepositoryUrl.Visibility = Visibility.Visible;
                    textBlockUsername.Visibility = Visibility.Visible;
                    textBlockPassword.Visibility = Visibility.Visible;
                    textBlockBranchName.Visibility = Visibility.Visible;

                    buttonEnableCheckoutSystem.Content = "Click here to disable the check out system";                    
                }
                else
                {
                    textBoxGitRepositoryUrl.Visibility = Visibility.Collapsed;
                    textBoxUsername.Visibility = Visibility.Collapsed;
                    passwordBoxPassword.Visibility = Visibility.Collapsed;
                    textBoxBranchName.Visibility = Visibility.Collapsed;

                    textBlockGitRepositoryUrl.Visibility = Visibility.Collapsed;
                    textBlockUsername.Visibility = Visibility.Collapsed;
                    textBlockPassword.Visibility = Visibility.Collapsed;
                    textBlockBranchName.Visibility = Visibility.Collapsed;

                    textBoxGitRepositoryUrl.Clear();
                    textBoxUsername.Clear();
                    passwordBoxPassword.Clear();
                    textBoxBranchName.Clear();

                    buttonEnableCheckoutSystem.Content = "Click here to enable the check out system";
                }
            }
        }

        /// <summary>
        /// The repository url.
        /// </summary>
        public string RepositoryUrl
        {
            get { return textBoxGitRepositoryUrl.Text; }
            set { textBoxGitRepositoryUrl.Text = value; }
        }

        /// <summary>
        /// The user name.
        /// </summary>
        public string Username
        {
            get { return textBoxUsername.Text; }
            set { textBoxUsername.Text = value; }
        }

        /// <summary>
        /// The password.
        /// </summary>
        public string Password
        {
            get { return passwordBoxPassword.Password; }
            set { passwordBoxPassword.Password = value; }
        }

        /// <summary>
        /// The name of the branch.
        /// </summary>
        public string BranchName
        {
            get { return textBoxBranchName.Text; }
            set { textBoxBranchName.Text = value; }
        }

        /// <summary>
        /// Enables/Disables the delete repository button.
        /// </summary>
        public bool IsDeleteRepositoryEnabled
        {
            get { return buttonDelete.IsEnabled; }
            set { buttonDelete.IsEnabled = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the ToggleCheckOutSystemClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnToggleCheckOutSystemClick(EventArgs e)
        {
            if (ToggleCheckOutSystemClick != null)
                ToggleCheckOutSystemClick(this, e);
        }

        /// <summary>
        /// Raises the DeleteRepositoryClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnDeleteRepositoryClick(EventArgs e)
        {
            if (DeleteRepositoryClick != null)
                DeleteRepositoryClick(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Close the dialog with an ok result.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Raise the ToggleCheckOutSystemClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonEnableCheckoutSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnToggleCheckOutSystemClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the DeleteRepositoryClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnDeleteRepositoryClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks to toggle on/off the check out system.
        /// </summary>
        public event EventHandler ToggleCheckOutSystemClick;

        /// <summary>
        /// Raised when the user clicks the delete repository button.
        /// </summary>
        public event EventHandler DeleteRepositoryClick;

        #endregion
    }
}
