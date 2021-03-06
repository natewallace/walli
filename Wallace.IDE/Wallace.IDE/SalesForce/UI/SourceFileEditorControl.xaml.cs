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

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Search;
using Wallace.IDE.Framework;
using Wallace.IDE.Framework.UI;
using System.Text;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for SourceFileEditorControl.xaml
    /// </summary>
    public partial class SourceFileEditorControl : UserControl, ITextEditorView
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
        public SourceFileEditorControl()
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

        /// <summary>
        /// When true the view will be read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return textEditor.IsReadOnly; }
            set { textEditor.IsReadOnly = value; }
        }

        /// <summary>
        /// The current line number that the caret is on.
        /// </summary>
        public int CurrentLineNumber
        {
            get
            {
                return textEditor.Document.GetLineByOffset(textEditor.CaretOffset).LineNumber;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply the editor settings as currently configured for the application.
        /// </summary>
        public void ApplyEditorSettings()
        {
        }

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
        /// Open the text search dialog with the given text.
        /// </summary>
        /// <param name="text">The text to search with.</param>
        public void SearchText(string text)
        {
            _searchPanel.SearchPattern = text;

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                (Action)delegate { SearchText(); });
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
        /// Insert the given text.
        /// </summary>
        /// <param name="text">The text to insert.</param>
        /// <param name="matchPadding">
        /// When set to true the left padding from the current cursor will be applied to each line inserted.
        /// </param>
        public void InsertText(string text, bool matchPadding)
        {
            if (!matchPadding)
            {
                textEditor.Document.Insert(textEditor.CaretOffset, text);
            }
            else if (text != null)
            {
                // get current line padding
                StringBuilder sb = new StringBuilder();
                for (int index = textEditor.TextArea.Caret.Offset - textEditor.TextArea.Caret.Column;
                     index < textEditor.TextArea.Caret.Offset;
                     index++)
                {
                    char c = textEditor.Document.GetCharAt(index);
                    if (c == ' ' || c == '\t')
                        sb.Append(c);
                    else
                        break;
                }
                string padding = sb.ToString();

                using (System.IO.StringReader reader = new System.IO.StringReader(text))
                {
                    string line = reader.ReadLine();
                    textEditor.Document.Insert(textEditor.CaretOffset, String.Format("{0}{1}", line, Environment.NewLine));

                    while ((line = reader.ReadLine()) != null)
                        textEditor.Document.Insert(textEditor.CaretOffset, String.Format("{0}{1}{2}", padding, line, Environment.NewLine));
                }
            }
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
