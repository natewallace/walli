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
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for LogParametersManagerWindow.xaml
    /// </summary>
    public partial class LogParametersManagerWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogParametersManagerWindow()
        {
            InitializeComponent();
            UpdateView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The log paramters displayed.
        /// </summary>
        public IList<LogParameters> LogParameters
        {
            get { return listViewLogParameters.ItemsSource as IList<LogParameters>; }
            set { listViewLogParameters.ItemsSource = value; }
        }

        /// <summary>
        /// The currently selected LogParameters or null if there isn't one currently selected.
        /// </summary>
        public LogParameters SelectedLogParameters
        {
            get { return listViewLogParameters.SelectedItem as LogParameters; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the view based on the current state.
        /// </summary>
        private void UpdateView()
        {
            if (SelectedLogParameters != null)
            {
                buttonViewLogs.IsEnabled = true;
                buttonDelete.IsEnabled = true;
                buttonEdit.IsEnabled = true;
            }
            else
            {
                buttonViewLogs.IsEnabled = false;
                buttonDelete.IsEnabled = false;
                buttonEdit.IsEnabled = false;
            }
        }

        /// <summary>
        /// Raises the DeleteClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnDeleteClick(EventArgs e)
        {
            if (DeleteClick != null)
                DeleteClick(this, e);
        }

        /// <summary>
        /// Raises the NewClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnNewClick(EventArgs e)
        {
            if (NewClick != null)
                NewClick(this, e);
        }

        /// <summary>
        /// Raises the ViewLogsClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnViewLogsClick(EventArgs e)
        {
            if (ViewLogsClick != null)
                ViewLogsClick(this, e);
        }

        /// <summary>
        /// Raises the EditClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnEditClick(EventArgs e)
        {
            if (EditClick != null)
                EditClick(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the ui display.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewLogParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        /// Closes the window.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
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
        /// Raises the NewClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnNewClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the ViewLogsClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonViewLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnViewLogsClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the ViewClick event when a user double clicks a log paramter.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewLogParameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedLogParameters != null)
                    OnViewLogsClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the EditClick event.
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

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks the delete button.
        /// </summary>
        public event EventHandler DeleteClick;

        /// <summary>
        /// Raised when the user clicks the new button.
        /// </summary>
        public event EventHandler NewClick;

        /// <summary>
        /// Raised when the user clicks the view logs button.
        /// </summary>
        public event EventHandler ViewLogsClick;

        /// <summary>
        /// Raised when the user clicks the edit button.
        /// </summary>
        public event EventHandler EditClick;

        #endregion
    }
}
