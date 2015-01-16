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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Interface to a node manager.
    /// </summary>
    public interface INodeManager
    {
        /// <summary>
        /// The root level nodes currently in the tree.
        /// </summary>
        Collection<INode> Nodes { get; }

        /// <summary>
        /// The currently selected nodes.
        /// </summary>
        Collection<INode> SelectedNodes { get; }

        /// <summary>
        /// Update all nodes.
        /// </summary>
        void UpdateNodes();

        /// <summary>
        /// The currently active node.
        /// </summary>
        INode ActiveNode { get; set; }

        /// <summary>
        /// Get the first node found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get.</typeparam>
        /// <returns>The first node found of the given type or null if one is not found.</returns>
        TType GetNode<TType>() where TType : INode;

        /// <summary>
        /// Get all nodes found with the given type.
        /// </summary>
        /// <typeparam name="TType">The type of node to get.</typeparam>
        /// <returns>All nodes found with the given type.</returns>
        IEnumerable<TType> GetNodes<TType>() where TType : INode;

        /// <summary>
        /// Get all nodes that represent the given entity.
        /// </summary>
        /// <param name="entity">The entity to get nodes for.</param>
        /// <returns>All nodes that represent the given entity.</returns>
        INode[] GetNodesByEntity(object entity);

        /// <summary>
        /// Raised when the active node is changed.
        /// </summary>
        event EventHandler ActiveNodeChanged;
    }
}
