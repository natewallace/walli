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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// This node holds metadata items.
    /// </summary>
    public class SourceFileNode : NodeBase
    {
        #region Fields

        /// <summary>
        /// Supports the SourceFile property.
        /// </summary>
        private SourceFile _sourceFile;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public SourceFileNode(Project project, SourceFile sourceFile)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (sourceFile == null)
                throw new ArgumentNullException("sourceFile");

            Project = project;
            SourceFile = sourceFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The source file being shown.  If null then this node is the root for all source items.
        /// </summary>
        public SourceFile SourceFile
        {
            get
            {
                return _sourceFile;                         
            }
            set
            {
                if (value == null)
                    throw new Exception("SourceFile can't be set to null.");

                _sourceFile = value;
            }
        }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return (SourceFile == null) ? String.Empty : SourceFile.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            UpdateHeader();
        }

        /// <summary>
        /// Update the header.
        /// </summary>
        public virtual void UpdateHeader()
        {
            if (SourceFile.CheckedOutBy != null)
                if (SourceFile.CheckedOutBy.Equals(Project.Client.User))
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "LockGreen.png");
                else
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "LockRed.png");
            else
                Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "Document.png");
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        /// <returns>true.</returns>
        public override bool HasChildren()
        {
            if (SourceFile == null)
                return true;

            return (SourceFile.Children.Length > 0);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The resulting children.</returns>
        public override INode[] GetChildren()
        {
            List<INode> nodes = new List<INode>();

            Dictionary<string, SourceFileType> fileTypes = new Dictionary<string, SourceFileType>();
            foreach (SourceFile sf in SourceFile.Children)
            {
                if (!fileTypes.ContainsKey(sf.FileType.Name))
                    fileTypes.Add(sf.FileType.Name, sf.FileType);
            }

            foreach (SourceFileType fileType in fileTypes.Values.OrderBy(ft => ft.Name))
                nodes.Add(new SourceFileFolderNode(Project, fileType, SourceFile));

            return nodes.ToArray();
        }

        /// <summary>
        /// Support drag drop operations.
        /// </summary>
        public override void DragStart()
        {
            List<SourceFile> selectedFiles = new List<SourceFile>();
            foreach (INode node in Presenter.NodeManager.SelectedNodes)
            {
                if (node is SourceFileNode)
                    selectedFiles.Add((node as SourceFileNode).SourceFile);
            }

            if (selectedFiles.Count > 0)
                Presenter.DoDragDrop("SalesForceData.SourceFile[]", selectedFiles.ToArray(), System.Windows.DragDropEffects.Copy);
        }

        /// <summary>
        /// Open a new document or make a currently open document active.
        /// </summary>
        public override void DoubleClick()
        {
            if (!OpenExistingEditorDocument())
            {
                using (App.Wait("Opening file..."))
                {
                    SourceFileEditorDocument sourceFileDocument = new SourceFileEditorDocument(Project, SourceFile);
                    App.Instance.Content.OpenDocument(sourceFileDocument);
                }
            }
        }

        /// <summary>
        /// Looks for an editor document that is already open and makes it active.
        /// </summary>
        /// <returns>true if the editor document is open and was made active, false if the editor document is not open.</returns>
        protected bool OpenExistingEditorDocument()
        {
            // look for already open document
            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is ISourceFileEditorDocument && (document as ISourceFileEditorDocument).File.Equals(SourceFile))
                {
                    App.Instance.Content.OpenDocument(document);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get context menu items.
        /// </summary>
        /// <returns>The context menu items for this node.</returns>
        public override IFunction[] GetContextFunctions()
        {
            return new IFunction[]
            {
                App.Instance.GetFunction<CheckOutFileFunction>(),
                App.Instance.GetFunction<PropertiesFunction>()
            };
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (SourceFile.CompareTo(entity) == 0);
        }

        #endregion
    }
}
