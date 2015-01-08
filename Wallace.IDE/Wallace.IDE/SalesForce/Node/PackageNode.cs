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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// A node that reprsents a package.
    /// </summary>
    public class PackageNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="package">Package.</param>
        public PackageNode(Project project, Package package)
        {
            if (project == null)
                throw new ArgumentException("project");
            if (package == null)
                throw new ArgumentException("package");

            Project = project;
            Package = package;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the package belongs to.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The package this node represents.
        /// </summary>
        public Package Package { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return (Package == null) ? String.Empty : Package.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(Package.Name, "Package.png");
        }

        /// <summary>
        /// Open a package editor.
        /// </summary>
        public override void DoubleClick()
        {
            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is PackageViewDocument && (document as PackageViewDocument).Package.Equals(Package))
                {
                    App.Instance.Content.OpenDocument(document);
                    return;
                }
            }

            PackageViewDocument packageDocument = new PackageViewDocument(Project, Package);
            App.Instance.Content.OpenDocument(packageDocument);     
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (Package.CompareTo(entity) == 0);
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
                    App.Instance.GetFunction<DeletePackageFunction>()
                });
        }

        /// <summary>
        /// Allow package to be exported by drag and drop.
        /// </summary>
        public override void DragStart()
        {
            Presenter.DoDragDrop(
                System.Windows.DataFormats.FileDrop, 
                new string[] { Package.FileName }, 
                System.Windows.DragDropEffects.Copy);
        }

        #endregion
    }
}
