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
using System.Windows.Controls;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;
using Wallace.IDE.SalesForce.Node;

namespace Wallace.IDE.SalesForce.Document
{    
    /// <summary>
    /// Abstract base class for text editor documents.
    /// </summary>
    /// <typeparam name="TView">The view for the document.</typeparam>
    public abstract class SourceFileEditorDocumentBase<TView> : TextEditorDocumentBase<TView>, ISourceFileEditorDocument 
        where TView : Control, ITextEditorView
    {
        #region Fields

        /// <summary>
        /// Holds the text that was last retrieved from the server.
        /// </summary>
        private SourceFileContent _serverContent;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the file on.</param>
        /// <param name="file">The file that is being edited.</param>
        public SourceFileEditorDocumentBase(Project project, SourceFile file)
            : base(project)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            File = file;
            Text = File.Name;

            Reload();

            if (file.CheckedOutBy != null && !file.CheckedOutBy.Equals(project.Client.User))
                View.IsReadOnly = true;

            OnViewReady();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The file being edited.
        /// </summary>
        public SourceFile File { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the title to display for this document.
        /// </summary>
        /// <returns>The title to display for this document.</returns>
        protected override string GetTitle()
        {
            return File.Name;
        }

        /// <summary>
        /// Gets the icon to display for this document.
        /// </summary>
        /// <returns>The icon to display for this document.</returns>
        protected override string GetIcon()
        {
            return "Document.png";
        }

        /// <summary>
        /// Save changes made to the content.
        /// </summary>
        public override void Save()
        {
            if (!IsDirty)
                return;

            try
            {
                // check for conflict first
                string currentTimeStamp = null;
                using (App.Wait("Checking for conflicts..."))
                    currentTimeStamp = Project.Client.Meta.GetSourceFileContentLastModifiedTimeStamp(File);
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
                    SalesForceError[] errors = Project.Client.Meta.SaveSourceFileContent(
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
                        currentTimeStamp = Project.Client.Meta.GetSourceFileContentLastModifiedTimeStamp(File);
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
                using (App.Wait("Refreshing document."))
                {
                    _serverContent = Project.Client.Meta.GetSourceFileContent(File);
                    View.Text = _serverContent.ContentValue;
                    View.IsReadOnly = _serverContent.IsReadOnly;
                    IsDirty = false;
                }
            }

            return canReload;
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

        /// <summary>
        /// Set the dirty status and checkout the file if needed.
        /// </summary>
        protected override void OnTextChanged()
        {
            bool isFirstChange = !IsDirty;
            IsDirty = true;

            if (isFirstChange &&
                Project.Client.Checkout.IsEnabled(false) &&
                (bool)Properties.Settings.Default["AutoCheckoutFile"] &&
                File.CheckedOutBy == null)
            {
                using (App.Wait("Checking out file"))
                {
                    Project.Client.Checkout.CheckoutFile(File);
                    foreach (INode node in App.Instance.Navigation.GetNodesByEntity(File))
                    {
                        if (node is SourceFileNode)
                            (node as SourceFileNode).UpdateHeader();
                    }

                    App.Instance.UpdateWorkspaces();
                }
            }
        }

        #endregion
    }
}
