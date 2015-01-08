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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements IDocumentManager using a TabControl.
    /// </summary>
    public class TabControlDocumentManager : HostBase<TabControl>, IDocumentManager
    {
        #region Fields

        /// <summary>
        /// Binding for the CloseTabItem command.
        /// </summary>
        private CommandBinding _closeTabItemBinding = null;

        /// <summary>
        /// Holds the item that was last selected.
        /// </summary>
        private object _lastSelectedItem = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabControlDocumentManager()
        {
            _closeTabItemBinding = new CommandBinding(UI.Commands.CloseTabItem, CloseTabItemCommandExecute);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        public TabControlDocumentManager(TabControl host)
            : this()
        {
            Host = host;            
        }

        #endregion        

        #region Methods

        /// <summary>
        /// Remove bindings from host.
        /// </summary>
        protected override void BeforeHostSet()
        {
            base.BeforeHostSet();

            if (Host != null)
            {
                Host.CommandBindings.Remove(_closeTabItemBinding);
                Host.SelectionChanged -= Host_SelectionChanged;
            }
        }

        /// <summary>
        /// Add bindings to host.
        /// </summary>
        protected override void AfterHostSet()
        {
            base.AfterHostSet();

            if (Host != null)
            {
                Host.CommandBindings.Add(_closeTabItemBinding);
                Host.SelectionChanged += Host_SelectionChanged;
            }
        }

        /// <summary>
        /// Get the TabItem that contains the given document.
        /// </summary>
        /// <param name="document">The document to search with.</param>
        /// <returns>The TabItem that contains the given document or null if it isn't found.</returns>
        private TabItem GetTabItem(IDocument document)
        {
            EnsureHost();

            if (document == null)
                return null;

            foreach (TabItem tabItem in Host.Items)
            {
                if (tabItem.Tag is IDocument && (tabItem.Tag as IDocument).Id == document.Id)
                    return tabItem;
            }

            return null;
        }

        /// <summary>
        /// Raises the ActiveDocumentChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnActiveDocumentChanged(EventArgs e)
        {
            if (ActiveDocumentChanged != null)
                ActiveDocumentChanged(this, e);
        }

        #endregion

        #region IDocumentManager Members

        /// <summary>
        /// Open a document.
        /// </summary>
        /// <param name="document">The document to open.</param>
        public void OpenDocument(IDocument document)
        {
            EnsureHost();

            if (document == null)
                throw new ArgumentNullException("document");

            // check to see if this document is already open in the host
            TabItem tabItem = GetTabItem(document);
            if (tabItem != null)
            {
                ActiveDocument = document;
            }
            // open a new document
            else
            {
                TabItem newTabItem = new TabItem();
                TabItemDocumentPresenter presenter = new TabItemDocumentPresenter(newTabItem);
                newTabItem.Tag = document;
                document.Init(presenter);
                document.Update(true);

                Host.Items.Insert(0, newTabItem);
                newTabItem.IsSelected = true;
                document.Opened();
            }
        }

        /// <summary>
        /// Close a document.
        /// </summary>
        /// <param name="document">The document to close.</param>
        /// <returns>true if the document was closed, false if it wasn't.</returns>
        public bool CloseDocument(IDocument document)
        {
            EnsureHost();

            if (document == null)
                throw new ArgumentNullException("document");

            TabItem tabItem = GetTabItem(document);
            if (tabItem != null)
            {
                if (document.Closing())
                {
                    Host.Items.Remove(tabItem);
                    document.Closed();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// All of the currently open documents.
        /// </summary>
        public IDocument[] OpenDocuments
        {
            get
            {
                EnsureHost();

                List<IDocument> documents = new List<IDocument>();
                foreach (TabItem tabItem in Host.Items)
                {
                    if (tabItem.Tag is IDocument)
                        documents.Add(tabItem.Tag as IDocument);
                }

                return documents.ToArray();
            }
        }

        /// <summary>
        /// The document that is currently being displayed or null if there isn't one.
        /// </summary>
        public IDocument ActiveDocument
        {
            get
            {
                EnsureHost();              

                TabItem tabItem = Host.SelectedItem as TabItem;
                if (tabItem == null || !(tabItem.Tag is IDocument))
                    return null;

                return tabItem.Tag as IDocument;
            }
            set
            {
                EnsureHost();

                if (value == null)
                {
                    Host.SelectedItem = null;
                }
                else
                {
                    TabItem tabItem = GetTabItem(value);
                    if (tabItem == null)
                    {
                        Host.SelectedItem = null;
                    }
                    else
                    {
                        int index = Host.Items.IndexOf(tabItem);
                        Host.Items.RemoveAt(index);
                        Host.Items.Insert(0, tabItem);
                        Host.SelectedItem = tabItem;
                    }
                }
            }
        }

        /// <summary>
        /// Close all open documents.
        /// </summary>
        /// <returns>true all documents were closed, false if they weren't.</returns>
        public bool CloseAllDocuments()
        {
            IDocument[] documents = OpenDocuments;
            foreach (IDocument document in documents)
                if (!CloseDocument(document))
                    return false;

            return true;
        }

        /// <summary>
        /// Get all documents that represent the given entity.
        /// </summary>
        /// <param name="entity">The entity to get documents for.</param>
        /// <returns>All documents that represent the given entity.</returns>
        public IDocument[] GetDocumentsByEntity(object entity)
        {
            List<IDocument> result = new List<IDocument>();
            foreach (IDocument document in OpenDocuments)
                if (document.RepresentsEntity(entity))
                    result.Add(document);

            return result.ToArray();
        }

        /// <summary>
        /// Raised when the active document is changed.
        /// </summary>
        public event EventHandler ActiveDocumentChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles execution of the TabItemClose command.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CloseTabItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                TabItem tabItem = VisualHelper.GetAncestor<TabItem>(e.OriginalSource as DependencyObject);
                if (tabItem != null && tabItem.Tag is IDocument)
                    CloseDocument(tabItem.Tag as IDocument);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the ActiveDocumentChanged event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_lastSelectedItem != Host.SelectedItem)
                {
                    _lastSelectedItem = Host.SelectedItem;

                    TabItem item = Host.SelectedItem as TabItem;
                    if (item != null && item.Tag is IDocument)
                        (item.Tag as IDocument).Activated();

                    OnActiveDocumentChanged(EventArgs.Empty);
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
