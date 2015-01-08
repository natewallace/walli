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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements INodePresenter with a TreeViewItem.
    /// </summary>
    public class TreeViewItemNodePresenter : HostBase<TreeViewItem>, INodePresenter
    {
        #region Fields

        /// <summary>
        /// Supports the Header property.
        /// </summary>
        private object _header;

        /// <summary>
        /// Supports the ExpandedHeader property.
        /// </summary>
        private object _expandedHeader;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        /// <param name="nodeManager">NodeManager.</param>
        public TreeViewItemNodePresenter(
            TreeViewItem host, 
            TreeViewNodeManager nodeManager, 
            INodePresenter parent)
        {
            Host = host;
            NodeManager = nodeManager;
            Parent = parent;

            _header = null;
            _expandedHeader = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parent of this presenter.
        /// </summary>
        private INodePresenter Parent { get; set; }

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
                if (Nodes != null)
                    (Nodes as ObservableCollection<INode>).CollectionChanged -= TreeViewItemNodePresenter_NodesChanged;

                Host.DragOver -= Host_DragOver;
                Host.Drop -= Host_Drop;
                
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
                Nodes = new ObservableCollection<INode>();
                (Nodes as ObservableCollection<INode>).CollectionChanged += TreeViewItemNodePresenter_NodesChanged;

                Host.DragOver += Host_DragOver;
                Host.Drop += Host_Drop;
            }
        }

        /// <summary>
        /// Updates the header displayed.
        /// </summary>
        public void UpdateHeader()
        {
            EnsureHost();
            if (Host.IsExpanded && ExpandedHeader != null)
                Host.Header = ExpandedHeader;
            else
                Host.Header = Header;
        }

        #endregion

        #region INodePresenter Members

        /// <summary>
        /// The node manager this presenter belongs to.
        /// </summary>
        public INodeManager NodeManager { get; set; }

        /// <summary>
        /// The header for the node.
        /// </summary>
        public object Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                UpdateHeader();
            }
        }

        /// <summary>
        /// The header to display when the node is expanded.  When null the Header is displayed.
        /// </summary>
        public object ExpandedHeader
        {
            get
            {
                return _expandedHeader;
            }
            set
            {
                _expandedHeader = value;
                UpdateHeader();
            }
        }

        /// <summary>
        /// Expand the node so the children are displayed.
        /// </summary>
        public void Expand()
        {
            EnsureHost();
            Host.IsExpanded = true;
        }

        /// <summary>
        /// Collapse the node so children are not displayed.
        /// </summary>
        public void Collapse()
        {
            EnsureHost();
            Host.IsExpanded = false;
        }

        /// <summary>
        /// Set to true if the node allows items to be dropped on it.
        /// </summary>
        public bool AllowDrop
        {
            get 
            { 
                return (Host == null) ? false : Host.AllowDrop; 
            }
            set
            {
                if (Host != null)
                    Host.AllowDrop = value;
            }
        }

        /// <summary>
        /// Start a drag drop operation.
        /// </summary>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the DragDropEffects values that specifies permitted effects of the drag-and-drop operation.</param>
        public void DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            EnsureHost();           
            DragDrop.DoDragDrop(Host, data, allowedEffects);
        }

        /// <summary>
        /// Start a drag drop operation.
        /// </summary>
        /// <param name="dataFormat">The format of the data.</param>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the DragDropEffects values that specifies permitted effects of the drag-and-drop operation.</param>
        public void DoDragDrop(string dataFormat, object data, DragDropEffects allowedEffects)
        {
            EnsureHost();
            DragDrop.DoDragDrop(Host, new DataObject(dataFormat, data), allowedEffects);
        }

        /// <summary>
        /// The root level nodes currently in the tree.
        /// </summary>
        public Collection<INode> Nodes { get; private set; }

        /// <summary>
        /// Refresh the nodes displayed under this node.
        /// </summary>
        public void RefreshNodes()
        {
            if (NodeManager is TreeViewNodeManager)
                (NodeManager as TreeViewNodeManager).ReloadNode(Host);
        }

        /// <summary>
        /// Removes this node from the tree.
        /// </summary>
        public void Remove()
        {
            if (Parent != null)
                Parent.Nodes.Remove(Host.Tag as INode);
            else
                NodeManager.Nodes.Remove(Host.Tag as INode);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sync changes to the Nodes collection with the tree.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event args.</param>
        private void TreeViewItemNodePresenter_NodesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TreeViewNodeManager nodeManager = NodeManager as TreeViewNodeManager;
            nodeManager.HandleNodeCollectionChange(Host, this, e);
        }

        /// <summary>
        /// Pass event to node.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (Host.Tag is INode)
                    (Host.Tag as INode).DragOver(e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Pass event to node.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Host_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (Host.Tag is INode)
                    (Host.Tag as INode).Drop(e);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
