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
using SalesForceData;
using Wallace.IDE.Framework.UI;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for EditSalesForceCredentialWindow.xaml
    /// </summary>
    public partial class EditSalesForceCredentialWindow : Window
    {
        #region Fields

        /// <summary>
        /// Supports the Credential property.
        /// </summary>
        private SalesForceCredential _credential;

        /// <summary>
        /// Used to display errors.
        /// </summary>
        private ErrorProvider _errors;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditSalesForceCredentialWindow()
        {
            InitializeComponent();

            Credential = null;
            _errors = new ErrorProvider();

            comboBoxDomain.Items.Add(SalesForceDomain.Sandbox);
            comboBoxDomain.Items.Add(SalesForceDomain.Production);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The currently entered credential or null if there isn't one.
        /// </summary>
        public SalesForceCredential Credential
        {
            get
            {
                return _credential;
            }
            set
            {
                _credential = value;

                if (_credential == null)
                {
                    Domain = SalesForceDomain.Sandbox;
                    Username = String.Empty;
                    Password = String.Empty;
                    Token = String.Empty;
                }
                else
                {
                    Domain = _credential.Domain;
                    Username = _credential.Username;
                    Password = _credential.Password;
                    Token = _credential.Token;
                }
            }
        }

        /// <summary>
        /// The domain for the credentail.
        /// </summary>
        private SalesForceDomain Domain
        {
            get
            {
                return (SalesForceDomain)comboBoxDomain.SelectedItem;
            }
            set
            {
                comboBoxDomain.SelectedItem = value;
            }
        }

        /// <summary>
        /// The username for the credential.
        /// </summary>
        private string Username
        {
            get
            {
                return textBoxUsername.Text;
            }
            set
            {
                textBoxUsername.Text = value;
            }
        }

        /// <summary>
        /// The password for the credential.
        /// </summary>
        private string Password
        {
            get
            {
                return passwordBoxPassword.Password;
            }
            set
            {
                passwordBoxPassword.Password = value;
            }
        }

        /// <summary>
        /// The token for the credential.
        /// </summary>
        private string Token
        {
            get
            {
                return passwordBoxToken.Password;
            }
            set
            {
                passwordBoxToken.Password = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate the user input.
        /// </summary>
        /// <returns>false if there is invalid input, true if all input is valid.</returns>
        private bool ValidateUserInput()
        {
            bool isValid = true;
            _errors.ClearErrors();

            if (String.IsNullOrWhiteSpace(Username))
            {
                isValid = false;
                _errors.SetError(textBoxUsername, "A username is required.");
            }

            if (String.IsNullOrWhiteSpace(Password))
            {
                isValid = false;
                _errors.SetError(passwordBoxPassword, "A password is required.");
            }

            return isValid;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Cancel the edit.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _credential = null;
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Save the edit.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateUserInput())
                {
                    _credential = new SalesForceCredential(
                        Domain,
                        Username,
                        Password,
                        Token);

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
