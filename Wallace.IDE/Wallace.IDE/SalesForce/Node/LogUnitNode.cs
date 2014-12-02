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
using SalesForceData;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Node that represents a log unit.
    /// </summary>
    public class LogUnitNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unit">Unit.</param>
        public LogUnitNode(LogUnit unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit");

            Unit = unit;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The log unit for this node.
        /// </summary>
        public LogUnit Unit { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            switch (Unit.EventType.ToUpper())
            {
                case "METHOD":
                case "CONSTRUCTOR":
                    Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "Method.png");
                    break;

                case "CODE_UNIT":
                case "EXECUTION":
                    Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "Namespace.png");
                    break;

                case "USER_DEBUG":
                    Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "Comment.png");
                    break;

                default:
                    if (Unit.EventType.ToUpper().Contains("ERROR"))
                    {
                        Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "Error.png");
                    }
                    else if (Unit.Units.Length > 0)
                    {
                        Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "FolderClosed.png");
                        Presenter.ExpandedHeader = VisualHelper.CreateIconHeader(Unit.ToString(), "FolderOpen.png");
                    }
                    else
                    {
                        Presenter.Header = VisualHelper.CreateIconHeader(Unit.ToString(), "Property.png");
                    }
                    break;
            }
        }

        /// <summary>
        /// Indicates if this node has children.
        /// </summary>
        /// <returns>true if there are sub units.</returns>
        public override bool HasChildren()
        {
            return (Unit.Units.Length > 0);
        }

        /// <summary>
        /// Get the children if there are any.
        /// </summary>
        /// <returns>The children of this node.</returns>
        public override INode[] GetChildren()
        {
            List<INode> result = new List<INode>();
            foreach (LogUnit child in Unit.Units)
                result.Add(new LogUnitNode(child));

            return result.ToArray();
        }

        #endregion
    }
}
