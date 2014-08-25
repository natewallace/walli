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
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for DataEditControl.xaml
    /// </summary>
    public partial class DataEditControl : UserControl
    {
        #region Fields

        /// <summary>
        /// The context menu which is displayed when a row is clicked on.
        /// </summary>
        private ContextMenu _contextMenu;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataEditControl()
        {
            InitializeComponent();

            Data = null;

            StreamResourceInfo info = Application.GetResourceStream(new Uri("Resources/SOQL.xshd", UriKind.Relative));
            using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(info.Stream))
            {
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.SelectionBrush = Brushes.LightBlue;
            textEditor.TextArea.SelectionBorder = null;
            textEditor.TextArea.SelectionForeground = null;

            textEditor.TextArea.TextView.Drop += textEditor_Drop;

            // setup context menu
            _contextMenu = new ContextMenu();
            _contextMenu.Style = Application.Current.FindResource("ChromeContextMenuStyle") as Style;

            MenuItem menuItemDeleteRecord = new MenuItem();
            menuItemDeleteRecord.Header = "Delete record";
            Image imageDeleteRecord = new Image();
            imageDeleteRecord.Source = VisualHelper.LoadBitmap("Delete.png");
            menuItemDeleteRecord.Icon = imageDeleteRecord;
            menuItemDeleteRecord.Click += menuItemDeleteRecord_Click;
            _contextMenu.Items.Add(menuItemDeleteRecord);

            MenuItem menuItemCopyValue = new MenuItem();
            menuItemCopyValue.Header = "Copy value to clipboard";
            Image imageCopyValue = new Image();
            imageCopyValue.Source = VisualHelper.LoadBitmap("Copy.png");
            menuItemCopyValue.Icon = imageCopyValue;
            menuItemCopyValue.Click += menuItemCopyValue_Click;
            _contextMenu.Items.Add(menuItemCopyValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The SOQL text entered by the user.
        /// </summary>
        public string SOQLText
        {
            get { return textEditor.Text; }
            set { textEditor.Text = value; }
        }

        /// <summary>
        /// The data displayed.
        /// </summary>
        public DataView Data
        {
            get 
            { 
                return dataGridResults.ItemsSource as DataView; 
            }
            set 
            {
                if (dataGridResults.ItemsSource is DataView)
                {
                    (dataGridResults.ItemsSource as DataView).ListChanged -= DataEditControl_ListChanged;
                }

                dataGridResults.ItemsSource = null;
                dataGridResults.Columns.Clear();

                if (value != null)
                {
                    foreach (DataColumn column in value.Table.Columns)
                    {
                        DataGridTextColumn c = new DataGridTextColumn();
                        c.Binding = new Binding(column.ColumnName);
                        c.Header = column.ColumnName.Replace("_", "__");
                        c.IsReadOnly = column.ReadOnly;
                        dataGridResults.Columns.Add(c);
                    }

                    dataGridResults.ItemsSource = value;
                    value.ListChanged += DataEditControl_ListChanged;

                    stackPanelResultsTitle.Visibility = Visibility.Visible;
                    dataGridResults.Visibility = Visibility.Visible;
                    dockPanelResultButtons.Visibility = Visibility.Visible;
                }
                else
                {
                    stackPanelResultsTitle.Visibility = Visibility.Collapsed;
                    dataGridResults.Visibility = Visibility.Collapsed;
                    dockPanelResultButtons.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Result text displayed.
        /// </summary>
        public string DataResultText
        {
            get { return textBlockResultText.Text; }
            set { textBlockResultText.Text = value; }
        }

        /// <summary>
        /// Enable/Disable the commit function.
        /// </summary>
        public bool IsCommitEnabled
        {
            get { return buttonCommit.IsEnabled; }
            set { buttonCommit.IsEnabled = value; }
        }

        /// <summary>
        /// Set the visibility of the next function.
        /// </summary>
        public bool IsNextVisible
        {
            get { return (buttonNext.Visibility == System.Windows.Visibility.Visible); }
            set { buttonNext.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
        }

        /// <summary>
        /// Enable/Disable the Execute button.
        /// </summary>
        public bool IsExecuteEnabled
        {
            get { return buttonExecute.IsEnabled; }
            set { buttonExecute.IsEnabled = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Give focus to the text input.
        /// </summary>
        public void FocusText()
        {
            textEditor.Focus();
        }

        /// <summary>
        /// Raises the ExecuteClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnExecuteClick(EventArgs e)
        {
            if (ExecuteClick != null)
                ExecuteClick(this, e);
        }

        /// <summary>
        /// Raises the CommitClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnCommitClick(EventArgs e)
        {
            if (CommitClick != null)
                CommitClick(this, e);
        }

        /// <summary>
        /// Raises the NextClick event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnNextClick(EventArgs e)
        {
            if (NextClick != null)
                NextClick(this, e);
        }

        /// <summary>
        /// Raises the SOQLTextChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnSOQLTextChanged(EventArgs e)
        {
            if (SOQLTextChanged != null)
                SOQLTextChanged(this, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks the execute button.
        /// </summary>
        public event EventHandler ExecuteClick;

        /// <summary>
        /// Raised when the user clicks the commit button.
        /// </summary>
        public event EventHandler CommitClick;

        /// <summary>
        /// Raised when the user clicks the next button.
        /// </summary>
        public event EventHandler NextClick;

        /// <summary>
        /// Raised when the SOQLText is changed.
        /// </summary>
        public event EventHandler SOQLTextChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raise the ExecuteClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnExecuteClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raises the CommitClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCommit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCommitClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the NextClick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnNextClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the ExecuteClick event when F5 key is pressed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F5)
                    OnExecuteClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Raise the SOQLTextChanged event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OnSOQLTextChanged(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Give focus to the text editor after a drag drop.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void textEditor_Drop(object sender, DragEventArgs e)
        {
            try
            {
                textEditor.Focus();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update display of changed rows.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DataEditControl_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            try
            {
                switch (e.ListChangedType)
                {
                    case System.ComponentModel.ListChangedType.ItemChanged:
                        DataGridRow changeRow = dataGridResults.ItemContainerGenerator.ContainerFromIndex(e.NewIndex) as DataGridRow;
                        if (changeRow != null && changeRow.Header == null)
                        {
                            Image image = new Image();
                            image.Source = VisualHelper.LoadBitmap("Pencil.png");
                            changeRow.Header = image;
                            changeRow.Background = Brushes.Orange;
                        }
                        break;

                    case System.ComponentModel.ListChangedType.ItemAdded:
                        DataGridRow addRow = dataGridResults.ItemContainerGenerator.ContainerFromIndex(e.NewIndex) as DataGridRow;
                        if (addRow != null)
                        {
                            Image image = new Image();
                            image.Source = VisualHelper.LoadBitmap("Add.png");
                            addRow.Header = image;
                            addRow.Background = Brushes.Orange;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Delete the currently selected record.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void menuItemDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridResults.SelectedIndex != -1 && dataGridResults.ItemsSource is DataView)
                {
                    (dataGridResults.ItemsSource as DataView)[dataGridResults.SelectedIndex].Delete();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Copy the cell that is under the current mouse position.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void menuItemCopyValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                

                if (dataGridResults.SelectedCells.Count == 1)
                {
                    DataRowView row = dataGridResults.SelectedCells[0].Item as DataRowView;
                    DataGridColumn column = dataGridResults.SelectedCells[0].Column;
                    if (row != null && row.Row != null && column != null && row.Row.ItemArray[column.DisplayIndex] != null)
                    {
                        Clipboard.SetText(row.Row.ItemArray[column.DisplayIndex].ToString());
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Show context menu.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void dataGridResults_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGridRow item = VisualHelper.GetAncestor<DataGridRow>(e.OriginalSource as DependencyObject);
                if (item != null)
                {
                    dataGridResults.SelectedIndex = item.GetIndex();
                    _contextMenu.PlacementTarget = item;
                    _contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    _contextMenu.IsOpen = true;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
