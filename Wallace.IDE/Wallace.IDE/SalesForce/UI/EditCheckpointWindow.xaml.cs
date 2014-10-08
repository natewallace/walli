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
using System.Windows.Shapes;
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for NewCheckpointWindow.xaml
    /// </summary>
    public partial class EditCheckpointWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditCheckpointWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the file for the checkpoint.
        /// </summary>
        public string FileName
        {
            get { return textBlockFileName.Text; }
            set { textBlockFileName.Text = value; }
        }

        /// <summary>
        /// The line number for the checkpoint.
        /// </summary>
        public string LineNumber
        {
            get { return textBlockLineNumber.Text; }
            set { textBlockLineNumber.Text = value; }
        }

        /// <summary>
        /// The number of iterations for the checkpoint.
        /// </summary>
        public int Iteration
        {
            get { return int.Parse(textBoxIteration.Text); }
            set { textBoxIteration.Text = value.ToString(); }
        }

        /// <summary>
        /// Indicates if a heap dump will be captured.
        /// </summary>
        public bool HeapDump
        {
            get { return checkBoxHeapDump.IsChecked.HasValue && checkBoxHeapDump.IsChecked.Value; }
            set { checkBoxHeapDump.IsChecked = value; }
        }

        /// <summary>
        /// The selected script type.
        /// </summary>
        public CheckpointScriptType ScriptType
        {
            get { return (CheckpointScriptType)comboBoxScriptType.SelectedItem; }
            set { comboBoxScriptType.SelectedItem = value; }
        }

        /// <summary>
        /// The entered script.
        /// </summary>
        public string Script
        {
            get { return textBoxScript.Text; }
            set { textBoxScript.Text = value; }
        }

        /// <summary>
        /// The text that appears in the action button.
        /// </summary>
        public string ActionText
        {
            get { return buttonCreate.Content as string; }
            set { buttonCreate.Content = value; }
        }
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// Close the dialog with an ok status.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the dialog with a cancel status.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
