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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements INodeManager with a TreeView.
    /// </summary>
    public class TreeViewNodeManager : HostBase<TreeView>, INodeManager
    {
        #region Fields

        /// <summary>
        /// Handler for tree view item expanded events.
        /// </summary>
        private RoutedEventHandler _treeViewItemExpandedHandler;

        /// <summary>
        /// Handler for tree view item collapse events.
        /// </summary>
        private RoutedEventHandler _treeViewItemCollapsedHandler;

        /// <summary>
        /// Used as a place holder for expandeding nodes.
        /// </summary>
        private object _nodePlaceHolder;

        /// <summary>
        /// The context menu which is displayed when a node is clicked on.
        /// </summary>
        private ContextMenu _contextMenu;

        /// <summary>
        /// The manager for the context menu.
        /// </summary>
        private MenuFunctionManager _contextMenuManager;

        /// <summary>
        /// Holds the point the mouse was last clicked to support drag operations.
        /// </summary>
        private Point? _lastMouseDownPoint;

        /// <summary>
        /// Holds the item that was last clicked on to support drag operations.
        /// </summary>
        private TreeViewItem _lastMouseDownTreeViewItem;

        /// <summary>
        /// Holds teh item that was last selected to support multi select operations.
        /// </summary>
        private TreeViewItem _lastSelectedTreeViewItem;

        /// <summary>
        /// When set to true selection change processing will not be done.
        /// </summary>
        private bool _suspendSelectionChange;

        /// <summary>
        /// This is used to capture a selection change for later processing.
        /// </summary>
        private RoutedPropertyChangedEventArgs<object> _delayedSelection;

        /// <summary>
        /// Used to reset the text entered by a user for a search after a certain period of time.
        /// </summary>
        private Timer _searchTimer;

        /// <summary>
        /// Text entered by user to do a search.
        /// </summary>
        private StringBuilder _searchTextBuilder;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TreeViewNodeManager()
        {
            _treeViewItemExpandedHandler = new RoutedEventHandler(TreeViewItem_Expanded);
            _treeViewItemCollapsedHandler = new RoutedEventHandler(TreeViewItem_Collapsed);
            _nodePlaceHolder = new object();
            _contextMenu = new ContextMenu();
            _contextMenu.Style = Application.Current.FindResource("ChromeContextMenuStyle") as Style;
            _contextMenuManager = new MenuFunctionManager(_contextMenu);
            _lastMouseDownPoint = null;
            _lastMouseDownTreeViewItem = null;
            _lastSelectedTreeViewItem = null;
            _suspendSelectionChange = false;
            _delayedSelection = null;

            _searchTimer = new Timer();
            _searchTimer.Interval = 750;
            _searchTimer.AutoReset = false;
            _searchTimer.Elapsed += searchTimer_Elapsed;

            _searchTextBuilder = new StringBuilder();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        public TreeViewNodeManager(TreeView host)
            : this()
        {
            Host = host;
        }

        #endregion

        #region Properties

        /// <summary>
        /// All currently selected items.
        /// </summary>
        private IList<TreeViewMultiSelectItem> SelectedItems
        {
            get
            {
                List<TreeViewMultiSelectItem> result = new List<TreeViewMultiSelectItem>();
                GetSelectedItems(null, result);
                return result;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Detach event handlers from current host.
        /// </summary>
        protected override void BeforeHostSet()
        {
            base.BeforeHostSet();

            if (Host != null)
            {
                Host.RemoveHandler(TreeViewItem.ExpandedEvent, _treeViewItemExpandedHandler);
                Host.RemoveHandler(TreeViewItem.CollapsedEvent, _treeViewItemCollapsedHandler);
                Host.MouseRightButtonDown -= Host_MouseRightButtonDown;
                Host.PreviewMouseLeftButtonDown -= Host_PreviewMouseLeftButtonDown;
                Host.PreviewMouseMove -= Host_PreviewMouseMove;
                Host.PreviewMouseUp -= Host_PreviewMouseUp;
                Host.SelectedItemChanged -= Host_SelectedItemChanged;
                Host.TextInput -= Host_TextInput;

                if (Nodes != null)
                    (Nodes as ObservableCollection<INode>).CollectionChanged -= TreeViewNodeManager_NodesChanged;

                if (SelectedNodes != null)
                    (SelectedNodes as ObservableCollection<INode>).CollectionChanged -= TreeViewNodeManager_SelectedNodesChanged;
            }
        }

        /// <summary>
        /// Attach event handlers to new host.
        /// </summary>
        protected override void AfterHostSet()
        {
            base.AfterHostSet();

            if (Host != null)
            {
                Host.AddHandler(TreeViewItem.ExpandedEvent, _treeViewItemExpandedHandler);
                Host.AddHandler(TreeViewItem.CollapsedEvent, _treeViewItemCollapsedHandler);
                Host.MouseDoubleClick += Host_MouseDoubleClick;
                Host.MouseRightButtonDown += Host_MouseRightButtonDown;
                Host.PreviewMouseLeftButtonDown += Host_PreviewMouseLeftButtonDown;
                Host.PreviewMouseMove += Host_PreviewMouseMove;
                Host.PreviewMouseUp += Host_PreviewMouseUp;
                Host.SelectedItemChanged += Host_SelectedItemChanged;
                Host.TextInput += Host_TextInput;

                Nodes = new ObservableCollection<INode>();
                (Nodes as ObservableCollection<INode>).CollectionChanged += TreeViewNodeManager_NodesChanged;

                SelectedNodes = new ObservableCollection<INode>();
                (SelectedNodes as ObservableCollection<INode>).CollectionChanged += TreeViewNodeManager_SelectedNodesChanged;
            }
        }

        /// <summary>
        /// Add a node to the given node within the tree.
        /// </summary>
        /// <param name="index">The index to insert the node at.</param>
        /// <param name="node">The node to add.</param>
        /// <param name="parent">The node to add the new node to or null to add a root node.</param>
        public void InsertNode(int index, INode node, TreeViewItem parent)
        {
            EnsureHost();

            if (node == null)
                throw new ArgumentNullException("node");

            TreeViewMultiSelectItem item = new TreeViewMultiSelectItem();
            item.Tag = node;
            node.Presenter = (parent == null) ?
                new TreeViewItemNodePresenter(item, this, null) :
                new TreeViewItemNodePresenter(item, this, (parent.Tag as INode).Presenter);
            node.Init();
            node.Update();
            ProcessNode(item);

            if (parent == null)
                Host.Items.Insert(index, item);
            else
                parent.Items.Insert(index, item);

            node.Loaded();
        }

        /// <summary>
        /// Add a root node to the tree.
        /// </summary>
        /// <param name="index">The index to insert the node at.</param>
        /// <param name="node">The node to add.</param>
        public void InsertNode(int index, INode node)
        {
            InsertNode(index, node, null);
        }

        /// <summary>
        /// Remove the node at the given index from the given node.
        /// </summary>
        /// <param name="index">The index of the node to remove.</param>
        /// <param name="parent">The parent to remove the node from or null to remove a root node.</param>
        public void RemoveNode(int index, TreeViewItem parent)
        {
            EnsureHost();

            TreeViewItem item = null;

            if (parent == null)
            {
                item = Host.Items[index] as TreeViewItem;
                Host.Items.RemoveAt(index);
            }
            else
            {
                item = parent.Items[index] as TreeViewItem;
                parent.Items.RemoveAt(index);
            }

            if (item != null)
                UnselectNodes(item);
        }

        /// <summary>
        /// Unselect any nodes that are in or below the item in the tree.
        /// </summary>
        /// <param name="item">The item to unselect nodes for.</param>
        private void UnselectNodes(TreeViewItem item)
        {
            if (item.Tag is INode)
                SelectedNodes.Remove(item.Tag as INode);

            foreach (object child in item.Items)
                if (item is TreeViewItem)
                    UnselectNodes(child as TreeViewItem);
        }

        /// <summary>
        /// Remove the root node at the given index.
        /// </summary>
        /// <param name="index">The index of the node to remove.</param>
        public void RemoveNode(int index)
        {
            RemoveNode(index, null);
        }

        /// <summary>
        /// Remove all root nodes from the given node.
        /// </summary>
        /// <param name="parent">The parent to remove the nodes from or null to remove root nodes.</param>
        public void RemoveAllNodes(TreeViewItem parent)
        {
            EnsureHost();

            if (parent == null)
                while (Host.Items.Count > 0)
                    RemoveNode(0);
            else
                while (parent.Items.Count > 0)
                    RemoveNode(0, parent);
        }

        /// <summary>
        /// Remove all root nodes from the tree.
        /// </summary>
        public void RemoveAllNodes()
        {
            RemoveAllNodes(null);
        }

        /// <summary>
        /// Gets all of the selected items.  This method will recurse from the item down.
        /// </summary>
        /// <param name="item">The item to start the search at.  If null the search starts at the root of the tree.</param>
        /// <param name="result">The list that holds the results.</param>
        private void GetSelectedItems(TreeViewItem item, IList<TreeViewMultiSelectItem> result)
        {
            if (item == null)
            {
                foreach (TreeViewItem child in Host.Items)
                    GetSelectedItems(child, result);
            }
            else
            {
                if (item is TreeViewMultiSelectItem && (item as TreeViewMultiSelectItem).IsMultiSelected)
                    result.Add(item as TreeViewMultiSelectItem);

                foreach (TreeViewItem child in item.Items)
                    GetSelectedItems(child, result);
            }
        }

        /// <summary>
        /// Clear all multi-selected items.
        /// </summary>
        private void ClearMultiSelect()
        {
            foreach (TreeViewMultiSelectItem item in SelectedItems)
                item.IsMultiSelected = false;
        }

        /// <summary>
        /// Get the items between the two given items.
        /// </summary>
        /// <param name="itemOne">One of the items that marks the boundary.</param>
        /// <param name="itemTwo">The other item that marks the boundary.</param>
        /// <returns>The items that are between the to given parameters inclusive.</returns>
        private IList<TreeViewMultiSelectItem> GetItemsInbetween(TreeViewMultiSelectItem itemOne, TreeViewMultiSelectItem itemTwo)
        {
            List<TreeViewMultiSelectItem> result = new List<TreeViewMultiSelectItem>();

            if (itemOne == itemTwo)
            {
                result.Add(itemOne);
                return result;
            }

            foreach (TreeViewItem item in Host.Items)
            {
                if (item is TreeViewMultiSelectItem)
                    if (GetItemsInbetween(itemOne, itemTwo, item as TreeViewMultiSelectItem, result))
                        break;
            }

            return result;
        }

        /// <summary>
        /// Recursive method that gets all the items between the two given items.
        /// </summary>
        /// <param name="itemOne">One of the items that marks the boundary.</param>
        /// <param name="itemTwo">The other item that marks the boundary.</param>
        /// <param name="current">The current item in the evaluation.</param>
        /// <param name="result">The list that holds the results.</param>
        /// <returns>true if the last item in the list has been found, false if it hasn't been found yet.</returns>
        private bool GetItemsInbetween(
            TreeViewMultiSelectItem itemOne, 
            TreeViewMultiSelectItem itemTwo, 
            TreeViewMultiSelectItem current, 
            IList<TreeViewMultiSelectItem> result)
        {
            if (current == null)
                return false;

            if (current == itemOne || current == itemTwo)
            {                
                result.Add(current);
                if (result.Count > 1)
                    return true;
            }
            else
            {
                if (result.Count > 0)
                    result.Add(current);
            }

            if (current.IsExpanded)
            {
                foreach (TreeViewItem item in current.Items)
                {
                    if (item is TreeViewMultiSelectItem)
                        if (GetItemsInbetween(itemOne, itemTwo, item as TreeViewMultiSelectItem, result))
                            return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the item for the given node.
        /// </summary>
        /// <param name="item">The current item being searched.  If null the search starts at the root level.</param>
        /// <param name="node">The node to look for.</param>
        /// <returns>The item if found or null if not found.</returns>
        public TreeViewItem GetItem(TreeViewItem item, INode node)
        {
            if (item == null)
            {
                foreach (object child in Host.Items)
                {
                    if (child is TreeViewItem)
                    {
                        TreeViewItem result = GetItem(child as TreeViewItem, node);
                        if (result != null)
                            return result;
                    }
                }
            }
            else
            {
                if (item.Tag == node)
                    return item;

                foreach (object child in item.Items)
                {
                    if (child is TreeViewItem)
                    {
                        TreeViewItem result = GetItem(child as TreeViewItem, node);
                        if (result != null)
                            return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recursive call that gets nodes by entity.
        /// </summary>
        /// <param name="item">The current item.</param>
        /// <param name="entity">The entity to check for.</param>
        /// <param name="nodes">The list to add the matching nodes to.</param>
        private void GetNodesByEntity(TreeViewItem item, object entity, IList<INode> nodes)
        {
            if (item.Tag is INode && (item.Tag as INode).RepresentsEntity(entity))
                nodes.Add(item.Tag as INode);

            foreach (object child in item.Items)
                if (child is TreeViewItem)
                    GetNodesByEntity(child as TreeViewItem, entity, nodes);
        }

        /// <summary>
        /// Get the item for the given node type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get.</typeparam>
        /// <param name="item">The current item being searched.  If null the search starts at the root level.</param>
        /// <returns>The item if found or null if not found.</returns>
        public TType GetItem<TType>(TreeViewItem item) where TType : INode
        {
            if (item == null)
            {
                foreach (object child in Host.Items)
                {
                    if (child is TreeViewItem)
                    {
                        TType result = GetItem<TType>(child as TreeViewItem);
                        if (result != null)
                            return result;
                    }
                }
            }
            else
            {
                if (item.Tag is TType)
                    return (TType)item.Tag;

                foreach (object child in item.Items)
                {
                    if (child is TreeViewItem)
                    {
                        TType result = GetItem<TType>(child as TreeViewItem);
                        if (result != null)
                            return result;
                    }
                }
            }

            return default(TType);
        }

        /// <summary>
        /// Get the tree view item associated with the given node.
        /// </summary>
        /// <param name="node">The node to get the tree view item for.</param>
        /// <returns>The tree view item for the given node or null if one isn't found.</returns>
        private TreeViewItem GetTreeViewItem(INode node)
        {
            EnsureHost();

            foreach (TreeViewItem item in Host.Items)
                if (item.Tag is INode && (item.Tag as INode) == node)
                    return item;

            return null;
        }

        /// <summary>
        /// Reload the node.
        /// </summary>
        /// <param name="item">The node to reload.</param>
        public void ReloadNode(TreeViewItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Tag is INode)
                (item.Tag as INode).Presenter.Nodes.Clear();

            item.Items.Add(new TreeViewItem()
            {
                Header = "reloading...",
                Tag = _nodePlaceHolder
            });

            ProcessNode(item);
        }

        /// <summary>
        /// Process a node that is being displayed.
        /// </summary>
        /// <param name="item">The tree view item that is displaying the node.</param>
        private void ProcessNode(TreeViewItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            using (App.Wait(null))
            {
                INode node = item.Tag as INode;
                if (node == null)
                    throw new Exception("Node is missing from TreeViewItem.Tag");

                if (!item.IsExpanded)
                {
                    // add place holder if there are children
                    if (node.HasChildren() && item.Items.Count == 0)
                    {
                        TreeViewItem placeHolder = new TreeViewItem();
                        placeHolder.Header = "loading...";
                        placeHolder.Tag = _nodePlaceHolder;
                        item.Items.Add(placeHolder);
                    }
                }
                else if (node.HasChildren())
                {
                    // get children
                    if (item.Items.Count == 1 && (item.Items[0] as TreeViewItem).Tag == _nodePlaceHolder)
                    {
                        try
                        {
                            INode[] children = node.GetChildren();
                            item.Items.Clear();

                            if (children != null)
                            {
                                foreach (INode childNode in children)
                                    node.Presenter.Nodes.Add(childNode);
                            }
                        }
                        catch
                        {
                            item.IsExpanded = false;
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle a change to a collection of nodes.
        /// </summary>
        /// <param name="parentItem">The parent item that owns the collection or null if it's the root node.</param>
        /// <param name="presenter">The presenter that owns the collection or null if it's the root node.</param>
        /// <param name="e">The change event arguments.</param>
        public void HandleNodeCollectionChange(TreeViewItem parentItem, INodePresenter presenter, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int addIndex = e.NewStartingIndex;
                    foreach (object item in e.NewItems)
                    {
                        INode node = item as INode;
                        if (node == null)
                            throw new Exception("Only INode objects can be added to the collection.");

                        InsertNode(addIndex, node, parentItem);
                        addIndex++;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                        RemoveNode(e.OldStartingIndex, parentItem);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RemoveAllNodes(parentItem);
                    int resetIndex = 0;
                    IEnumerable<INode> nodes = (presenter == null) ? Nodes : presenter.Nodes;
                    foreach (INode node in nodes)
                    {
                        InsertNode(resetIndex, node);
                        resetIndex++;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                        RemoveNode(e.OldStartingIndex, parentItem);

                    int replaceIndex = e.NewStartingIndex;
                    foreach (object item in e.NewItems)
                    {
                        INode node = item as INode;
                        if (node == null)
                            throw new Exception("Only INode objects can be added to the collection.");

                        InsertNode(replaceIndex, node, parentItem);
                        replaceIndex++;
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    // ignore.  this is processed as a remove and then add
                    break;

                default:
                    throw new Exception("Unsupported action: " + e.Action);
            }
        }

        /// <summary>
        /// Recursively go through tree and call Update method on nodes.
        /// </summary>
        /// <param name="item">The item to recurse from.  If null then start from the root of the tree.</param>
        private void UpdateNodes(TreeViewItem item)
        {
            if (item == null)
            {
                foreach (object child in Host.Items)
                {
                    if (child is TreeViewItem)
                        UpdateNodes(child as TreeViewItem);
                }
            }
            else
            {
                if (item.Tag is INode)
                    (item.Tag as INode).Update();

                foreach (object child in item.Items)
                {
                    if (child is TreeViewItem)
                        UpdateNodes(child as TreeViewItem);
                }
            }
        }

        /// <summary>
        /// Performs search for node with the given text from the currently selected node.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        private void SearchNodes(string text)
        {
            bool start = false;
            foreach (TreeViewItem item in Host.Items)
                if (SearchNodes(text, item, ref start))
                    break;
        }

        /// <summary>
        /// Recursive search through visible nodes.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="item">The item to search.</param>
        /// <param name="start">When set to true the search will start in the recursion.</param>
        /// <returns>true if a match was found, false if not.</returns>
        private bool SearchNodes(string text, TreeViewItem item, ref bool start)
        {
            if (item == null)
                return false;

            if (!start && item == Host.SelectedItem)
                start = true;

            if (item.Tag is INode)
            {
                INode node = item.Tag as INode;
                if (start && node.Text != null && node.Text.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    item.IsSelected = true;
                    return true;
                }
            }

            if (item.IsExpanded)
            {
                foreach (TreeViewItem i in item.Items)
                    if (SearchNodes(text, i, ref start))
                        return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the ActiveNodeChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnActiveNodeChanged(EventArgs e)
        {
            if (ActiveNodeChanged != null)
                ActiveNodeChanged(this, e);
        }

        #endregion

        #region INodeManager Members

        /// <summary>
        /// The root level nodes currently in the tree.
        /// </summary>
        public Collection<INode> Nodes { get; private set; }

        /// <summary>
        /// The currently selected nodes.
        /// </summary>
        public Collection<INode> SelectedNodes { get; private set; }

        /// <summary>
        /// The currently active node.
        /// </summary>
        public INode ActiveNode
        {
            get
            {
                TreeViewItem item = Host.SelectedItem as TreeViewItem;
                if (item == null)
                    return null;

                return item.Tag as INode;
            }
            set
            {
                ClearMultiSelect();
                if (value != null)
                {
                    TreeViewItem item = GetItem(null, value);
                    if (item != null)
                        item.IsSelected = true;
                    else if (Host.SelectedItem is TreeViewItem)
                        (Host.SelectedItem as TreeViewItem).IsSelected = false;
                }
                else
                {
                    if (Host.SelectedItem != null)
                        (Host.SelectedItem as TreeViewItem).IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Update all nodes.
        /// </summary>
        public void UpdateNodes()
        {
            UpdateNodes(null);
        }

        /// <summary>
        /// Get the first node found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get.</typeparam>
        /// <returns>The first node found of the given type or null if one is not found.</returns>
        public TType GetNode<TType>() where TType : INode
        {
            return GetItem<TType>(null);
        }

        /// <summary>
        /// Get all nodes that represent the given entity.
        /// </summary>
        /// <param name="entity">The entity to get nodes for.</param>
        /// <returns>All nodes that represent the given entity.</returns>
        public INode[] GetNodesByEntity(object entity)
        {
            List<INode> result = new List<INode>();
            foreach (object item in Host.Items)
                if (item is TreeViewItem)
                    GetNodesByEntity(item as TreeViewItem, entity, result);

            return result.ToArray();
        }

        /// <summary>
        /// Raised when the active node is changed.
        /// </summary>
        public event EventHandler ActiveNodeChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sync changes to the Nodes collection with the tree.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void TreeViewNodeManager_NodesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                HandleNodeCollectionChange(null, null, e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Sync changes to the SelectedNodes collection with the tree.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void TreeViewNodeManager_SelectedNodesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {           
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (object item in e.NewItems)
                        {
                            if (item is INode && 
                                (item as INode).Presenter is TreeViewItemNodePresenter &&
                                ((item as INode).Presenter as TreeViewItemNodePresenter).Host is TreeViewMultiSelectItem)
                                (((item as INode).Presenter as TreeViewItemNodePresenter).Host as TreeViewMultiSelectItem).IsMultiSelected = true;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (object item in e.OldItems)
                        {
                            if (item is INode &&
                                (item as INode).Presenter is TreeViewItemNodePresenter &&
                                ((item as INode).Presenter as TreeViewItemNodePresenter).Host is TreeViewMultiSelectItem)
                                (((item as INode).Presenter as TreeViewItemNodePresenter).Host as TreeViewMultiSelectItem).IsMultiSelected = false;
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (object item in e.NewItems)
                        {
                            if (item is INode &&
                                (item as INode).Presenter is TreeViewItemNodePresenter &&
                                ((item as INode).Presenter as TreeViewItemNodePresenter).Host is TreeViewMultiSelectItem)
                                (((item as INode).Presenter as TreeViewItemNodePresenter).Host as TreeViewMultiSelectItem).IsMultiSelected = true;
                        }
                        foreach (object item in e.OldItems)
                        {
                            if (item is INode &&
                                (item as INode).Presenter is TreeViewItemNodePresenter &&
                                ((item as INode).Presenter as TreeViewItemNodePresenter).Host is TreeViewMultiSelectItem)
                                (((item as INode).Presenter as TreeViewItemNodePresenter).Host as TreeViewMultiSelectItem).IsMultiSelected = false;
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        ClearMultiSelect();
                        foreach (INode node in SelectedNodes)
                        {
                            if (node.Presenter is TreeViewItemNodePresenter &&
                                (node.Presenter as TreeViewItemNodePresenter).Host is TreeViewMultiSelectItem)
                                ((node.Presenter as TreeViewItemNodePresenter).Host as TreeViewMultiSelectItem).IsMultiSelected = true;
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        break;

                    default:
                        throw new Exception("Unsupported action: " + e.Action);
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Process a node that has been expanded.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessNode(e.Source as TreeViewItem);

                TreeViewItem item = e.Source as TreeViewItem;
                if (item != null && item.Tag is INode)
                {
                    INode node = item.Tag as INode;
                    if (node.Presenter is TreeViewItemNodePresenter)
                        (node.Presenter as TreeViewItemNodePresenter).UpdateHeader();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Process a node that has been expanded.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = e.Source as TreeViewItem;
                if (item != null && item.Tag is INode)
                {
                    INode node = item.Tag as INode;
                    if (node.Presenter is TreeViewItemNodePresenter)
                        (node.Presenter as TreeViewItemNodePresenter).UpdateHeader();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Route double click on a node.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void Host_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItem item = VisualHelper.GetAncestor<TreeViewItem>(e.OriginalSource as DependencyObject);
                bool isExpandButton = (VisualHelper.GetAncestor<System.Windows.Controls.Primitives.ToggleButton>(e.OriginalSource as DependencyObject) != null);

                if (!isExpandButton && item != null && item.Tag is INode)
                    (item.Tag as INode).DoubleClick();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Add support for multi select.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (_suspendSelectionChange)
                {
                    _delayedSelection = e;
                    return;
                }

                TreeViewMultiSelectItem item = Host.SelectedItem as TreeViewMultiSelectItem;
                if (item == null)
                    return;

                INode node = item.Tag as INode;
                if (node == null)
                    return;

                // process multi select
                switch (System.Windows.Input.Keyboard.Modifiers)
                {
                    case System.Windows.Input.ModifierKeys.Control:
                        if (item.IsMultiSelected)
                            SelectedNodes.Remove(node);
                        else
                            SelectedNodes.Add(node);

                        _lastSelectedTreeViewItem = item;
                        break;

                    case System.Windows.Input.ModifierKeys.Shift:
                        if (_lastSelectedTreeViewItem is TreeViewMultiSelectItem)
                        {
                            IList<TreeViewMultiSelectItem> items = GetItemsInbetween(
                                _lastSelectedTreeViewItem as TreeViewMultiSelectItem,
                                item);

                            SelectedNodes.Clear();
                            foreach (TreeViewMultiSelectItem i in items)
                                if (i.Tag is INode)
                                    SelectedNodes.Add(i.Tag as INode);
                        }
                        else
                        {
                            SelectedNodes.Add(node);
                        }
                        break;

                    default:                        
                        SelectedNodes.Clear();
                        SelectedNodes.Add(node);
                        _lastSelectedTreeViewItem = item;
                        break;
                }

                OnActiveNodeChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Display context menu for the node that was clicked.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                _contextMenu.Items.Clear();
                TreeViewItem item = VisualHelper.GetAncestor<TreeViewItem>(e.OriginalSource as DependencyObject);
                bool isExpandButton = (VisualHelper.GetAncestor<System.Windows.Controls.Primitives.ToggleButton>(e.OriginalSource as DependencyObject) != null);

                if (!isExpandButton && item != null && item.Tag is INode)
                {
                    item.IsSelected = true;

                    INode node = item.Tag as INode;
                    IFunction[] menuItems = node.GetContextFunctions();
                    if (menuItems != null && menuItems.Length > 0)
                    {
                        foreach (IFunction menuItem in menuItems)
                            _contextMenuManager.AddFunction(menuItem);

                        _contextMenu.PlacementTarget = item;
                        _contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        _contextMenu.IsOpen = true;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Capture where the mouse down occured to support drag operations and multi-select.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Control control = sender as Control;
                if (control != null)
                {
                    TreeViewMultiSelectItem item = VisualHelper.GetAncestor<TreeViewMultiSelectItem>(e.OriginalSource as DependencyObject);
                    bool isExpandButton = (VisualHelper.GetAncestor<System.Windows.Controls.Primitives.ToggleButton>(e.OriginalSource as DependencyObject) != null);

                    if (item == null)
                    {
                        _lastMouseDownPoint = null;
                        _lastMouseDownTreeViewItem = null;
                    }
                    else
                    {
                        if (item.IsMultiSelected)
                        {
                            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.None)
                            {
                                _suspendSelectionChange = true;
                            }
                            else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
                            {
                                e.Handled = true;
                                item.IsMultiSelected = false;
                                item.IsSelected = false;
                            }
                        }

                        _lastMouseDownPoint = e.GetPosition(control);
                        _lastMouseDownTreeViewItem = item;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Capture the mouse move to start a drag operation.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                Control control = sender as Control;
                if (control != null &&
                    _lastMouseDownPoint.HasValue &&
                    _lastMouseDownTreeViewItem != null)
                {
                    Point centeredBoxPoint = new Point(
                        _lastMouseDownPoint.Value.X - (SystemParameters.MinimumHorizontalDragDistance / 2),
                        _lastMouseDownPoint.Value.Y - (SystemParameters.MinimumVerticalDragDistance / 2));

                    Rect dragBox = new Rect(
                        centeredBoxPoint,
                        new Size(
                            SystemParameters.MinimumHorizontalDragDistance,
                            SystemParameters.MinimumVerticalDragDistance));

                    Point currentPoint = e.GetPosition(control);

                    if (!dragBox.Contains(currentPoint))
                    {
                        INode node = _lastMouseDownTreeViewItem.Tag as INode;
                        if (node != null)
                            node.DragStart();

                        _lastMouseDownPoint = null;
                        _lastMouseDownTreeViewItem = null;
                        _suspendSelectionChange = false;
                        _delayedSelection = null;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Stop any active drag operation.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (_suspendSelectionChange)
                {
                    _suspendSelectionChange = false;
                    Host_SelectedItemChanged(null, _delayedSelection);
                    _delayedSelection = null;
                }

                _lastMouseDownPoint = null;
                _lastMouseDownTreeViewItem = null;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Perform search using text entered by user or execute double click if a carriage return is typed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                if (e.Text == "\r")
                {
                    if (ActiveNode != null)
                        ActiveNode.DoubleClick();
                }
                else
                {
                    _searchTimer.Stop();
                    _searchTextBuilder.Append(e.Text);
                    SearchNodes(_searchTextBuilder.ToString());
                    _searchTimer.Start();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Reset the search text.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void searchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Host != null)
            {
                Host.Dispatcher.Invoke(() => _searchTextBuilder.Clear());
            }
        }

        #endregion
    }
}
