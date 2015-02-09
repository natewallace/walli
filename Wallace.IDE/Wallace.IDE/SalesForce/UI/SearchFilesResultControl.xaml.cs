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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for SearchFilesResultControl.xaml
    /// </summary>
    public partial class SearchFilesResultControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchFilesResultControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The query text displayed.
        /// </summary>
        public string QueryText
        {
            get { return textBlockQuery.Text; }
            set { textBlockQuery.Text = value; }
        }

        /// <summary>
        /// The files displayed.
        /// </summary>
        public IEnumerable<SourceFile> Files
        {
            get { return dataGridFiles.ItemsSource as IEnumerable<SourceFile>; }
            set { dataGridFiles.ItemsSource = value; }
        }

        /// <summary>
        /// The currently selected file.
        /// </summary>
        public SourceFile SelectedFile
        {
            get { return dataGridFiles.SelectedItem as SourceFile; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the FileOpenClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnFileOpenClick(EventArgs e)
        {
            if (FileOpenClick != null)
                FileOpenClick(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raise FileOpenClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void dataGridFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedFile != null)
                    OnFileOpenClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user double clicks to open the file.
        /// </summary>
        public EventHandler FileOpenClick;

        #endregion
    }
}
