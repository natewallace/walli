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
            Presenter.AllowDrop = true;
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

            Presenter.Expand();
            ManifestNode manifestNode = new ManifestNode(Project, manifest);

            int index = 0;
            foreach (INode node in Presenter.Nodes)
            {
                if (node is ManifestNode)
                {
                    int result = manifest.CompareTo((node as ManifestNode).Manifest);
                    if (result == 0)
                    {
                        Presenter.NodeManager.ActiveNode = node;
                        return node as ManifestNode; // node is already present
                    }
                    else if (result < 0)
                    {
                        break;
                    }
                }
                index++;
            }

            Presenter.Nodes.Insert(index, manifestNode);
            Presenter.NodeManager.ActiveNode = manifestNode;

            return manifestNode;
        }

        /// <summary>
        /// Check to see if a drag drop can be performed.
        /// </summary>
        /// <param name="e">The item being dragged.</param>
        /// <returns>true if the drag drop can be performed, false if it can't.</returns>
        private bool CanDrop(System.Windows.DragEventArgs e)
        {
            bool canDrop = true;

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] fileNames = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (fileNames != null)
                {

                    foreach (string fileName in fileNames)
                    {
                        string directory = System.IO.Path.GetDirectoryName(fileName);
                        canDrop = (directory != Project.ManifestFolder);

                        if (canDrop)
                        {
                            string extension = System.IO.Path.GetExtension(fileName) ?? String.Empty;
                            if (String.Compare(extension, ".MANIFEST", true) != 0 &&
                                String.Compare(extension, ".XML", true) != 0)
                            {
                                canDrop = false;
                                break;
                            }
                        }
                    }
                }
            }

            return canDrop;
        }

        /// <summary>
        /// Allow users to drop local manifest files into folder.
        /// </summary>
        /// <param name="e">The item being dragged.</param>
        public override void DragOver(System.Windows.DragEventArgs e)
        {
            if (CanDrop(e))
                e.Effects = System.Windows.DragDropEffects.Copy;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        /// <summary>
        /// Process local manifest files that have been dropped into this folder.
        /// </summary>
        /// <param name="e">The item being dropped.</param>
        public override void Drop(System.Windows.DragEventArgs e)
        {
            if (CanDrop(e))
            {
                string[] fileNames = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (fileNames != null)
                {
                    HashSet<string> manifestFileNames = new HashSet<string>();
                    foreach (Manifest m in Project.GetManifests())
                        manifestFileNames.Add(m.FileName.ToLower());

                    foreach (string fileName in fileNames)
                    {
                        string extension = System.IO.Path.GetExtension(fileName) ?? String.Empty;
                        if (String.Compare(extension, ".MANIFEST", true) == 0 ||
                            String.Compare(extension, ".XML", true) == 0)
                        {
                            // make sure the name is unique
                            int index = 0;
                            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            string manifestFileName = System.IO.Path.Combine(
                                Project.ManifestFolder, 
                                System.IO.Path.GetFileName(fileName));

                            while (manifestFileNames.Contains(manifestFileName.ToLower()))
                            {
                                index++;
                                manifestFileName = System.IO.Path.Combine(
                                    Project.ManifestFolder,
                                    String.Format("{0}({1}).xml", fileNameWithoutExt, index));
                            }

                            manifestFileNames.Add(manifestFileName.ToLower());

                            // copy the file over
                            System.IO.File.Copy(fileName, manifestFileName, false);
                        }
                    }

                    // refresh the manifest folder
                    App.Instance.Navigation.ActiveNode = this;
                    App.Instance.GetFunction<RefreshFolderFunction>().Execute();
                }
            }            
        }

        #endregion
    }
}
