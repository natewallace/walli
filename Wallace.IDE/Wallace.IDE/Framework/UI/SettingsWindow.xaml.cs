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
        #region Fields

        /// <summary>
        /// Holds the views that have been registered.
        /// </summary>
        private Dictionary<string, Control> _views;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();

            _views = new Dictionary<string, Control>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers a view for settings.
        /// </summary>
        /// <param name="path">The path for the view, separated by forward slashes.</param>
        /// <param name="view">The view to display for the given path.</param>
        public void RegisterSettingsView(string path, Control view)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is null or whitespace", "path");
            if (view == null)
                throw new ArgumentNullException("view");

            string[] parts = path.Split(new char[] { '/', '\\' });
            ItemCollection items = treeViewCategories.Items;
            foreach (string part in parts)
            {
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
                    item.Tag = path;
                    items.Add(item);
                    items = item.Items;
                }
            }

            _views.Add(path, view);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the view to show the currently selected category.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void treeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeViewItem item = treeViewCategories.SelectedItem as TreeViewItem;
                string path = (item != null) ? item.Tag as string : null;
                Control view = (path != null && _views.ContainsKey(path)) ? _views[path] : null;
                borderContent.Child = view;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
