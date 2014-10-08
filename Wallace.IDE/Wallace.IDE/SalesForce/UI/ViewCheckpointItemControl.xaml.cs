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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for ViewCheckpointItemControl.xaml
    /// </summary>
    public partial class ViewCheckpointItemControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewCheckpointItemControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The file name displayed.
        /// </summary>
        public string FileName
        {
            get { return textBlockFile.Text; }
            set { textBlockFile.Text = value; }
        }

        /// <summary>
        /// The line number displayed.
        /// </summary>
        public string LineNumber
        {
            get { return textBlockLine.Text; }
            set { textBlockLine.Text = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the EditClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnEditClick(EventArgs e)
        {
            if (EditClick != null)
                EditClick(this, e);
        }

        /// <summary>
        /// Raises the DeleteClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnDeleteClick(EventArgs e)
        {
            if (DeleteClick != null)
                DeleteClick(this, e);
        }

        /// <summary>
        /// Raises the ResultsClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnResultsClick(EventArgs e)
        {
            if (ResultsClick != null)
                ResultsClick(this, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks edit.
        /// </summary>
        public event EventHandler EditClick;

        /// <summary>
        /// Raised when the user clicks delete.
        /// </summary>
        public event EventHandler DeleteClick;

        /// <summary>
        /// Raised when the user clicks results.
        /// </summary>
        public event EventHandler ResultsClick;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the EditClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnEditClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the DeleteClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnDeleteClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the ResultsClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnResultsClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
