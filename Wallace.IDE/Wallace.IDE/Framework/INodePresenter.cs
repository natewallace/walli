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

using System.Collections.ObjectModel;
using System.Windows;

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Interface to object that displays a node.
    /// </summary>
    public interface INodePresenter
    {
        /// <summary>
        /// The node manager this presenter belongs to.
        /// </summary>
        INodeManager NodeManager { get; }

        /// <summary>
        /// The header for the node.
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// Expand the node so the children are displayed.
        /// </summary>
        void Expand();

        /// <summary>
        /// Collapse the node so children are not displayed.
        /// </summary>
        void Collapse();

        /// <summary>
        /// Start a drag drop operation.
        /// </summary>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the DragDropEffects values that specifies permitted effects of the drag-and-drop operation.</param>
        void DoDragDrop(object data, DragDropEffects allowedEffects);

        /// <summary>
        /// Start a drag drop operation.
        /// </summary>
        /// <param name="dataFormat">The format of the data.</param>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the DragDropEffects values that specifies permitted effects of the drag-and-drop operation.</param>
        void DoDragDrop(string dataFormat, object data, DragDropEffects allowedEffects);

        /// <summary>
        /// The nodes that are children to this node.
        /// </summary>
        Collection<INode> Nodes { get; }

        /// <summary>
        /// Refresh the nodes displayed under this node.
        /// </summary>
        void RefreshNodes();

        /// <summary>
        /// Removes this node from the tree.
        /// </summary>
        void Remove();
    }
}
