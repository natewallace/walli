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
using System.Windows.Controls;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{    
    /// <summary>
    /// Abstract base class for text editor documents.
    /// </summary>
    /// <typeparam name="TView">The view for the document.</typeparam>
    public abstract class SourceFileEditorDocumentBase<TView> : DocumentBase, ISourceFileEditorDocument 
        where TView : Control, ITextEditorView
    {
        #region Fields

        /// <summary>
        /// Holds the text that was last retrieved from the server.
        /// </summary>
        private SourceFileContent _serverContent;

        /// <summary>
        /// Supports the IsDirty property.
        /// </summary>
        private bool _isDirty;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the file on.</param>
        /// <param name="file">The file that is being edited.</param>
        public SourceFileEditorDocumentBase(Project project, SourceFile file)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (file == null)
                throw new ArgumentNullException("file");

            Project = project;
            File = file;

            _isDirty = false;

            View = Activator.CreateInstance<TView>();
            OnViewCreated();

            Reload();

            View.TextChanged += View_TextChanged;
            View.PreviewKeyDown += View_PreviewKeyDown;

            OnViewReady();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project to edit the file on.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The file being edited.
        /// </summary>
        public SourceFile File { get; private set; }

        /// <summary>
        /// The control that is used for display.
        /// </summary>
        protected TView View { get; set; }

        /// <summary>
        /// Flag that indicates if there are unsaved changes.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    Presenter.Header = _isDirty ? VisualHelper.CreateIconHeader(GetTitle(), GetIcon(), "*") :
                                                  VisualHelper.CreateIconHeader(GetTitle(), GetIcon());
                    Presenter.ToolTip = File.FileName;
                    App.Instance.UpdateWorkspaces();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called right after the view has been created.
        /// </summary>
        protected virtual void OnViewCreated()
        {

        }

        /// <summary>
        /// Called when the view is ready.
        /// </summary>
        protected virtual void OnViewReady()
        {

        }

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader(GetTitle(), GetIcon());
                Presenter.ToolTip = File.FileName;
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Gets the title to display for this document.
        /// </summary>
        /// <returns>The title to display for this document.</returns>
        protected virtual string GetTitle()
        {
            return File.Name;
        }

        /// <summary>
        /// Gets the icon to display for this document.
        /// </summary>
        /// <returns>The icon to display for this document.</returns>
        protected virtual string GetIcon()
        {
            return "Document.png";
        }

        /// <summary>
        /// Give focus to the text input when the document is made active.
        /// </summary>
        public override void Activated()
        {
            View.FocusText();
        }

        /// <summary>
        /// Display warning if the document has unsaved changes.
        /// </summary>
        /// <returns>true if its ok to proceed with the closing.</returns>
        public override bool Closing()
        {
            if (IsDirty)
            {
                return (App.MessageUser(
                    "You have unsaved changes which will be lost.  Do you want to proceed?",
                    "Data Loss",
                    System.Windows.MessageBoxImage.Warning,
                    new string[] { "Yes", "No" }) == "Yes");
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Save changes made to the content.
        /// </summary>
        public virtual void Save()
        {
            if (!IsDirty)
                return;

            try
            {
                // check for conflict first
                string currentTimeStamp = null;
                using (App.Wait("Checking for conflicts..."))
                    currentTimeStamp = Project.Client.GetSourceFileContentLastModifiedTimeStamp(File);
                if (currentTimeStamp != _serverContent.LastModifiedTimeStamp)
                {
                    if (App.MessageUser(
                        "This file has been modified since you opened it.  If you continue you will overwrite the current file on the server which may result in the loss of someone else's changes.  Do you want to proceed?",
                        "Conflict",
                        System.Windows.MessageBoxImage.Warning,
                        new string[] { "Yes", "No" }) != "Yes")
                        return;
                }

                using (App.Wait("Saving..."))
                {
                    // save changes
                    SalesForceError[] errors = Project.Client.SaveSourceFileContent(
                        File, 
                        View.Text, 
                        _serverContent.MetadataValue);

                    // display errors
                    List<string> errorMessages = new List<string>();
                    foreach (SalesForceError error in errors)
                        errorMessages.Add(error.ToString());
                    View.SetErrors(errorMessages);

                    // update content
                    if (errors.Length == 0)
                    {
                        currentTimeStamp = Project.Client.GetSourceFileContentLastModifiedTimeStamp(File);
                        _serverContent = new SourceFileContent(
                            File.FileType.Name,
                            View.Text,
                            currentTimeStamp);

                        IsDirty = false;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Reload the document.
        /// </summary>
        /// <returns>true if the document was reloaded.</returns>
        public virtual bool Reload()
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
                using (App.Wait("Refreshing document."))
                {
                    _serverContent = Project.Client.GetSourceFileContent(File);
                    View.Text = _serverContent.ContentValue;
                    IsDirty = false;
                }
            }

            return canReload;
        }

        /// <summary>
        /// Open the text search dialog.
        /// </summary>
        public virtual void SearchText()
        {
            View.SearchText();
        }

        /// <summary>
        /// Copy selected text to the clipboard.
        /// </summary>
        public virtual void CopyText()
        {
            View.CopyText();
        }

        /// <summary>
        /// Delete selected text and copy it to the clipboard.
        /// </summary>
        public virtual void CutText()
        {
            View.CutText();
        }

        /// <summary>
        /// Paste text from the clipboard to the editor.
        /// </summary>
        public virtual void PasteText()
        {
            View.PasteText();
        }

        /// <summary>
        /// Undo the last test change.
        /// </summary>
        public void UndoText()
        {
            View.UndoText();
        }

        /// <summary>
        /// Redo the last text change.
        /// </summary>
        public void RedoText()
        {
            View.RedoText();
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        public void SelectAllText()
        {
            View.SelectAllText();
        }

        /// <summary>
        /// Go to the given line number in the document.
        /// </summary>
        /// <param name="line">The line number to go to. (1 based)</param>
        public void GotToLine(int line)
        {
            View.GotToLine(line);
        }

        /// <summary>
        /// If this document represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this document represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (File.CompareTo(entity) == 0);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the display to indicate the text has been changed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        /// <summary>
        /// Respond to shortcut keys.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.S:
                        Save();
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
