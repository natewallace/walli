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
using Wallace.IDE.SalesForce.Node;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Show the history for the given file.
    /// </summary>
    public class CheckoutFileHistoryFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected file.
        /// </summary>
        /// <returns>The currently selected file.</returns>
        private SourceFile GetSelectedFile()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Repository.IsValid)
            {
                if (App.Instance.Navigation.SelectedNodes.Count == 1 &&
                    App.Instance.Navigation.SelectedNodes[0] is SourceFileNode)
                {
                    SourceFileNode node = (App.Instance.Navigation.SelectedNodes[0] as SourceFileNode);
                    if (node.SourceFile.Parent == null && !String.IsNullOrEmpty(node.SourceFile.Id))
                        return node.SourceFile;
                }
            }

            return null;
        }

        /// <summary>
        /// Setup header.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "History.png");
                presenter.ToolTip = "Show file history";
            }
            else
            {
                presenter.Header = "Show file history";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "History.png");
            }
        }

        /// <summary>
        /// Set the header based on the currently selected file(s).
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (GetSelectedFile() != null);
        }

        /// <summary>
        /// Show history for the file.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Repository.IsValid)
            {
                SourceFile file = GetSelectedFile();
                SimpleRepositoryCommit[] history = project.Repository.GetHistory(file);
                CommitListDocument document = new CommitListDocument(project, file, history);
                App.Instance.Content.OpenDocument(document);
            }
        }

        #endregion
    }
}
