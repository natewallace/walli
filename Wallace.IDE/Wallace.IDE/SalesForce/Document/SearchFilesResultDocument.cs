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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Document for displayed search results.
    /// </summary>
    public class SearchFilesResultDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="query">The query text.</param>
        /// <param name="files">The result files.</param>
        public SearchFilesResultDocument(Project project, string query, SourceFile[] files)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;

            View = new SearchFilesResultControl();
            View.QueryText = query;
            View.Files = files;
            View.FileOpenClick += View_FileOpenClick;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this document is displayed in.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The view used by the document.
        /// </summary>
        private SearchFilesResultControl View { get; set; }

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
                Presenter.Header = VisualHelper.CreateIconHeader("Search results", "Find.png");
                Presenter.Content = View;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Open the given file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_FileOpenClick(object sender, EventArgs e)
        {
            SourceFile file = View.SelectedFile;
            if (file != null)
            {
                // activate existing document
                foreach (IDocument document in App.Instance.Content.OpenDocuments)
                {
                    if (document is ISourceFileEditorDocument &&
                        (document as ISourceFileEditorDocument).File.Equals(file))
                    {
                        App.Instance.Content.ActiveDocument = document;
                        return;
                    }
                }

                // open new document
                using (App.Wait("Opening file"))
                {
                    Project.Client.Checkout.RefreshCheckoutStatus(file);
                    IDocument document = null;

                    switch (file.FileType.Name)
                    {
                        case "ApexClass":
                            document = new ClassEditorDocument(Project, file);                            
                            break;

                        case "ApexTrigger":
                            document = new ClassEditorDocument(Project, file);
                            break;

                        case "ApexPage":
                        case "ApexComponent":
                            document = new VisualForceEditorDocument(Project, file);
                            break;

                        default:
                            break;
                    }

                    if (document != null)
                        App.Instance.Content.OpenDocument(document);
                }
            }
        }

        #endregion
    }
}
