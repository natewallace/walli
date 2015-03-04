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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for DiffViewControl.xaml
    /// </summary>
    public partial class TextViewControl : UserControl, ITextEditorView
    {
        #region Fields

        /// <summary>
        /// The text search panel.
        /// </summary>
        private SearchPanel _searchPanel;

        /// <summary>
        /// The color transformer used to color diff lines.
        /// </summary>
        private DiffDocumentColorizingTransformer _colorTransformer;

        /// <summary>
        /// Supports the HighlightDiffs property.
        /// </summary>
        private bool _highlightDiffs;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextViewControl()
        {
            InitializeComponent();

            _searchPanel = SearchPanel.Install(textEditor.TextArea);

            ApplyEditorSettings();

            textEditor.IsReadOnly = true;
            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBorder = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set to true to highlight diffs.
        /// </summary>
        public bool HighlightDiffs
        {
            get { return _highlightDiffs; }
            set
            {
                _highlightDiffs = value;
                UpdateHighlights();
            }
        }

        /// <summary>
        /// Returns the next diff line number from the caret or -1 if there isn't a next.
        /// </summary>
        public int NextDiffLine
        {
            get
            {
                if (_colorTransformer == null)
                    return -1;

                // combine the adds and deletes
                List<int> lineChanges = new List<int>();
                lineChanges.AddRange(_colorTransformer.LineAdds);
                lineChanges.AddRange(_colorTransformer.LineDeletes);
                lineChanges.Sort();

                // find the first line from the caret with a diff
                int currentLine = textEditor.TextArea.Caret.Line;
                foreach (int line in lineChanges)
                {
                    if (currentLine > line)
                        continue;

                    if (currentLine == line)
                        currentLine++;
                    else
                        return line;
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the previous diff line number from the caret or -1 if there isn't a previous.
        /// </summary>
        public int PreviousDiffLine
        {
            get
            {
                if (_colorTransformer == null)
                    return -1;

                // combine the adds and deletes
                List<int> lineChanges = new List<int>();
                lineChanges.AddRange(_colorTransformer.LineAdds);
                lineChanges.AddRange(_colorTransformer.LineDeletes);
                lineChanges.Sort();
                lineChanges.Reverse();

                // find the first line from the caret with a diff
                int currentLine = textEditor.TextArea.Caret.Line;
                foreach (int line in lineChanges)
                {
                    if (currentLine < line)
                        continue;

                    if (currentLine == line)
                        currentLine--;
                    else
                        return line;
                }

                return -1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Navigate to the next diff from the current cursor position.
        /// </summary>
        public void GotToNextDiff()
        {
            int line = NextDiffLine;
            if (line != -1)
                GotToLine(line);
        }

        /// <summary>
        /// Navigate to the previous diff from the current cursor position.
        /// </summary>
        public void GotToPreviousDiff()
        {
            int line = PreviousDiffLine;
            if (line != -1)
                GotToLine(line);
        }

        /// <summary>
        /// Update the highlights.
        /// </summary>
        private void UpdateHighlights()
        {
            if (HighlightDiffs)
            {
                textEditor.ShowLineNumbers = false;

                if (_colorTransformer != null)
                    textEditor.TextArea.TextView.LineTransformers.Remove(_colorTransformer);

                if (Text != null)
                {
                    using (System.IO.StringReader reader = new System.IO.StringReader(Text))
                    {
                        HashSet<int> lineAdds = new HashSet<int>();
                        HashSet<int> lineDeletes = new HashSet<int>();

                        string line = null;
                        int lineNumber = 1;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Length > 0)
                            {
                                if (line[0] == '+')
                                    lineAdds.Add(lineNumber);
                                else if (line[0] == '-')
                                    lineDeletes.Add(lineNumber);
                            }

                            lineNumber++;
                        }

                        _colorTransformer = new DiffDocumentColorizingTransformer(lineAdds, lineDeletes);
                    }

                    textEditor.TextArea.TextView.LineTransformers.Add(_colorTransformer);
                }
            }
            else
            {
                textEditor.ShowLineNumbers = true;

                if (_colorTransformer != null)
                    textEditor.TextArea.TextView.LineTransformers.Remove(_colorTransformer);
            }
        }

        #endregion

        #region ITextEditorView Members

        /// <summary>
        /// The Text displayed.
        /// </summary>
        public string Text
        {
            get { return textEditor.Text; }
            set 
            {
                textEditor.Text = value;
                UpdateHighlights();
            }
        }

        /// <summary>
        /// When true the view will be read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
            set { }
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
        /// Not supported.
        /// </summary>
        /// <param name="errors">Ignored.</param>
        public void SetErrors(IEnumerable<string> errors)
        {
        }

        /// <summary>
        /// This event is never raised.
        /// </summary>
        public event EventHandler TextChanged;

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
        /// Not supported.  Does nothing.
        /// </summary>
        public void CutText()
        {
        }

        /// <summary>
        /// Not supported.  Does nothing.
        /// </summary>
        public void PasteText()
        {
        }

        /// <summary>
        /// Not supported.  Does nothing.
        /// </summary>
        public void UndoText()
        {
        }

        /// <summary>
        /// Not supported.  Does nothing.
        /// </summary>
        public void RedoText()
        {
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
        /// Apply the editor settings as currently configured for the application.
        /// </summary>
        public void ApplyEditorSettings()
        {
            textEditor.FontFamily = EditorSettings.ApexSettings.FontFamily;
            textEditor.FontSize = EditorSettings.ApexSettings.FontSize;
            textEditor.Foreground = Brushes.DimGray;
            textEditor.Background = Brushes.White;

            if (EditorSettings.ApexSettings.SelectionForeground.HasValue)
                textEditor.TextArea.SelectionForeground = new SolidColorBrush(EditorSettings.ApexSettings.SelectionForeground.Value);
            else
                textEditor.TextArea.SelectionForeground = null;

            if (EditorSettings.ApexSettings.SelectionBackground.HasValue)
                textEditor.TextArea.SelectionBrush = new SolidColorBrush(EditorSettings.ApexSettings.SelectionBackground.Value);
            else
                textEditor.TextArea.SelectionBrush = null;

            _searchPanel.MarkerBrush = new SolidColorBrush(EditorSettings.ApexSettings.FindResultBackground);

            textEditor.TextArea.TextView.Redraw();
        }

        #endregion
    }
}
