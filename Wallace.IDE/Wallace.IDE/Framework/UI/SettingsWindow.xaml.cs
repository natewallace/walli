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
using System.Windows.Shapes;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The currently selected path.
        /// </summary>
        public string SelectedSettingsPath
        {
            get
            {
                TreeViewItem item = treeViewCategories.SelectedItem as TreeViewItem;
                return (item != null) ? item.Tag as string : null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display the given view in the content area.
        /// </summary>
        /// <param name="view">The view to display.</param>
        public void ShowSettingsView(UIElement view)
        {
            borderContent.Content = view;
        }

        /// <summary>
        /// Registers a view for settings.
        /// </summary>
        /// <param name="path">The path for the view, separated by forward slashes.</param>
        /// <param name="view">The view to display for the given path.</param>
        public void AddSettingsPath(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is null or whitespace", "path");

            string[] parts = path.Split(new char[] { '/', '\\' });
            ItemCollection items = treeViewCategories.Items;
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                bool found = false;
                foreach (TreeViewItem item in items)
                {
                    if (String.Compare(item.Header as string, part, true) == 0)
                    {
                        items = item.Items;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = part;
                    item.IsExpanded = true;
                    items.Add(item);
                    items = item.Items;

                    if (i == parts.Length - 1)
                        item.Tag = path;
                }
            }
        }

        /// <summary>
        /// Raises the SelectedSettingsPathChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        public virtual void OnSelectedSettingsPathChanged(EventArgs e)
        {
            if (SelectedSettingsPathChanged != null)
                SelectedSettingsPathChanged(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raise the SelectedSettingsPathChanged event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void treeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                OnSelectedSettingsPathChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the dialog with a dialog result of true.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonOK_Click(object sender, RoutedEventArgs e)
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
        /// Close the dialog with a dialog result of false.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the selected settings path has been changed.
        /// </summary>
        public event EventHandler SelectedSettingsPathChanged;

        #endregion
    }
}
