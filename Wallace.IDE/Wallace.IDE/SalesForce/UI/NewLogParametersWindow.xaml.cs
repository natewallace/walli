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
    /// Interaction logic for NewLogParametersWindow.xaml
    /// </summary>
    public partial class NewLogParametersWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewLogParametersWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The text displayed in the save button.
        /// </summary>
        public string SaveButtonText
        {
            get { return buttonCreate.Content as string; }
            set { buttonCreate.Content = value; }
        }

        /// <summary>
        /// The scope.
        /// </summary>
        public string Scope
        {
            get { return textBoxScope.Text; }
            set { textBoxScope.Text = value; }
        }

        /// <summary>
        /// The traced entity.
        /// </summary>
        public string TracedEntity
        {
            get { return textBoxTracedEntity.Text; }
            set { textBoxTracedEntity.Text = value; }
        }

        /// <summary>
        /// The expiration date entered by the user.
        /// </summary>
        public DateTime? ExpirationDate
        {
            get
            {
                DateTime result = DateTime.MinValue;
                if (DateTime.TryParse(textBoxExpirationDate.Text, out result))
                    return result;

                return null;
            }
            set
            {
                if (value == null)
                {
                    textBoxExpirationDate.Text = String.Empty;
                }
                else
                {
                    textBoxExpirationDate.Text = value.Value.ToString();
                }
            }
        }

        /// <summary>
        /// The log level for Code.
        /// </summary>
        public LogLevel LogLevelCode
        {
            get { return (LogLevel)comboBoxCode.SelectedItem; }
            set { comboBoxCode.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for VisualForce.
        /// </summary>
        public LogLevel LogLevelVisualForce
        {
            get { return (LogLevel)comboBoxVisualForce.SelectedItem; }
            set { comboBoxVisualForce.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for Profiling.
        /// </summary>
        public LogLevel LogLevelProfiling
        {
            get { return (LogLevel)comboBoxProfiling.SelectedItem; }
            set { comboBoxProfiling.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for Callout.
        /// </summary>
        public LogLevel LogLevelCallout
        {
            get { return (LogLevel)comboBoxCallout.SelectedItem; }
            set { comboBoxCallout.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for Database.
        /// </summary>
        public LogLevel LogLevelDatabase
        {
            get { return (LogLevel)comboBoxDatabase.SelectedItem; }
            set { comboBoxDatabase.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for System.
        /// </summary>
        public LogLevel LogLevelSystem
        {
            get { return (LogLevel)comboBoxSystem.SelectedItem; }
            set { comboBoxSystem.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for Validation.
        /// </summary>
        public LogLevel LogLevelValidation
        {
            get { return (LogLevel)comboBoxValidation.SelectedItem; }
            set { comboBoxValidation.SelectedItem = value; }
        }

        /// <summary>
        /// The log level for Workflow.
        /// </summary>
        public LogLevel LogLevelWorkflow
        {
            get { return (LogLevel)comboBoxWorkflow.SelectedItem; }
            set { comboBoxWorkflow.SelectedItem = value; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Cancel the creation of new log parameters.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// OK the creation of new log parameters.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCreate_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        #endregion
    }
}
