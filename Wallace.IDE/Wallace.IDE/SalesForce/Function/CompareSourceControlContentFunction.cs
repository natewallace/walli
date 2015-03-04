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

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Compare the current text with the most recent commit in source control.
    /// </summary>
    public class CompareSourceControlContentFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current document.
        /// </summary>
        private ISourceFileEditorDocument CurrentDocument
        {
            get
            {
                if (App.Instance.Content.ActiveDocument is ISourceFileEditorDocument &&
                    (App.Instance.Content.ActiveDocument as ISourceFileEditorDocument).File != null)
                    return App.Instance.Content.ActiveDocument as ISourceFileEditorDocument;
                else
                    return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "Compare.png");
                presenter.ToolTip = "Compare with latest version in source control";
            }
            else
            {
                presenter.Header = "Compare with latest version in source control";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Compare.png");
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = 
                App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.SalesForceApp.CurrentProject.Repository.IsValid &&
                (CurrentDocument != null);
        }

        /// <summary>
        /// Do comparison.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            ISourceFileEditorDocument currentDocument = CurrentDocument;

            if (project != null && currentDocument != null)
            {
                string olderText = project.Repository.GetContent(currentDocument.File, null);
                string newerText = currentDocument.Content;
                
                if (olderText == currentDocument.Content)
                {
                    App.MessageUser("The most recent version in source control is identical to your current version.",
                                    "Compare",
                                    System.Windows.MessageBoxImage.Information,
                                    new string[] { "OK" });
                }
                else
                {
                    string diff = DiffUtility.Diff(olderText, currentDocument.Content);
                    TextViewDocument document = new TextViewDocument(
                        project,
                        diff,
                        currentDocument.File.Name,
                        "Compare.png",
                        true);

                    App.Instance.Content.OpenDocument(document);
                }
            }
        }

        #endregion
    }
}
