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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Search;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for VisualForceEditorControl.xaml
    /// </summary>
    public partial class VisualForceEditorControl : UserControl, ITextEditorView
    {
        #region Fields

        /// <summary>
        /// The text search panel.
        /// </summary>
        private SearchPanel _searchPanel;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public VisualForceEditorControl()
        {
            InitializeComponent();
            SetErrors(null);

            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBrush = Brushes.LightBlue;
            textEditor.TextArea.SelectionBorder = null;
            textEditor.TextArea.SelectionForeground = null;

            _searchPanel = SearchPanel.Install(textEditor.TextArea);
            _searchPanel.MarkerBrush = Brushes.DarkOrange;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Text displayed.
        /// </summary>
        public string Text
        {
            get { return textEditor.Text; }
            set { textEditor.Text = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Give focus to the text input.
        /// </summary>
        public void FocusText()
        {
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                (Action)delegate { textEditor.Focus(); });
        }

        /// <summary>
        /// Open the text search dialog.
        /// </summary>
        public void SearchText()
        {
            _searchPanel.Open();

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                (Action)delegate { _searchPanel.Reactivate(); });
        }

        /// <summary>
        /// Copy selected text to the clipboard.
        /// </summary>
        public void CopyText()
        {
            textEditor.Copy();
        }

        /// <summary>
        /// Delete selected text and copy it to the clipboard.
        /// </summary>
        public void CutText()
        {
            textEditor.Cut();
        }

        /// <summary>
        /// Paste text from the clipboard to the editor.
        /// </summary>
        public void PasteText()
        {
            textEditor.Paste();
        }

        /// <summary>
        /// Undo the last test change.
        /// </summary>
        public void UndoText()
        {
            textEditor.Undo();
        }

        /// <summary>
        /// Redo the last text change.
        /// </summary>
        public void RedoText()
        {
            textEditor.Redo();
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        public void SelectAllText()
        {
            textEditor.SelectAll();
        }

        /// <summary>
        /// Go to the given line number in the document.
        /// </summary>
        /// <param name="line">The line number to go to. (1 based)</param>
        public void GotToLine(int line)
        {
            line = Math.Min(line, textEditor.Document.LineCount);
            line = Math.Max(line, 1);
            line--;
            textEditor.Select(textEditor.Document.Lines[line].Offset, textEditor.Document.Lines[line].Length);
            textEditor.ScrollTo(line, 0);
        }

        /// <summary>
        /// Set errors that are displayed.
        /// </summary>
        /// <param name="errors">The errors to display.  Null or an empty collection clears all errors.</param>
        public void SetErrors(IEnumerable<string> errors)
        {
            listBoxErrors.Items.Clear();

            if (errors != null)
                foreach (string error in errors)
                    listBoxErrors.Items.Add(VisualHelper.CreateIconHeader(error, "Error.png"));

            scrollViewerErrors.Visibility = (listBoxErrors.Items.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the syntax coloring.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OnTextChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the text has been changed.
        /// </summary>
        public event EventHandler TextChanged;

        #endregion
    }
}
