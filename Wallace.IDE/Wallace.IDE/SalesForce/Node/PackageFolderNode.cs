﻿/*
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
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Folder node for packages.
    /// </summary>
    public class PackageFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public PackageFolderNode(Project project)
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

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return "Package";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader("Package", "FolderClosed.png");
            Presenter.ExpandedHeader = VisualHelper.CreateIconHeader("Package", "FolderOpen.png");
        }

        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <returns>true.</returns>
        public override bool HasChildren()
        {
            return true;
        }

        /// <summary>
        /// Get the children for this node.
        /// </summary>
        /// <returns>The children for this node.</returns>
        public override INode[] GetChildren()
        {
            List<INode> result = new List<INode>();
            foreach (Package package in Project.GetPackages())
                result.Add(new PackageNode(Project, package));

            return result.ToArray();
        }

        /// <summary>
        /// Add a new node for the given package to this node.
        /// </summary>
        /// <param name="package">The package to add.</param>
        /// <returns>The newly added node.</returns>
        public PackageNode AddPackage(Package package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            Presenter.Expand();
            PackageNode packageNode = new PackageNode(Project, package);

            int index = 0;
            foreach (INode node in Presenter.Nodes)
            {
                if (node is PackageNode)
                {
                    int result = package.CompareTo((node as PackageNode).Package);
                    if (result == 0)
                    {
                        Presenter.NodeManager.ActiveNode = node;
                        return node as PackageNode; // node is already present
                    }
                    else if (result < 0)
                    {
                        break;
                    }
                }
                index++;
            }

            Presenter.Nodes.Insert(index, packageNode);            
            Presenter.NodeManager.ActiveNode = packageNode;

            return packageNode;
        }

        #endregion
    }
}
