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
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using SalesForceLanguage;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.CodeModel;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for ApexEditorControl.xaml
    /// </summary>
    public partial class ApexEditorControl : UserControl, ITextEditorView
    {
        #region Fields

        /// <summary>
        /// Holds the color transformer used for syntax highlighting.
        /// </summary>
        private ApexDocumentColorizingTransformer _colorTransformer;

        /// <summary>
        /// When set to true, navigation is suspended.
        /// </summary>
        private bool _suspendNavigation;

        /// <summary>
        /// When set to true, parsing is suspended.
        /// </summary>
        private bool _suspendParse;

        /// <summary>
        /// Used to redraw the screen when a user is done typing.
        /// </summary>
        private Timer _redrawTimer;

        /// <summary>
        /// The text search panel.
        /// </summary>
        private SearchPanel _searchPanel;

        /// <summary>
        /// Supports the LanguageManager property.
        /// </summary>
        private LanguageManager _languageManager;

        /// <summary>
        /// Dispalyed when a user hovers the mouse in the document.
        /// </summary>
        private ToolTip _toolTip;

        /// <summary>
        /// Window used for code completions.
        /// </summary>
        private CompletionWindow _completionWindow;

        /// <summary>
        /// Holds the class name from the last successful parse.
        /// </summary>
        private string _className;

        /// <summary>
        /// Supports the ParseData property.
        /// </summary>
        private ParseResult _parseData;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApexEditorControl()
        {
            InitializeComponent();
            SetErrors(null);

            StreamResourceInfo info = Application.GetResourceStream(new Uri("Resources/Apex.xshd", UriKind.Relative));
            using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(info.Stream))
            {
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            _colorTransformer = new ApexDocumentColorizingTransformer();
            textEditor.TextArea.TextView.LineTransformers.Add(_colorTransformer);
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            textEditor.MouseHover += textEditor_MouseHover;
            textEditor.MouseHoverStopped += textEditor_MouseHoverStopped;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBrush = Brushes.LightBlue;
            textEditor.TextArea.SelectionBorder = null;
            textEditor.TextArea.SelectionForeground = null;

            textEditor.TextArea.IndentationStrategy = new ApexIndentationStrategy();

            _searchPanel = SearchPanel.Install(textEditor.TextArea);
            _searchPanel.MarkerBrush = Brushes.DarkOrange;
            _toolTip = new ToolTip();
            _completionWindow = null;

            _suspendNavigation = false;
            _suspendParse = false;

            _redrawTimer = new Timer();
            _redrawTimer.Interval = 750;
            _redrawTimer.AutoReset = false;
            _redrawTimer.Elapsed += redrawTimer_Elapsed;

            ParseData = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parsed code.
        /// </summary>
        public ParseResult ParseData 
        {
            get
            {
                return _parseData;
            }
            private set
            {
                _parseData = value;
                OnParseDataChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// The language manager to use.
        /// </summary>
        public LanguageManager LanguageManager
        {
            get
            {
                return _languageManager;
            }
            set
            {
                _languageManager = value;
                UpdateView();
            }
        }

        /// <summary>
        /// The Text displayed.
        /// </summary>
        public string Text
        {
            get 
            { 
                return textEditor.Text; 
            }
            set 
            {
                ParseText(value);
                _suspendParse = true;
                textEditor.Text = value;
                _suspendParse = false;

                if (IsNavigationVisible)
                {
                    SetNavigationClasses(null);
                    UpdateNavigation();
                }
            }
        }

        /// <summary>
        /// Set the visibility of the navigation control.
        /// </summary>
        public bool IsNavigationVisible
        {
            get { return borderNavigation.Visibility == Visibility.Visible; }
            set { borderNavigation.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// The currently selected class navigation element.
        /// </summary>
        public Symbol SelectedClassNavigationElement
        {
            get
            {
                ComboBoxItem item = comboBoxNavigationClass.SelectedItem as ComboBoxItem;
                if (item == null)
                    return null;

                return item.Tag as Symbol;
            }
            set
            {
                if (value == null)
                {
                    comboBoxNavigationClass.SelectedItem = null;
                }
                else
                {
                    foreach (ComboBoxItem item in comboBoxNavigationClass.Items)
                    {
                        if (item != null && value.CompareTo(item.Tag) == 0)
                        {
                            comboBoxNavigationClass.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The current class elements that can be navigated to.
        /// </summary>
        public IEnumerable<Symbol> ClassNavigationElements
        {
            get
            {
                List<Symbol> elements = new List<Symbol>();
                foreach (ComboBoxItem item in comboBoxNavigationClass.Items)
                    if (item != null && item.Tag is Symbol)
                        elements.Add(item.Tag as Symbol);

                return elements;
            }
        }

        /// <summary>
        /// The currently selected member navigation element.
        /// </summary>
        public Symbol SelectedMemberNavigationElement
        {
            get
            {
                ComboBoxItem item = comboBoxNavigationMember.SelectedItem as ComboBoxItem;
                if (item == null)
                    return null;

                return item.Tag as Symbol;
            }
            set
            {
                if (value == null)
                {
                    comboBoxNavigationMember.SelectedItem = null;
                }
                else
                {
                    foreach (ComboBoxItem item in comboBoxNavigationMember.Items)
                    {
                        if (item != null && value.CompareTo(item.Tag) == 0)
                        {
                            comboBoxNavigationMember.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The current member elements that can be navigated to.
        /// </summary>
        public IEnumerable<Symbol> MemberNavigationElements
        {
            get
            {
                List<Symbol> elements = new List<Symbol>();
                foreach (ComboBoxItem item in comboBoxNavigationMember.Items)
                    if (item != null && item.Tag is Symbol)
                        elements.Add(item.Tag as Symbol);

                return elements;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the current text and update navigation.
        /// </summary>
        private void UpdateView()
        {
            ParseText(Text);

            textEditor.TextArea.TextView.Redraw();

            if (IsNavigationVisible)
            {
                SetNavigationClasses(null);
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Parse the given text.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        private void ParseText(string text)
        {
            if (LanguageManager != null)
            {
                ParseData = LanguageManager.ParseApex(text, true, false);
                _colorTransformer.ParseData = ParseData;
            }

            if (ParseData != null && ParseData.Symbols != null)
                _className = ParseData.Symbols.Name;
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
        /// Show the code completions window.
        /// </summary>
        /// <param name="symbols">The symbols to show completions for.</param>
        private void ShowCodeCompletions(Symbol[] symbols)
        {
            if (_completionWindow == null && symbols != null && symbols.Length > 0)
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

                foreach (Symbol symbol in symbols)
                    _completionWindow.CompletionList.CompletionData.Add(new ApexCodeCompletionData(symbol));

                if (_completionWindow.CompletionList.CompletionData.Count > 0)
                {
                    _completionWindow.Show();
                    _completionWindow.Closed += delegate { _completionWindow = null; };
                }
            }
        }

        /// <summary>
        /// Update the navigation controls based on current cursor position.
        /// </summary>
        private void UpdateNavigation()
        {
            try
            {
                _suspendNavigation = true;

                TextPosition position = new TextPosition(textEditor.TextArea.Caret.Line, textEditor.TextArea.Caret.Column);

                // get the current class that is selected based on cursor position
                Symbol mostInnerClass = null;
                Symbol mostOuterClass = null;
                foreach (Symbol element in ClassNavigationElements)
                {
                    if (element.Contains(position))
                    {
                        if (mostInnerClass == null || mostInnerClass.Contains(element.Location))
                            mostInnerClass = element;
                    }

                    if (mostOuterClass == null || element.Contains(mostOuterClass.Location))
                        mostOuterClass = element;
                }

                // update the class
                if (mostInnerClass != null)
                    SelectedClassNavigationElement = mostInnerClass;
                else
                    SelectedClassNavigationElement = mostOuterClass;

                // update the member
                Symbol memeberElement = null;
                foreach (Symbol element in MemberNavigationElements)
                {
                    if (element.Contains(position))
                    {
                        memeberElement = element;
                        break;
                    }
                }

                SelectedMemberNavigationElement = memeberElement;
            }
            finally
            {
                _suspendNavigation = false;
            }
        }

        /// <summary>
        /// Set available classes that can be selected.
        /// </summary>
        /// <param name="apexClass">The class to generate selections from.</param>
        private void SetNavigationClasses(SymbolTable apexClass)
        {
            if (apexClass == null)
            {
                comboBoxNavigationClass.Items.Clear();
                if (ParseData != null)
                    apexClass = ParseData.Symbols;
            }

            if (apexClass != null)
            {
                comboBoxNavigationClass.Items.Add(new ComboBoxItem()
                {
                    Tag = apexClass,
                    Content = VisualHelper.CreateIconHeader(apexClass.ToString(), "Class.png", new Thickness(0))
                });

                foreach (SymbolTable innerClass in apexClass.InnerClasses)
                    SetNavigationClasses(innerClass);
            }
        }

        /// <summary>
        /// Set available members that can be selected.
        /// </summary>
        private void SetNavigationMembers()
        {
            comboBoxNavigationMember.Items.Clear();

            if (SelectedClassNavigationElement is SymbolTable)
            {
                SymbolTable apexClass = SelectedClassNavigationElement as SymbolTable;

                foreach (Field field in apexClass.Fields.OrderBy(f => f.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = field,
                        Content = VisualHelper.CreateIconHeader(field.ToString(), ApexCodeCompletionData.GetIconFileName(field), new Thickness(0))
                    });

                foreach (ModifiedSymbol prop in apexClass.Properties.OrderBy(p => p.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = prop,
                        Content = VisualHelper.CreateIconHeader(prop.ToString(), ApexCodeCompletionData.GetIconFileName(prop), new Thickness(0))
                    });

                foreach (Constructor constructor in apexClass.Constructors.OrderBy(c => c.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = constructor,
                        Content = VisualHelper.CreateIconHeader(constructor.ToString(), ApexCodeCompletionData.GetIconFileName(constructor), new Thickness(0))
                    });

                foreach (Method method in apexClass.Methods.OrderBy(m => m.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = method,
                        Content = VisualHelper.CreateIconHeader(method.ToString(), ApexCodeCompletionData.GetIconFileName(method), new Thickness(0))
                    });
            }
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

        /// <summary>
        /// Raises the ParseDataChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnParseDataChanged(EventArgs e)
        {
            if (ParseDataChanged != null)
                ParseDataChanged(this, e);
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
                if (!_suspendParse)
                {
                    _redrawTimer.Stop();
                    _redrawTimer.Start();
                }

                OnTextChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the members that are available and navigate to the selected class.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxNavigationClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SetNavigationMembers();

                if (!_suspendNavigation)
                {
                    Symbol element = SelectedClassNavigationElement;
                    if (element != null)
                    {
                        textEditor.SelectionStart = textEditor.Document.GetOffset(element.Location.Line, element.Location.Column);
                        textEditor.SelectionLength = element.Name.Length;
                        textEditor.ScrollTo(element.Location.Line, element.Location.Column);
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Navigate to the selected member.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxNavigationMember_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_suspendNavigation)
                {
                    Symbol element = SelectedMemberNavigationElement;
                    if (element != null)
                    {
                        textEditor.SelectionStart = textEditor.Document.GetOffset(element.Location.Line, element.Location.Column);
                        textEditor.SelectionLength = element.Name.Length;
                        textEditor.ScrollTo(element.Location.Line, element.Location.Column);
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the Navigation menus when the caret position is changed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsNavigationVisible)
                {
                    UpdateNavigation();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Redraw the text view.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void redrawTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateView();
                });
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Check for key commands.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    switch (e.Key)
                    {
                        case Key.G:
                            e.Handled = true;
                            App.Instance.GetFunction<TextGoToLineFunction>().Execute();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close tooltip.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            try
            {
                _toolTip.IsOpen = false;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Open tooltip.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_MouseHover(object sender, MouseEventArgs e)
        {
            try
            {
                if (ParseData != null)
                {
                    TextViewPosition? position = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
                    if (position != null)
                    {
                        if (ParseData.ErrorsByLine.ContainsKey(position.Value.Line))
                        {
                            foreach (LanguageError err in ParseData.ErrorsByLine[position.Value.Line])
                            {
                                if (err.Location.Contains(position.Value.Line, position.Value.Column))
                                {
                                    _toolTip.PlacementTarget = this;
                                    _toolTip.Content = err.Message;
                                    _toolTip.IsOpen = true;
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Show completion window.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (Language != null && _completionWindow == null)
                {
                    Symbol[] symbols = null;

                    // calculate completions
                    if (e.Text == ".")
                    {
                        symbols = LanguageManager.Completion.GetCodeCompletionsDot(
                            new DocumentCharStream(textEditor.Document, textEditor.TextArea.Caret.Offset - 1),
                            _className,
                            new TextPosition(textEditor.TextArea.Caret.Line, textEditor.TextArea.Caret.Column));
                    }
                    // modify indentation when closing bracket is entered.
                    else if (e.Text == "}")
                    {
                        DocumentLine endLine = textEditor.Document.GetLineByOffset(textEditor.TextArea.Caret.Offset);
                        string endLineText = textEditor.Document.GetText(endLine.Offset, endLine.Length);
                        if (endLineText.Trim() == "}")
                        {
                            int openCount = 1;
                            for (int offset = textEditor.TextArea.Caret.Offset - 2; offset >= 0; offset--)
                            {
                                char c = textEditor.Document.GetCharAt(offset);
                                switch (c)
                                {
                                    case '{':
                                        openCount--;
                                        break;

                                    case '}':
                                        openCount++;
                                        break;

                                    default:
                                        break;
                                }

                                if (openCount == 0)
                                {
                                    DocumentLine startLine = textEditor.Document.GetLineByOffset(offset);
                                    string startLineText = textEditor.Document.GetText(startLine.Offset, startLine.Length);

                                    // get the padding at the start of the end line
                                    int paddingCount = 0;
                                    foreach (char endLineChar in endLineText)
                                    {
                                        if (endLineChar == ' ' || endLineChar == '\t')
                                            paddingCount++;
                                        else
                                            break;
                                    }

                                    // get the padding at the start of the start line
                                    StringBuilder sb = new StringBuilder();
                                    foreach (char lineChar in startLineText)
                                    {
                                        if (lineChar == ' ' || lineChar == '\t')
                                            sb.Append(lineChar);
                                        else
                                            break;
                                    }

                                    // replace padding at the start of the end line with padding at the start of the start line
                                    textEditor.Document.Remove(endLine.Offset, paddingCount);
                                    textEditor.Document.Insert(endLine.Offset, sb.ToString());
                                    break;
                                }
                            }
                        }
                    }

                    // show completions
                    ShowCodeCompletions(symbols);
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Process text input while completion window is open.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (_completionWindow != null && e.Text.Length > 0)
                {
                    if (!char.IsLetterOrDigit(e.Text[0]))
                    {
                        _completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
                else if (_completionWindow == null && e.Text.Length == 1 && Char.IsLetter(e.Text[0]))
                {
                    // show completions
                    ShowCodeCompletions(LanguageManager.Completion.GetCodeCompletionsLetter(
                        new DocumentCharStream(textEditor.Document, textEditor.TextArea.Caret.Offset),
                        _className,
                        new TextPosition(textEditor.TextArea.Caret.Line, textEditor.TextArea.Caret.Column)));
                }
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

        /// <summary>
        /// Raised when the parse data has been changed.
        /// </summary>
        public event EventHandler ParseDataChanged;

        #endregion
    }
}
