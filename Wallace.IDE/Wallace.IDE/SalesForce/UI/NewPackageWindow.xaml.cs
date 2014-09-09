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

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for NewPackageWindow.xaml
    /// </summary>
    public partial class NewPackageWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewPackageWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The manifest name displayed.
        /// </summary>
        public string PackageManifestName
        {
            get { return textBlockManifest.Text; }
            set { textBlockManifest.Text = value; }
        }

        /// <summary>
        /// The package name entered by the user.
        /// </summary>
        public string PackageName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        /// <summary>
        /// Indicates if the user selected the destructive option.
        /// </summary>
        public bool IsPackageDestructive
        {
            get { return (checkBoxIsDestructive.IsChecked.HasValue && checkBoxIsDestructive.IsChecked.Value); }
            set { checkBoxIsDestructive.IsChecked = value; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Set the dialog result to true and close.
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
        /// Set the dialog result to false and close.
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
