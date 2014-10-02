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
using System.Collections;
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
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ReportControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReportControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The displayed report items.
        /// </summary>
        public IEnumerable<SourceFile> ReportItems
        {
            get 
            {
                List<SourceFile> result = new List<SourceFile>();

                IEnumerable<ReportItem> items = dataGridFiles.ItemsSource as IEnumerable<ReportItem>;
                if (items != null)
                    foreach (ReportItem item in items)
                        result.Add(item.File);

                return result; 
            }
            set 
            {
                if (value == null)
                {
                    dataGridFiles.ItemsSource = null;
                }
                else
                {
                    List<ReportItem> items = new List<ReportItem>();
                    foreach (SourceFile file in value)
                        items.Add(new ReportItem(file));

                    dataGridFiles.ItemsSource = items;
                }
            }
        }

        /// <summary>
        /// Get the currently selected report items.
        /// </summary>
        public IEnumerable<SourceFile> SelectedReportItems
        {
            get
            {
                List<SourceFile> result = new List<SourceFile>();

                IEnumerable<ReportItem> items = dataGridFiles.ItemsSource as IEnumerable<ReportItem>;
                if (items != null)
                {
                    dataGridFiles.CommitEdit(DataGridEditingUnit.Row, true);
                    foreach (ReportItem item in items)
                        if (item.IsSelected)
                            result.Add(item.File);
                }

                return result; 
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Enable editing on one click.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGridCell cell = sender as DataGridCell;
                if (cell != null && !cell.IsEditing)
                {
                    if (!cell.IsFocused)
                        cell.Focus();
                    if (!cell.IsSelected)
                        cell.IsSelected = true;
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
