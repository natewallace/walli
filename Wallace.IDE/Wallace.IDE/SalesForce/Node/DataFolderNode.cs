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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Node that holds all data objects.
    /// </summary>
    public class DataFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public DataFolderNode(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader("Data", "FolderClosed.png");
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        /// <returns>true.</returns>
        public override bool HasChildren()
        {
            return true;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The resulting children.</returns>
        public override INode[] GetChildren()
        {
            List<INode> nodes = new List<INode>();

            var dataObjects = Project.Client.DataDescribeGlobal().OrderBy(dObj => dObj.Name);
            foreach (SObjectTypePartial dObj in dataObjects)
                nodes.Add(new DataObjectNode(Project, dObj));

            return nodes.ToArray();
        }

        #endregion
    }
}
