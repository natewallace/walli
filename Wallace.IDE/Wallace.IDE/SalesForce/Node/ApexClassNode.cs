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
    /// A node that represents an apex class.
    /// </summary>
    public class ApexClassNode : SourceFileNode
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project the class belongs to.</param>
        /// <param name="sourceFile">The class source file.</param>
        public ApexClassNode(Project project, SourceFile sourceFile) :
            base(project, sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException("sourceFile");
            if (sourceFile.FileType.Name != "ApexClass")
                throw new ArgumentException("sourceFile", "sourceFile must be of type ApexClass");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the header.
        /// </summary>
        public override void UpdateHeader()
        {
            if (SourceFile.CheckedOutBy != null)
                if (SourceFile.CheckedOutBy.Equals(Project.Client.User))
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentClassEdit.png");
                else
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentClassLock.png");
            else
                Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentClass.png");
        }

        /// <summary>
        /// Open the apex class editor.
        /// </summary>
        public override void DoubleClick()
        {
            if (!OpenExistingEditorDocument())
            {
                using (App.Wait("Opening class..."))
                {
                    if (Project.IsCheckoutEnabled)
                    {
                        Project.Client.RefreshCheckOutStatus(SourceFile);
                        UpdateHeader();
                    }

                    ClassEditorDocument apexDocument = new ClassEditorDocument(Project, SourceFile);
                    App.Instance.Content.OpenDocument(apexDocument);
                }
            }
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
                    App.Instance.GetFunction<DeleteSourceFileFunction>()
                });
        }

        #endregion
    }
}
