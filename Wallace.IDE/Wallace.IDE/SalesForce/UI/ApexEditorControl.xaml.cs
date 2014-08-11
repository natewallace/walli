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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using ICSharpCode.AvalonEdit;
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
        /// The most recent parse data.
        /// </summary>
        private ParseResult _parseData;

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
            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBrush = Brushes.LightBlue;
            textEditor.TextArea.SelectionBorder = null;
            textEditor.TextArea.SelectionForeground = null;

            textEditor.TextArea.IndentationStrategy = new ApexIndentationStrategy();

            _searchPanel = SearchPanel.Install(textEditor.TextArea);
            _searchPanel.MarkerBrush = Brushes.DarkOrange;
            _toolTip = new ToolTip();

            _suspendNavigation = false;
            _suspendParse = false;

            _redrawTimer = new Timer();
            _redrawTimer.Interval = 750;
            _redrawTimer.AutoReset = false;
            _redrawTimer.Elapsed += redrawTimer_Elapsed;

            _parseData = null;
        }

        #endregion

        #region Properties

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
                _parseData = LanguageManager.ParseApex(text);
                _colorTransformer.ParseData = _parseData;
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
                if (_parseData != null)
                    apexClass = _parseData.Symbols;
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

                foreach (VisibilitySymbol prop in apexClass.Properties.OrderBy(p => p.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = prop,
                        Content = VisualHelper.CreateIconHeader(prop.ToString(), "Property.png", new Thickness(0))
                    });

                foreach (Constructor constructor in apexClass.Constructors.OrderBy(c => c.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = constructor,
                        Content = VisualHelper.CreateIconHeader(constructor.ToString(), "Method.png", new Thickness(0))
                    });

                foreach (Method method in apexClass.Methods.OrderBy(m => m.Name))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = method,
                        Content = VisualHelper.CreateIconHeader(method.ToString(), "Method.png", new Thickness(0))
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
                if (_parseData != null)
                {
                    TextViewPosition? position = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
                    if (position != null)
                    {
                        if (_parseData.ErrorsByLine.ContainsKey(position.Value.Line))
                        {
                            foreach (LanguageError err in _parseData.ErrorsByLine[position.Value.Line])
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

        #endregion

        #region Events

        /// <summary>
        /// Raised when the text has been changed.
        /// </summary>
        public event EventHandler TextChanged;

        #endregion
    }
}
