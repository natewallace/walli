﻿/*
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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for PackageEditorControl.xaml
    /// </summary>
    public partial class PackageEditorControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public PackageEditorControl()
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

        #endregion

        #region Methods

        /// <summary>
        /// Raises the ListDragOver event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnListDragOver(DragEventArgs e)
        {
            if (ListDragOver != null)
                ListDragOver(this, e);
        }

        /// <summary>
        /// Raises the ListDrop event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnListDrop(DragEventArgs e)
        {
            if (ListDrop != null)
                ListDrop(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raise the ListDragOver event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewItems_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                OnListDragOver(e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the ListDrop event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewItems_Drop(object sender, DragEventArgs e)
        {
            try
            {
                OnListDrop(e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

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

        #region Events

        /// <summary>
        /// Raised when a DragOver event occurs on the list in this view.
        /// </summary>
        public event DragEventHandler ListDragOver;

        /// <summary>
        /// Raised when a Drop event occurs on the list in this view.
        /// </summary>
        public event DragEventHandler ListDrop;

        #endregion
    }
}
