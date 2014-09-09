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
using System.Collections.Specialized;
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
    /// Interaction logic for PackageViewControl.xaml
    /// </summary>
    public partial class PackageViewControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PackageViewControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The items displayed.
        /// </summary>
        public System.Collections.IEnumerable ItemsSource
        {
            get
            {
                return listViewItems.ItemsSource;
            }
            set
            {
                if (listViewItems.ItemsSource is INotifyCollectionChanged)
                    (listViewItems.ItemsSource as INotifyCollectionChanged).CollectionChanged -= PackageEditorControl_CollectionChanged;

                listViewItems.ItemsSource = value;

                if (listViewItems.ItemsSource is INotifyCollectionChanged)
                    (listViewItems.ItemsSource as INotifyCollectionChanged).CollectionChanged += PackageEditorControl_CollectionChanged;
            }
        }

        /// <summary>
        /// The title in the view.
        /// </summary>
        public string ViewTitle
        {
            get { return textBlockTitle.Text; }
            set { textBlockTitle.Text = value; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update column width.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PackageEditorControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (listViewItems.View is GridView)
                {
                    GridView gridView = listViewItems.View as GridView;
                    if (gridView.Columns.Count > 0)
                    {
                        if (double.IsNaN(gridView.Columns[0].Width))
                            gridView.Columns[0].Width = gridView.Columns[0].ActualWidth;
                        gridView.Columns[0].Width = double.NaN;
                    }
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
