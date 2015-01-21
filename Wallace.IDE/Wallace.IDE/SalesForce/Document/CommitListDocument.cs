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
using Wallace.IDE.Framework.UI;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// List of commits.
    /// </summary>
    public class CommitListDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="commits">Commits.</param>
        public CommitListDocument(Project project, SourceFile file, IEnumerable<SimpleRepositoryCommit> commits)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (file == null)
                throw new ArgumentNullException("file");
            if (commits == null)
                throw new ArgumentNullException("commits");

            Project = project;
            File = file;
            Commits = commits;

            View = new CommitHeaderListControl();
            View.Commits = Commits;
            View.OpenClick += View_OpenClick;

            MenuFunctionManager menuManager = new MenuFunctionManager(View.ListContextMenu);
            menuManager.AddFunction(App.Instance.GetFunction<CommitFileOpenFunction>());
            menuManager.AddFunction(App.Instance.GetFunction<CommitFileCompareFunction>());
            menuManager.AddFunction(App.Instance.GetFunction<CommitFileShaCopyFunction>());
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the document is in.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The file for the document.
        /// </summary>
        public SourceFile File { get; private set; }

        /// <summary>
        /// The commits in the document.
        /// </summary>
        public IEnumerable<SimpleRepositoryCommit> Commits { get; private set; }

        /// <summary>
        /// The document view.
        /// </summary>
        public CommitHeaderListControl View { get; private set; }

        /// <summary>
        /// Get the currently selected commits.
        /// </summary>
        public SimpleRepositoryCommit[] SelectedCommits
        {
            get { return View.SelectedCommits; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader(File.FullName, "History.png");
                Presenter.Content = View;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Open the version of the file currently selected.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_OpenClick(object sender, EventArgs e)
        {
            App.Instance.GetFunction<CommitFileOpenFunction>().Execute();
        }

        #endregion
    }
}
