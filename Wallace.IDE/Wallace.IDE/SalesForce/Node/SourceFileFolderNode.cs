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
    /// A node that displays a source type.
    /// </summary>
    public class SourceFileFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="sourceFileType">SourceFileType.</param>
        public SourceFileFolderNode(Project project, SourceFileType sourceFileType)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (sourceFileType == null)
                throw new ArgumentNullException("sourceFileType");

            Project = project;
            SourceFileType = sourceFileType;
            SourceFile = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="sourceFileType">SourceFileType.</param>
        /// <param name="sourceFile">SourceFile.</param>
        public SourceFileFolderNode(Project project, SourceFileType sourceFileType, SourceFile sourceFile)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (sourceFileType == null)
                throw new ArgumentNullException("sourceFileType");

            Project = project;
            SourceFileType = sourceFileType;
            SourceFile = sourceFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The type of source files in the folder.
        /// </summary>
        public SourceFileType SourceFileType { get; private set; }

        /// <summary>
        /// A source file which the types are being shown for.  This can be null.
        /// </summary>
        public SourceFile SourceFile { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return (SourceFileType == null) ? String.Empty : SourceFileType.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(SourceFileType.Name, "FolderClosed.png");
            Presenter.ExpandedHeader = VisualHelper.CreateIconHeader(SourceFileType.Name, "FolderOpen.png");
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
            IEnumerable<SourceFile> sourceFiles = null;

            // get source files for the current type
            if (SourceFile == null)
                sourceFiles = Project.Client.GetSourceFiles(new SourceFileType[] { SourceFileType }, true).OrderBy(sf => sf.Name);
            // get source files for the current type within the current file
            else
                sourceFiles = SourceFile.Children.Where(sf => sf.FileType.Name == SourceFileType.Name).OrderBy(sf => sf.Name);

            // create nodes for the source files
            foreach (SourceFile sf in sourceFiles)
            {
                switch (sf.FileType.Name)
                {
                    case "ApexClass":
                        nodes.Add(new ApexClassNode(Project, sf));
                        break;

                    case "ApexTrigger":
                        nodes.Add(new ApexTriggerNode(Project, sf));
                        break;

                    case "ApexPage":
                        nodes.Add(new ApexPageNode(Project, sf));
                        break;

                    case "ApexComponent":
                        nodes.Add(new ApexComponentNode(Project, sf));
                        break;

                    default:
                        nodes.Add(new SourceFileNode(Project, sf));
                        break;
                }
            }

            return nodes.ToArray();
        }

        #endregion
    }
}
