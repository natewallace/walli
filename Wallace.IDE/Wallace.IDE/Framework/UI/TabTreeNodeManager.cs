using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// A node manager that displays each root node in it's own tab.
    /// </summary>
    public class TabTreeNodeManager : INodeManager
    {
        #region Fields

        /// <summary>
        /// The tab control used to display the node manager.
        /// </summary>
        private TabControl _tabControl;

        /// <summary>
        /// Used by the SelectedNodes property when an empty collection is needed.
        /// </summary>
        private Collection<INode> _emptyCollection;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tabControl">The tab control to display nodes in.</param>
        public TabTreeNodeManager(TabControl tabControl)
        {
            if (tabControl == null)
                throw new ArgumentNullException("tabControl");

            _tabControl = tabControl;
            _tabControl.SelectionChanged += tabControl_SelectionChanged;
            _tabControl.CommandBindings.Add(new CommandBinding(UI.Commands.CloseTabItem, CloseTabItemCommandExecute));

            Nodes = new ObservableCollection<INode>();
            (Nodes as ObservableCollection<INode>).CollectionChanged += Nodes_CollectionChanged;

            _emptyCollection = new Collection<INode>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a new manager.
        /// </summary>
        /// <param name="node">The node to add a manager for.</param>
        /// <param name="index">The index to add the node to.</param>
        /// <returns>The newly added manager.</returns>
        private INodeManager AddManager(INode node, int index)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            TreeView view = new TreeView();
            TreeViewNodeManager manager = new TreeViewNodeManager(view);

            manager.Nodes.Add(node);
            manager.ActiveNodeChanged += manager_ActiveNodeChanged;

            TabItem item = new TabItem();
            item.Header = node.Text;
            item.Content = view;
            item.Tag = manager;
            ChromeTab.SetShowCloseButton(item, false);
            _tabControl.Items.Add(item);

            return manager;
        }

        /// <summary>
        /// Remove a manager.
        /// </summary>
        /// <param name="node">The node to remove the manager for.</param>
        private void RemoveManager(INode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            for (int i = 0; i < _tabControl.Items.Count; i++ )
            {
                INodeManager manager = (_tabControl.Items[i] as TabItem).Tag as INodeManager;
                if (manager.Nodes.Count > 0 && manager.Nodes[0] == node)
                {
                    manager.ActiveNodeChanged -= manager_ActiveNodeChanged;
                    _tabControl.Items.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Remove all managers.
        /// </summary>
        private void RemoveAllManagers()
        {
            List<INode> nodes = new List<INode>(Nodes);
            foreach (TabItem item in _tabControl.Items)
            {
                INodeManager manager = item.Tag as INodeManager;
                if (manager.Nodes.Count > 0)
                    nodes.Add(manager.Nodes[0]);
            }

            foreach (INode node in nodes)
                RemoveManager(node);
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
        /// The root level nodes, each of which will be it's own tab.
        /// </summary>
        public Collection<INode> Nodes { get; set; }

        /// <summary>
        /// The currently selected nodes.
        /// </summary>
        public Collection<INode> SelectedNodes 
        { 
            get
            {
                if (_tabControl.SelectedItem == null)
                {
                    _emptyCollection.Clear();
                    return _emptyCollection;
                }

                return ((_tabControl.SelectedItem as TabItem).Tag as INodeManager).SelectedNodes;
            }
        }

        /// <summary>
        /// Updates all nodes.
        /// </summary>
        public void UpdateNodes()
        {
            foreach (TabItem item in _tabControl.Items)
                (item.Tag as INodeManager).UpdateNodes();
        }

        /// <summary>
        /// The currently active node.
        /// </summary>
        public INode ActiveNode
        {
            get
            {
                TabItem item = _tabControl.SelectedItem as TabItem;
                if (item == null)
                    return null;

                return (item.Tag as INodeManager).ActiveNode;
            }
            set
            {
                foreach (TabItem item in _tabControl.Items)
                {
                    (item.Tag as INodeManager).ActiveNode = value;
                    if (value != null && (item.Tag as INodeManager).ActiveNode == value)
                    {
                        _tabControl.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the node with the given type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get,</typeparam>
        /// <returns>The node with the given type or null if it isn't found.</returns>
        public TType GetNode<TType>() where TType : INode
        {
            TType result = default(TType);
            foreach (TabItem item in _tabControl.Items)
            {
                result = (item.Tag as INodeManager).GetNode<TType>();
                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Get all nodes found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get.</typeparam>
        /// <returns>All nodes found with the given type.</returns>
        public IEnumerable<TType> GetNodes<TType>() where TType : INode
        {
            List<TType> result = new List<TType>();
            foreach (TabItem item in _tabControl.Items)
                result.AddRange((item.Tag as INodeManager).GetNodes<TType>());

            return result;
        }

        /// <summary>
        /// Get the node for the given entity.
        /// </summary>
        /// <param name="entity">The entity to get the corresponding node for.</param>
        /// <returns>The node for the given entity or null if it isn't found.</returns>
        public INode[] GetNodesByEntity(object entity)
        {
            List<INode> result = new List<INode>();
            foreach (TabItem item in _tabControl.Items)
            {
                result.AddRange((item.Tag as INodeManager).GetNodesByEntity(entity));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Raised when the active node is changed.
        /// </summary>
        public event EventHandler ActiveNodeChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the internal state to reflect the change.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

                        AddManager(node, addIndex);
                        addIndex++;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                        RemoveManager(item as INode);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RemoveAllManagers();
                    int resetIndex = 0;
                    foreach (INode node in Nodes)
                    {
                        AddManager(node, resetIndex);
                        resetIndex++;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                        RemoveManager(item as INode);

                    int replaceIndex = e.NewStartingIndex;
                    foreach (object item in e.NewItems)
                    {
                        INode node = item as INode;
                        if (node == null)
                            throw new Exception("Only INode objects can be added to the collection.");

                        AddManager(node, replaceIndex);
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
        /// Clear selections between tab changes.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_tabControl.SelectedItem != null)
                {
                    INodeManager manager = (_tabControl.SelectedItem as TabItem).Tag as INodeManager;
                    manager.SelectedNodes.Clear();
                    manager.ActiveNode = null;
                }
                OnActiveNodeChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the active node changed event when the active node changes.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void manager_ActiveNodeChanged(object sender, EventArgs e)
        {
            try
            {
                OnActiveNodeChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

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
                if (tabItem != null)
                {
                    INodeManager manager = tabItem.Tag as INodeManager;
                    if (manager.Nodes.Count > 0)
                        RemoveManager(manager.Nodes[0]);
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
