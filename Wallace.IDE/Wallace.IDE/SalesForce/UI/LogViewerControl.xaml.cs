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
using ICSharpCode.AvalonEdit.Search;
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for LogViewerControl.xaml
    /// </summary>
    public partial class LogViewerControl : UserControl
    {
        #region Fields

        /// <summary>
        /// The text search panel.
        /// </summary>
        private SearchPanel _searchPanel;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogViewerControl()
        {
            InitializeComponent();

            _searchPanel = SearchPanel.Install(textEditor.TextArea);
            _searchPanel.MarkerBrush = Brushes.DarkOrange;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The traced entity.
        /// </summary>
        public string TracedEntity
        {
            get { return textBlockEntity.Text; }
            set { textBlockEntity.Text = value; }
        }

        /// <summary>
        /// The displayed logs.
        /// </summary>
        public IEnumerable<Log> Logs
        {
            get
            {
                List<Log> result = new List<Log>();

                IEnumerable<Log> items = dataGridLogs.ItemsSource as IEnumerable<Log>;
                if (items != null)
                    foreach (Log item in items)
                        result.Add(item);

                return result;
            }
            set
            {
                if (value == null)
                {
                    dataGridLogs.ItemsSource = null;
                }
                else
                {
                    List<Log> items = new List<Log>();
                    foreach (Log log in value)
                        items.Add(log);

                    dataGridLogs.ItemsSource = items;
                }
            }
        }

        /// <summary>
        /// The currently selected log.
        /// </summary>
        public Log SelectedLog
        {
            get
            {
                return dataGridLogs.SelectedItem as Log; 
            }
        }

        /// <summary>
        /// The log content displayed.
        /// </summary>
        public string LogContentText
        {
            get { return textEditor.Text; }
            set { textEditor.Text = value; }
        }

        /// <summary>
        /// The tree view for log content data.
        /// </summary>
        public TreeView LogContentData
        {
            get { return treeViewData; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Open the text search dialog.
        /// </summary>
        public void SearchText()
        {
            _searchPanel.Open();

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                (Action)delegate { _searchPanel.Reactivate(); });
        }

        /// <summary>
        /// Raises the SelectedLogChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnSelectedLogChanged(EventArgs e)
        {
            if (SelectedLogChanged != null)
                SelectedLogChanged(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the SelectedLogChanged event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void dataGridLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OnSelectedLogChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the selected log has changed.
        /// </summary>
        public event EventHandler SelectedLogChanged;

        #endregion
    }
}
