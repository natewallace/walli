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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Compare two commits.
    /// </summary>
    public class CommitFileCompareFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected commit or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected commit or null if there isn't one.</returns>
        private SimpleRepositoryCommit[] GetSelectedCommits()
        {
            if (App.Instance.SalesForceApp.CurrentProject == null)
                return null;

            CommitListDocument document = App.Instance.Content.ActiveDocument as CommitListDocument;
            if (document == null || document.File == null)
                return null;

            if (document.SelectedCommits.Length == 2)
                return document.SelectedCommits;
            else
                return null;
        }

        /// <summary>
        /// Set the header based on the currently selected commits.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            SimpleRepositoryCommit[] commits = GetSelectedCommits();

            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "Compare.png");
                presenter.ToolTip = "Compare";
            }
            else
            {
                presenter.Header = "Compare";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Compare.png");
            }

            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null &&
                         App.Instance.Content.ActiveDocument is CommitListDocument);
            IsEnabled = (commits != null);
        }

        /// <summary>
        /// Open the commit.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            SimpleRepositoryCommit[] commits = GetSelectedCommits();
            CommitListDocument document = App.Instance.Content.ActiveDocument as CommitListDocument;

            if (project != null && commits != null && document != null)
            {
                string text = null;
                using (App.Wait("Creating diff."))
                    text = project.Repository.Diff(document.File, commits[1], commits[0]);

                string title = String.Format(
                    "{0} [{1}/{2}]",
                    document.File.Name,
                    commits[1].ShaShort,
                    commits[0].ShaShort);

                TextViewDocument doc = new TextViewDocument(project, text, title, "Compare.png", true);
                App.Instance.Content.OpenDocument(doc);
            }
        }

        #endregion
    }
}
