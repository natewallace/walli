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
        /// Used to parse the apex.
        /// </summary>
        private LanguageManager _language;

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
            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBrush = Brushes.LightBlue;
            textEditor.TextArea.SelectionBorder = null;
            textEditor.TextArea.SelectionForeground = null;

            textEditor.TextArea.IndentationStrategy = new ApexIndentationStrategy();

            _searchPanel = SearchPanel.Install(textEditor.TextArea);
            _searchPanel.MarkerBrush = Brushes.DarkOrange;

            _language = new LanguageManager();
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
                    //SetNavigationClasses(null);
                    //UpdateNavigation();
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

        /*
        /// <summary>
        /// The currently selected class navigation element.
        /// </summary>
        public ICodeElement SelectedClassNavigationElement
        {
            get
            {
                ComboBoxItem item = comboBoxNavigationClass.SelectedItem as ComboBoxItem;
                if (item == null)
                    return null;

                return item.Tag as ICodeElement;
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
        public IEnumerable<ICodeElement> ClassNavigationElements
        {
            get
            {
                List<ICodeElement> elements = new List<ICodeElement>();
                foreach (ComboBoxItem item in comboBoxNavigationClass.Items)
                    if (item != null && item.Tag is ICodeElement)
                        elements.Add(item.Tag as ICodeElement);

                return elements;
            }
        }

        /// <summary>
        /// The currently selected member navigation element.
        /// </summary>
        public ICodeElement SelectedMemberNavigationElement
        {
            get
            {
                ComboBoxItem item = comboBoxNavigationMember.SelectedItem as ComboBoxItem;
                if (item == null)
                    return null;

                return item.Tag as ICodeElement;
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
        public IEnumerable<ICodeElement> MemberNavigationElements
        {
            get
            {
                List<ICodeElement> elements = new List<ICodeElement>();
                foreach (ComboBoxItem item in comboBoxNavigationMember.Items)
                    if (item != null && item.Tag is ICodeElement)
                        elements.Add(item.Tag as ICodeElement);

                return elements;
            }
        }
        */

        #endregion

        #region Methods

        /// <summary>
        /// Parse the given text.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        private void ParseText(string text)
        {
            _parseData = _language.ParseApex(text);
            _colorTransformer.SetErrors(_parseData.Errors);
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

        /*
        /// <summary>
        /// Update the navigation controls based on current cursor position.
        /// </summary>
        private void UpdateNavigation()
        {
            try
            {
                _suspendNavigation = true;

                int offset = textEditor.TextArea.Caret.Offset;

                // get the current class that is selected based on cursor position
                ICodeElement mostInnerClass = null;
                ICodeElement mostOuterClass = null;
                foreach (ICodeElement element in ClassNavigationElements)
                {
                    if (element.Location.ContainsOffset(offset))
                    {
                        if (mostInnerClass == null || mostInnerClass.Location.ContainsLocation(element.Location))
                            mostInnerClass = element;
                    }

                    if (mostOuterClass == null || element.Location.ContainsLocation(mostOuterClass.Location))
                        mostOuterClass = element;
                }

                // update the class
                if (mostInnerClass != null)
                    SelectedClassNavigationElement = mostInnerClass;
                else
                    SelectedClassNavigationElement = mostOuterClass;

                // update the member
                ICodeElement memeberElement = null;
                foreach (ICodeElement element in MemberNavigationElements)
                {
                    if (element.Location.ContainsOffset(offset))
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
        private void SetNavigationClasses(ClassDeclaration apexClass)
        {
            if (apexClass == null)
            {
                comboBoxNavigationClass.Items.Clear();
                if (_parseData != null && _parseData.Elements.Length > 0)
                    apexClass = _parseData.Elements[0] as ClassDeclaration;

                if (apexClass == null)
                    SetNavigationInterfaces(null);
            }

            if (apexClass != null)
            {
                comboBoxNavigationClass.Items.Add(new ComboBoxItem()
                {
                    Tag = apexClass,
                    Content = VisualHelper.CreateIconHeader(apexClass.ToString(), "Class.png", new Thickness(0))
                });

                foreach (ClassDeclaration innerClass in apexClass.InnerClasses)
                    SetNavigationClasses(innerClass);

                foreach (InterfaceDeclaration innerInterface in apexClass.InnerInterfaces)
                    SetNavigationInterfaces(innerInterface);
            }
        }

        /// <summary>
        /// Set available interfaces that can be selected.
        /// </summary>
        /// <param name="apexInterface">The interface to generate selections from.</param>
        private void SetNavigationInterfaces(InterfaceDeclaration apexInterface)
        {
            if (apexInterface == null)
            {
                comboBoxNavigationClass.Items.Clear();
                if (_parseData != null && _parseData.Elements.Length > 0)
                    apexInterface = _parseData.Elements[0] as InterfaceDeclaration;
            }

            if (apexInterface != null)
            {
                comboBoxNavigationClass.Items.Add(new ComboBoxItem()
                {
                    Tag = apexInterface,
                    Content = VisualHelper.CreateIconHeader(apexInterface.ToString(), "Interface.png", new Thickness(0))
                });
            }
        }

        /// <summary>
        /// Set available members that can be selected.
        /// </summary>
        private void SetNavigationMembers()
        {
            comboBoxNavigationMember.Items.Clear();

            if (SelectedClassNavigationElement is ClassDeclaration)
            {
                ClassDeclaration apexClass = SelectedClassNavigationElement as ClassDeclaration;

                foreach (FieldDeclaration field in apexClass.Fields.OrderBy(f => f.Name.Text))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = field,
                        Content = VisualHelper.CreateIconHeader(field.ToString(), "Field.png", new Thickness(0))
                    });

                foreach (ConstructorDeclaration constructor in apexClass.Constructors.OrderBy(c => c.Name.Text))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = constructor,
                        Content = VisualHelper.CreateIconHeader(constructor.ToString(), "Method.png", new Thickness(0))
                    });

                foreach (PropertyDeclaration prop in apexClass.Properties.OrderBy(p => p.Name.Text))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = prop,
                        Content = VisualHelper.CreateIconHeader(prop.ToString(), "Property.png", new Thickness(0))
                    });

                foreach (MethodDeclaration method in apexClass.Methods.OrderBy(m => m.Name.Text))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = method,
                        Content = VisualHelper.CreateIconHeader(method.ToString(), "Method.png", new Thickness(0))
                    });
            }
            else if (SelectedClassNavigationElement is InterfaceDeclaration)
            {
                InterfaceDeclaration apexInterface = SelectedClassNavigationElement as InterfaceDeclaration;

                foreach (MethodDeclaration method in apexInterface.Methods.OrderBy(m => m.Name.Text))
                    comboBoxNavigationMember.Items.Add(new ComboBoxItem()
                    {
                        Tag = method,
                        Content = VisualHelper.CreateIconHeader(method.ToString(), "Method.png", new Thickness(0))
                    });
            }
        }
        */

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
                //SetNavigationMembers();

                //if (!_suspendNavigation)
                //{
                //    ICodeElement element = SelectedClassNavigationElement;
                //    if (element != null)
                //    {
                //        textEditor.SelectionStart = element.Name.Location.StartPosition;
                //        textEditor.SelectionLength = element.Name.Location.Length;
                //        textEditor.ScrollTo(element.Name.Location.StartLine, element.Name.Location.StartColumn);
                //    }
                //}
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
                //if (!_suspendNavigation)
                //{
                //    ICodeElement element = SelectedMemberNavigationElement;
                //    if (element != null)
                //    {
                //        textEditor.SelectionStart = element.Name.Location.StartPosition;
                //        textEditor.SelectionLength = element.Name.Location.Length;
                //        textEditor.ScrollTo(element.Name.Location.StartLine, element.Name.Location.StartColumn);
                //    }
                //}
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
                    //UpdateNavigation();
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
                    ParseText(Text);

                    textEditor.TextArea.TextView.Redraw();

                    //if (IsNavigationVisible)
                    //{
                    //    SetNavigationClasses(null);
                    //    UpdateNavigation();
                    //}
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

        #endregion

        #region Events

        /// <summary>
        /// Raised when the text has been changed.
        /// </summary>
        public event EventHandler TextChanged;

        #endregion
    }
}
