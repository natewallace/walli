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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Folder node for manifests.
    /// </summary>
    public class ManifestFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public ManifestFolderNode(Project project)
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
                return "Manifest";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader("Manifest", "FolderClosed.png");
            Presenter.ExpandedHeader = VisualHelper.CreateIconHeader("Manifest", "FolderOpen.png");
        }

        /// <summary>
        /// Get functions available.
        /// </summary>
        /// <returns>The functions that can be executed on this node.</returns>
        public override IFunction[] GetContextFunctions()
        {
            List<IFunction> functions = new List<IFunction>(base.GetContextFunctions());
            functions.AddRange(
                new IFunction[] 
                {
                    App.Instance.GetFunction<NewManifestFunction>()
                }
            );

            return functions.ToArray();
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
            foreach (Manifest manifest in Project.GetManifests())
                result.Add(new ManifestNode(Project, manifest));

            return result.ToArray();
        }

        /// <summary>
        /// Add a new node for the given manifest to this node.
        /// </summary>
        /// <param name="manifest">The manifest to add.</param>
        /// <returns>The newly added node.</returns>
        public ManifestNode AddManifest(Manifest manifest)
        {
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            ManifestNode manifestNode = new ManifestNode(Project, manifest);

            int index = 0;
            foreach (INode node in Presenter.Nodes)
            {
                if (node is ManifestNode)
                {
                    int result = manifest.CompareTo((node as ManifestNode).Manifest);
                    if (result == 0)
                        throw new Exception("There is already a manifest named: " + manifest.Name);
                    else if (result < 0)
                        break;
                }
                index++;
            }

            Presenter.Nodes.Insert(index, manifestNode);
            Presenter.Expand();
            Presenter.NodeManager.ActiveNode = manifestNode;

            return manifestNode;
        }

        #endregion
    }
}
