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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// This node represents a manifest.
    /// </summary>
    public class ManifestNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="manifest">Manifest.</param>
        public ManifestNode(Project project, Manifest manifest)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            Project = project;
            Manifest = manifest;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The manifest for the node.
        /// </summary>
        public Manifest Manifest { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return Manifest.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(Text, "Manifest.png");
        }

        /// <summary>
        /// Open the manifest file.
        /// </summary>
        public override void DoubleClick()
        {
            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is ManifestEditorDocument && (document as ManifestEditorDocument).Manifest.Equals(Manifest))
                {
                    App.Instance.Content.OpenDocument(document);
                    return;
                }
            }

            ManifestEditorDocument manifestDocument = new ManifestEditorDocument(Project, Manifest);
            App.Instance.Content.OpenDocument(manifestDocument);            
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (Manifest.CompareTo(entity) == 0);
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
                    App.Instance.GetFunction<DeleteManifestFunction>()
                });
        }

        /// <summary>
        /// Allow manifest to be exported by drag and drop.
        /// </summary>
        public override void DragStart()
        {
            Presenter.DoDragDrop(
                System.Windows.DataFormats.FileDrop,
                new string[] { Manifest.FileName },
                System.Windows.DragDropEffects.Copy);
        }

        #endregion
    }
}
