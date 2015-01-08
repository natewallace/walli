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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// A node displayed in a tree type UI.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// The presenter that will be used for this node.
        /// </summary>
        INodePresenter Presenter { get; set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Called by the UI when the node is ready for display.
        /// </summary>
        void Init();

        /// <summary>
        /// Called by the UI  when the node has been compeletly loaded.
        /// </summary>
        void Loaded();

        /// <summary>
        /// Called by the UI when the node should be updated.
        /// </summary>
        void Update();

        /// <summary>
        /// The UI will call this method when the node is double clicked.
        /// </summary>
        void DoubleClick();

        /// <summary>
        /// A drag operation has been started with this node.
        /// </summary>
        void DragStart();

        /// <summary>
        /// This method is called when a drag drop item is dragged over this node.
        /// </summary>
        /// <param name="e">The drag drop item.</param>
        void DragOver(System.Windows.DragEventArgs e);

        /// <summary>
        /// This method is called when a drag drop item is dropped over this node.
        /// </summary>
        /// <param name="e">The drag drop item that was dropped.</param>
        void Drop(System.Windows.DragEventArgs e);

        /// <summary>
        /// The UI will call this method to get items that should be displayed in a context menu for this node.
        /// </summary>
        /// <returns>The items that should be displayed in a context menu for this node.</returns>
        IFunction[] GetContextFunctions();

        /// <summary>
        /// Indicates if there are children or not.
        /// </summary>
        /// <returns>true if there are children, false if there are not.</returns>
        bool HasChildren();

        /// <summary>
        /// Called by the UI to get the child nodes for this node.
        /// </summary>
        /// <returns>The child nodes of this node.</returns>
        INode[] GetChildren();

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        bool RepresentsEntity(object entity);
    }
}
