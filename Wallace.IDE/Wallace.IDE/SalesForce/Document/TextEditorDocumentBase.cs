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
using System.Windows.Controls;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Abstract base class for text editor documents.
    /// </summary>
    /// <typeparam name="TView">The view for the document.</typeparam>
    public abstract class TextEditorDocumentBase<TView> : DocumentBase, ITextEditorDocument 
        where TView : Control, ITextEditorView
    {
        #region Fields

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
        public TextEditorDocumentBase(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;

            _isDirty = false;

            View = Activator.CreateInstance<TView>();
            OnViewCreated();            

            View.PreviewKeyDown += View_PreviewKeyDown;
            View.TextChanged += View_TextChanged;

            OnViewReady();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project to edit the file on.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The control that is used for display.
        /// </summary>
        protected TView View { get; set; }

        /// <summary>
        /// The text displayed.
        /// </summary>
        public string Content
        {
            get
            {
                if (View == null)
                    return String.Empty;
                else
                    return View.Text;
            }
            set
            {
                if (View != null)
                    View.Text = value;
            }
        }

        /// <summary>
        /// When true the view will be read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return View.IsReadOnly;
            }
            set
            {
                View.IsReadOnly = value;
            }
        }

        /// <summary>
        /// Gets the current line number
        /// </summary>
        public int CurrentLineNumber
        {
            get { return View.CurrentLineNumber; }
        }

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
                if (_isDirty != value && Presenter != null)
                {
                    _isDirty = value;
                    Presenter.Header = _isDirty ? VisualHelper.CreateIconHeader(GetTitle(), GetIcon(), "*") :
                                                  VisualHelper.CreateIconHeader(GetTitle(), GetIcon());
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
                Presenter.ToolTip = GetTitle();
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Gets the title to display for this document.
        /// </summary>
        /// <returns>The title to display for this document.</returns>
        protected virtual string GetTitle()
        {
            return String.Empty;
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
        /// Update the editor settings.
        /// </summary>
        public void UpdateEditorSettings()
        {
            View.ApplyEditorSettings();
        }

        /// <summary>
        /// Save changes made to the content.
        /// </summary>
        public virtual void Save()
        {            
        }

        /// <summary>
        /// Reload the document.
        /// </summary>
        /// <returns>true if the document was reloaded.</returns>
        public virtual bool Reload()
        {
            return false;
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
            return false;
        }

        /// <summary>
        /// Updates the dirty status.
        /// </summary>
        protected virtual void OnTextChanged()
        {
            IsDirty = true;
        }

        #endregion

        #region Event Handlers        

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

        /// <summary>
        /// Update the dirty status.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged();
        }

        #endregion
    }
}
