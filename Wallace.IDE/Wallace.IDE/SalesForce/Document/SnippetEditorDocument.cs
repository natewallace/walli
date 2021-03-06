﻿/*
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
using SalesForceLanguage.Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Snippet document editor.
    /// </summary>
    public class SnippetEditorDocument : TextEditorDocumentBase<ApexEditorControl>
    {
        #region Fields

        /// <summary>
        /// Supports the FoldAllToggle property.
        /// </summary>
        private bool _foldAllToggle;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the apex on.</param>
        /// <param name="path">The path to the snippet being edited.</param>
        public SnippetEditorDocument(Project project, string path)
            : base(project)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is null or whitespace.", "path");

            Path = path;

            Reload();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The path to the snippet being edited.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Get/Set the toggle property that folds or unfolds all folding sections.
        /// </summary>
        public bool FoldAllToggle
        {
            get
            {
                return _foldAllToggle;
            }
            set
            {
                _foldAllToggle = value;
                View.FoldAll(_foldAllToggle);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reload the saved snippet text.
        /// </summary>
        /// <returns>true if the document was reloaded.</returns>
        public override bool Reload()
        {
            bool canReload = true;
            if (IsDirty)
            {
                canReload = (App.MessageUser(
                    "You have unsaved changes which will be lost.  Do you want to proceed?",
                    "Data Loss",
                    System.Windows.MessageBoxImage.Warning,
                    new string[] { "Yes", "No" }) == "Yes");
            }

            if (canReload)
            {
                View.Text = System.IO.File.ReadAllText(Path);
                IsDirty = false;
            }

            return canReload;            
        }

        /// <summary>
        /// Disable the navigation controls.
        /// </summary>
        protected override void OnViewReady()
        {
            base.OnViewReady();
            View.IsNavigationVisible = false;
        }

        /// <summary>
        /// Use the file name for the title.
        /// </summary>
        /// <returns>The file name for the title.</returns>
        protected override string GetTitle()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        /// <summary>
        /// Returns the trigger icon.
        /// </summary>
        /// <returns>The trigger icon.</returns>
        protected override string GetIcon()
        {
            return "Snippet.png";
        }

        /// <summary>
        /// Set the language manager on the view.
        /// </summary>
        protected override void OnViewCreated()
        {
            View.ParseRequested += View_ParseRequested;            
            View.LanguageManager = Project.Language;
        }

        /// <summary>
        /// Handle name changes.
        /// </summary>
        public override void Save()
        {
            System.IO.File.WriteAllText(Path, View.Text);
            IsDirty = false;
        }

        /// <summary>
        /// Comment or uncomment the currently selected text.
        /// </summary>
        /// <param name="flag">If true the selected text is commented.  If false the selected text is uncommented.</param>
        public void CommentSelectedText(bool flag)
        {
            View.CommentSelectedText(flag);
        }

        /// <summary>
        /// Parse the text to make sure its up to date.
        /// </summary>
        public override void Activated()
        {
            base.Activated();
            Parse();
        }

        /// <summary>
        /// Parse the text.
        /// </summary>
        private void Parse()
        {
            ParseResult result = Project.Language.ParseApex(
                String.Format("public class WalliSnippetPlaceHolder52724597325 {{ public void f() {{ \n{0}\n }} }}", View.Text),
                true,
                false,
                false);

            result.ApplyLineOffset(-1);

            View.ParseData = result;
            App.Instance.UpdateWorkspaces();
        }

        /// <summary>
        /// Execute the snippet.
        /// </summary>
        public void Execute()
        {
            View.SetErrors(null);

            // check if log exists and create one if it doesn't
            bool logExists = false;
            LogListener log = null;

            foreach (LogListener l in Project.Client.Diagnostic.GetLogListeners())
            {
                if (l.TracedEntityId == Project.Client.User.Id)
                {
                    // if there is an expired log delete it
                    if (l.ExpirationDate < DateTime.Now)
                    {
                        Project.Client.Diagnostic.DeleteLogListener(l);
                    }
                    else
                    {
                        logExists = true;
                        log = l;
                        break;
                    }
                }
            }

            if (!logExists)
            {
                log = Project.Client.Diagnostic.CreateLogListener(
                    Project.Client.User.Id,
                    Project.Client.User.Name,
                    Project.Client.User.Id,
                    Project.Client.User.Name,
                    DateTime.Now.AddHours(1),
                    LogLevel.Debug,
                    LogLevel.Error,
                    LogLevel.Fine,
                    LogLevel.Error,
                    LogLevel.Error,
                    LogLevel.Error,
                    LogLevel.Error,
                    LogLevel.Error);
            }

            // execute the code
            SalesForceError result = Project.Client.Diagnostic.ExecuteSnippet(View.Text);
            if (result != null)
            {
                View.SetErrors(new string[] { result.ToString() });
            }
            
            // show results
            if (result == null || result.StatusCode != "COMPILE FAILURE")
            {
                LogViewerDocument logViewer = null;
                IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(log);
                foreach (IDocument document in documents)
                {
                    if (document is LogViewerDocument)
                    {
                        logViewer = document as LogViewerDocument;
                        break;
                    }
                }

                if (logViewer == null)
                {
                    logViewer = new LogViewerDocument(Project, log);
                    App.Instance.Content.OpenDocument(logViewer);
                    logViewer.Refresh();
                    logViewer.SelectFirstLog();
                }
                else
                {
                    App.Instance.Content.ActiveDocument = logViewer;
                    logViewer.Refresh();
                    logViewer.SelectFirstLog();
                }
            }

            // delete log listener if it was created just for this
            if (!logExists)
                Project.Client.Diagnostic.DeleteLogListener(log);
        }

        /// <summary>
        /// Listen for F5 key press to execute snippet.
        /// </summary>
        /// <param name="e">Key press arguments.</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case System.Windows.Input.Key.F5:
                    App.Instance.GetFunction<ExecuteSnippetFunction>().Execute();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// If this document represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this document represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            if (!(entity is string))
                return false;

            return System.IO.Path.Equals(Path, entity as string);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Perform parse and update workspace.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_ParseRequested(object sender, EventArgs e)
        {
            Parse();
        }

        #endregion
    }
}
