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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// A node for a snippet.
    /// </summary>
    public class SnippetNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="path">Path.</param>
        public SnippetNode(Project project, string path)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is null or whitespace.", "path");

            Project = project;
            Path = path;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The path for the snippet.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(Text, "Snippet.png");
        }

        /// <summary>
        /// Open the manifest file.
        /// </summary>
        public override void DoubleClick()
        {
            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is SnippetEditorDocument && 
                    System.IO.Path.Equals(Path, (document as SnippetEditorDocument).Path))
                {
                    App.Instance.Content.OpenDocument(document);
                    return;
                }
            }

            SnippetEditorDocument snippetDocument = new SnippetEditorDocument(Project, Path);
            App.Instance.Content.OpenDocument(snippetDocument);            
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (Path == (entity as string));
        }

        /// <summary>
        /// Get functions that show up in the nodes context menu.
        /// </summary>
        /// <returns>Functions that show up in the nodes context menu.</returns>
        public override IFunction[] GetContextFunctions()
        {
            return MergeFunctions(
                base.GetContextFunctions(),
                new IFunction[] 
                {
                    //App.Instance.GetFunction<MergeManifestFunction>(),
                    //App.Instance.GetFunction<DeleteManifestFunction>()
                });
        }

        /// <summary>
        /// Allow manifest to be exported by drag and drop.
        /// </summary>
        public override void DragStart()
        {
            // get selected manifests
            //List<Manifest> selectedManifests = new List<SalesForceData.Manifest>();
            //foreach (INode node in App.Instance.Navigation.SelectedNodes)
            //{
            //    if (node is ManifestNode)
            //    {
            //        selectedManifests.Add((node as ManifestNode).Manifest);
            //    }
            //    else
            //    {
            //        selectedManifests = null;
            //        break;
            //    }
            //}

            //// start drag operation with selected manifests
            //if (selectedManifests != null)
            //{
            //    List<string> fileNames = new List<string>();
            //    foreach (Manifest manifest in selectedManifests)
            //        fileNames.Add(manifest.FileName);

            //    System.Windows.DataObject dataObject = new System.Windows.DataObject();
            //    dataObject.SetData(System.Windows.DataFormats.FileDrop, fileNames.ToArray());
            //    dataObject.SetData("SalesForceData.Manifest[]", selectedManifests.ToArray());

            //    Presenter.DoDragDrop(dataObject, System.Windows.DragDropEffects.Copy);
            //}
        }

        #endregion
    }
}
