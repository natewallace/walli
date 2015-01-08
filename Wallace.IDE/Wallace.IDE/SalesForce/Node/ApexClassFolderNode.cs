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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// This node is a folder that holds all of the apex class nodes.
    /// </summary>
    public class ApexClassFolderNode : SourceFileFolderNode
    {
        #region Fields

        /// <summary>
        /// This is set to true after the apex classes have been read in.
        /// </summary>
        private bool _isClassesLoaded = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="sourceFileType">SourceFileType.</param>
        public ApexClassFolderNode(Project project, SourceFileType sourceFileType) :
            base(project, sourceFileType)
        {
            if (sourceFileType.Name != "ApexClass")
                throw new ArgumentException("sourceFileType", "The sourceFileType must be for ApexClass");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add the given apex file to the folder.
        /// </summary>
        /// <param name="sourceFile">The apex file to add.</param>
        public void AddApexClass(SourceFile sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException("file");
            if (sourceFile.FileType.Name != "ApexClass")
                throw new ArgumentException("file", "The file type must be ApexClass");

            if (_isClassesLoaded)
            {
                int index = 0;
                for (; index < Presenter.Nodes.Count; index++)
                {
                    ApexClassNode node = Presenter.Nodes[index] as ApexClassNode;
                    if (node == null)
                        continue;

                    if (sourceFile.CompareTo(node.SourceFile) < 0)
                        break;
                }

                Presenter.Nodes.Insert(index, new ApexClassNode(Project, sourceFile));
            }
        }

        /// <summary>
        /// Remove the given file from the folder.
        /// </summary>
        /// <param name="sourceFile">The source file to remove.</param>
        public void RemoveApexClass(SourceFile sourceFile)
        {
            if (_isClassesLoaded)
            {
                for (int i = 0; i < Presenter.Nodes.Count; i++)
                {
                    if (Presenter.Nodes[i] is ApexPackageFolderNode)
                    {
                        (Presenter.Nodes[i] as ApexPackageFolderNode).RemoveSourceFile(sourceFile);
                    }
                    else if (Presenter.Nodes[i] is ApexClassNode && (Presenter.Nodes[i] as ApexClassNode).SourceFile.Equals(sourceFile))
                    {
                        Presenter.Nodes.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set the _isClassesLoaded flag.
        /// </summary>
        /// <returns>The children apex classes that were read in.</returns>
        public override INode[] GetChildren()
        {
            _isClassesLoaded = true;
            return GroupNodesByPackage(base.GetChildren());
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
                    App.Instance.GetFunction<NewClassFunction>()
                }
            );

            return functions.ToArray();
        }

        #endregion
    }
}
