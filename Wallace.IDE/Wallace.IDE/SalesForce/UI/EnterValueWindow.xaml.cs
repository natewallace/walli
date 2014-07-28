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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for EnterNameWindow.xaml
    /// </summary>
    public partial class EnterValueWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnterValueWindow()
        {
            InitializeComponent();

            InputRestriction = null;
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
        public string EnteredValue
        {
            get { return textBoxInput.Text; }
            set { textBoxInput.Text = value; }
        }

        /// <summary>
        /// The max number of characters allowed in the value.
        /// </summary>
        public int InputMaxLength
        {
            get { return textBoxInput.MaxLength; }
            set { textBoxInput.MaxLength = value; }
        }

        /// <summary>
        /// A regular expression.  Only characters that match the regular expression are allowed to be entered.
        /// If null then there are no restrictions.
        /// </summary>
        public string InputRestriction { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns true if the given text is restricted.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>true if the given text is restricted.</returns>
        private bool IsRestricted(string text)
        {
            if (text == null || String.IsNullOrEmpty(InputRestriction))
                return false;

            for (int i = 0; i < text.Length; i++)
            {
                if (!Regex.IsMatch(text.Substring(i, 1), InputRestriction))
                    return true;
            }

            return false;
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
                EnteredValue = String.Empty;
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Treat the enter key as an ok click.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
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
        /// Restrict input if specified.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                e.Handled = IsRestricted(e.Text);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Restrict input if specified.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxInput_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                if (IsRestricted(e.DataObject.GetData(typeof(string)) as string))
                    e.CancelCommand();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
