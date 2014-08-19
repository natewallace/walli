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

using System.Collections.Generic;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Implements INode using default logic.
    /// </summary>
    public abstract class NodeBase : INode
    {
        #region Methods

        /// <summary>
        /// Merge two collections of functions together into one collection.
        /// </summary>
        /// <param name="top">The functions that will appear at the top of the merged collection.</param>
        /// <param name="bottom">The functions that will appear at the bottom of the merged collection.</param>
        /// <returns>The resulting merged collection.</returns>
        protected IFunction[] MergeFunctions(IEnumerable<IFunction> top, IEnumerable<IFunction> bottom)
        {
            List<IFunction> result = new List<IFunction>();

            if (top != null)
                foreach (IFunction function in top)
                    result.Add(function);

            if (bottom != null)
                foreach (IFunction function in bottom)
                    result.Add(function);

            return result.ToArray();
        }

        #endregion

        #region INode Members

        /// <summary>
        /// The presenter that will be used for this node.
        /// </summary>
        public virtual INodePresenter Presenter { get; set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public virtual string Text { get; protected set; }

        /// <summary>
        /// Called by the UI when the node is ready for display.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Called by the UI  when the node has been compeletly loaded.
        /// </summary>
        public virtual void Loaded()
        {
        }

        /// <summary>
        /// Called by the UI when the node should be updated.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// The UI will call this method when the node is double clicked.
        /// </summary>
        public virtual void DoubleClick()
        {
        }

        /// <summary>
        /// A drag operation has been started with this node.
        /// </summary>
        public virtual void DragStart()
        {
        }

        /// <summary>
        /// The UI will call this method to get items that should be displayed in a context menu for this node.
        /// </summary>
        /// <returns>The items that should be displayed in a context menu for this node.</returns>
        public virtual IFunction[] GetContextFunctions()
        {
            if (HasChildren())
            {
                return new IFunction[] 
                {
                    App.Instance.GetFunction<RefreshFolderFunction>()
                };
            }
            else
            {
                return new IFunction[0];
            }
        }

        /// <summary>
        /// Indicates if there are children or not.
        /// </summary>
        /// <returns>true if there are children, false if there are not.</returns>
        public virtual bool HasChildren()
        {
            return false;
        }

        /// <summary>
        /// Called by the UI to get the child nodes for this node.
        /// </summary>
        /// <returns>The child nodes of this node.</returns>
        public virtual INode[] GetChildren()
        {
            return null;
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public virtual bool RepresentsEntity(object entity)
        {
            return false;
        }

        #endregion
    }
}
