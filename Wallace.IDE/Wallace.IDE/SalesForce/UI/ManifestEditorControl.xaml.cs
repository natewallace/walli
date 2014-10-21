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
using System.Windows;
using System.Windows.Controls;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for PackageEditorControl.xaml
    /// </summary>
    public partial class ManifestEditorControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManifestEditorControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A comment for the manifest.
        /// </summary>
        public string Comment
        {
            get { return textBoxComment.Text; }
            set { textBoxComment.Text = value; }
        }

        /// <summary>
        /// The currently selected items.
        /// </summary>
        public object[] SelectedItems
        {
            get
            {
                List<object> items = new List<object>();
                if (listViewItems.SelectedItems != null)
                    foreach (object item in listViewItems.SelectedItems)
                        items.Add(item);

                return items.ToArray();
            }
        }

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
        /// Raise the RemoveItemsClicked event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnRemoveItemsClicked(EventArgs e)
        {
            if (RemoveItemsClicked != null)
                RemoveItemsClicked(this, e);
        }

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

        /// <summary>
        /// Remove the currently selected items when delete or backspace is typed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewItems_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                    case System.Windows.Input.Key.Back:
                        OnRemoveItemsClicked(EventArgs.Empty);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Do a save when Ctrl+S is keyed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewItems_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
                {
                    switch (e.Key)
                    {
                        case System.Windows.Input.Key.S:
                            App.Instance.GetFunction<SaveManifestFunction>().Execute();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the CommentChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                OnCommentChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the CommentChanged event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnCommentChanged(EventArgs e)
        {
            if (CommentChanged != null)
                CommentChanged(this, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user has clicked to remove items.
        /// </summary>
        public event EventHandler RemoveItemsClicked;

        /// <summary>
        /// Raised when a DragOver event occurs on the list in this view.
        /// </summary>
        public event DragEventHandler ListDragOver;

        /// <summary>
        /// Raised when a Drop event occurs on the list in this view.
        /// </summary>
        public event DragEventHandler ListDrop;

        /// <summary>
        /// Raised when the comment has been changed.
        /// </summary>
        public event EventHandler CommentChanged;

        #endregion
    }
}
