﻿/*
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for CommitHeaderListControl.xaml
    /// </summary>
    public partial class CommitHeaderListControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommitHeaderListControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The commits to display in a list.
        /// </summary>
        public IEnumerable<SimpleRepositoryCommit> Commits
        {
            get
            {
                if (listBoxItems.Tag == null)
                    return new SimpleRepositoryCommit[0];
                else
                    return listBoxItems.Tag as IEnumerable<SimpleRepositoryCommit>;
            }
            set
            {
                listBoxItems.Tag = value;

                listBoxItems.Items.Clear();
                if (value != null)
                {
                    foreach (SimpleRepositoryCommit commit in value)
                    {
                        CommitHeaderControl header = new CommitHeaderControl();
                        header.Comment = commit.Comment;
                        header.Author = commit.AuthorName;
                        header.Sha = commit.Sha;
                        header.Date = commit.Date;
                        header.Tag = commit;

                        listBoxItems.Items.Add(header);
                    }
                }
            }
        }

        /// <summary>
        /// Get the currently selected commits.
        /// </summary>
        public SimpleRepositoryCommit[] SelectedCommits
        {
            get
            {
                List<SimpleRepositoryCommit> result = new List<SimpleRepositoryCommit>();
                foreach (CommitHeaderControl header in listBoxItems.SelectedItems)
                    result.Add(header.Tag as SimpleRepositoryCommit);

                return result.ToArray();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the OpenClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnOpenClick(EventArgs e)
        {
            if (OpenClick != null)
                OpenClick(this, e);
        }

        /// <summary>
        /// Raises the CompareClick event.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected virtual void OnCompareClick(EventArgs e)
        {
            if (CompareClick != null)
                CompareClick(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the OpenClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void menuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnOpenClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the OpenClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedCommits.Length == 1)
                    OnOpenClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the CompareClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void menuItemCompare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCompareClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update menu items.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                menuItemCompare.IsEnabled = (SelectedCommits.Length == 2);
                menuItemOpen.IsEnabled = (SelectedCommits.Length == 1);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Change behavior of the right mouse button click.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxItems_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;

                CommitHeaderControl item = VisualHelper.GetAncestor<CommitHeaderControl>(e.OriginalSource as DependencyObject);
                if (item != null)
                {
                    int count = SelectedCommits.Length;
                    bool isSelected = listBoxItems.SelectedItems.Contains(item);

                    if (!isSelected)
                    {
                        if (count > 1)
                            listBoxItems.SelectedItems.Clear();

                        listBoxItems.SelectedItems.Add(item);
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
        /// Raised when the user clicks to open an entry.
        /// </summary>
        public event EventHandler OpenClick;

        /// <summary>
        /// Raised when the user clicks to compare two entries.
        /// </summary>
        public event EventHandler CompareClick;

        #endregion
    }
}
