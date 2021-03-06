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
using System.Windows.Resources;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using SalesForceLanguage;
using SalesForceLanguage.Apex;
using Wallace.IDE.Framework;
using System.Text;

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

        /// <summary>
        /// Window used for code completions.
        /// </summary>
        private CompletionWindow _completionWindow;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public VisualForceEditorControl()
        {
            InitializeComponent();
            SetErrors(null);
            _searchPanel = SearchPanel.Install(textEditor.TextArea);

            ApplyEditorSettings();

            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBorder = null;            
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.TextEntering += TextArea_TextEntering;

            _completionWindow = null;
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

        /// <summary>
        /// The language manager to use.
        /// </summary>
        public LanguageManager LanguageManager { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Apply the editor settings as currently configured for the application.
        /// </summary>
        public void ApplyEditorSettings()
        {
            textEditor.SyntaxHighlighting = EditorSettings.VisualForceSettings.HighlightDefinition;
            textEditor.FontFamily = EditorSettings.VisualForceSettings.FontFamily;
            textEditor.FontSize = EditorSettings.VisualForceSettings.FontSize;
            textEditor.Foreground = new SolidColorBrush(EditorSettings.VisualForceSettings.Foreground);
            textEditor.Background = new SolidColorBrush(EditorSettings.VisualForceSettings.Background);

            if (EditorSettings.VisualForceSettings.SelectionForeground.HasValue)
                textEditor.TextArea.SelectionForeground = new SolidColorBrush(EditorSettings.VisualForceSettings.SelectionForeground.Value);
            else
                textEditor.TextArea.SelectionForeground = null;

            if (EditorSettings.VisualForceSettings.SelectionBackground.HasValue)
                textEditor.TextArea.SelectionBrush = new SolidColorBrush(EditorSettings.VisualForceSettings.SelectionBackground.Value);
            else
                textEditor.TextArea.SelectionBrush = null;

            _searchPanel.MarkerBrush = new SolidColorBrush(EditorSettings.VisualForceSettings.FindResultBackground);

            textEditor.TextArea.TextView.Redraw();
        }

        /// <summary>
        /// Show the code completions window.
        /// </summary>
        /// <param name="items">The items to show completions for.</param>
        /// <param name="isTag">true if the items are tags, false if they are attributes.</param>
        private void ShowCodeCompletions(string[] items, bool isTag)
        {
            if (_completionWindow == null && items != null && items.Length > 0)
            {
                _completionWindow = new CompletionWindow(textEditor.TextArea);
                _completionWindow.Style = null;
                _completionWindow.CompletionList.Style = null;
                _completionWindow.SizeToContent = SizeToContent.WidthAndHeight;
                _completionWindow.MaxWidth = 400;
                _completionWindow.AllowsTransparency = true;
                _completionWindow.Background = Brushes.Transparent;
                _completionWindow.SnapsToDevicePixels = true;
                _completionWindow.UseLayoutRounding = true;

                foreach (string tag in items)
                    _completionWindow.CompletionList.CompletionData.Add(new VisualForceCompletionData(tag, isTag));

                if (_completionWindow.CompletionList.CompletionData.Count > 0)
                {
                    _completionWindow.Show();
                    _completionWindow.Closed += delegate { _completionWindow = null; };
                }
            }
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
            buttonCloseErrors.Visibility = scrollViewerErrors.Visibility;
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

        /// <summary>
        /// Do code completions.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                if (LanguageManager != null)
                {
                    if (_completionWindow == null)
                    {
                        // tags
                        if (e.Text == "<")
                        {
                            ShowCodeCompletions(LanguageManager.Completion.GetVisualForceCompletionsTags(
                                new DocumentCharStream(textEditor.Document, textEditor.TextArea.Caret.Offset)),
                                true);
                        }
                        // attributes
                        else if (e.Text == " ")
                        {
                            ShowCodeCompletions(LanguageManager.Completion.GetVisualForceCompletionsAttributes(
                                new DocumentCharStream(textEditor.Document, textEditor.TextArea.Caret.Offset)),
                                false);
                        }
                    }
                    // closing tags
                    else if (e.Text == "/")
                    {
                        if (textEditor.TextArea.Caret.Offset > 1 && textEditor.Document.GetCharAt(textEditor.TextArea.Caret.Offset - 2) == '<')
                        {
                            _completionWindow.Close();
                            ShowCodeCompletions(LanguageManager.Completion.GetVisualForceCompletionsTags(
                                new DocumentCharStream(textEditor.Document, textEditor.TextArea.Caret.Offset)),
                                true);
                        }
                        else
                        {
                            _completionWindow.Close();
                        }
                    }
                    // close window
                    else if (e.Text == ">" || e.Text == " ")
                    {
                        _completionWindow.Close();
                    }
                }
                
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Do insertions when certain characters are entered.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                if (_completionWindow != null && e.Text.Length > 0)
                {
                    if (!char.IsLetterOrDigit(e.Text[0]) && 
                        e.Text[0] != '_' &&
                        e.Text[0] != ':' && 
                        e.Text[0] != '/')
                    {
                        _completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the errors view.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCloseErrors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                scrollViewerErrors.Visibility = Visibility.Collapsed;
                buttonCloseErrors.Visibility = Visibility.Collapsed;
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
