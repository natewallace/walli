﻿/*
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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Node that holds local objects.
    /// </summary>
    public class LocalFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public LocalFolderNode(Project project)
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
                return "Local ";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader("Local", "FolderClosed.png");
            Presenter.ExpandedHeader = VisualHelper.CreateIconHeader("Local", "FolderOpen.png");
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
            return new INode[] { 
                new ManifestFolderNode(Project), 
                new PackageFolderNode(Project),
                new SnippetFolderNode(Project)
            };
        }

        #endregion
    }
}
