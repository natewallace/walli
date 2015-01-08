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
    /// Interaction logic for PackageDeployWindow.xaml
    /// </summary>
    public partial class PackageDeployWindow : Window
    {
        #region Fields

        /// <summary>
        /// Supports the Targets property.
        /// </summary>
        private IDictionary<string, string[]> _targets;

        /// <summary>
        /// Supports the TargetTypeOptions property.
        /// </summary>
        private IDictionary<string, string[]> _targetTypeOptions;

        /// <summary>
        /// This field is used to revert to the previous selection when a user cancels a folder browse dialog.
        /// </summary>
        private string _lastSelectedTarget;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public PackageDeployWindow()
        {
            InitializeComponent();
            
            _targets = new Dictionary<string, string[]>();
            _targetTypeOptions = new Dictionary<string, string[]>();
            _lastSelectedTarget = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the package being deployed.
        /// </summary>
        public string PackageName
        {
            get { return textBlockPackage.Text; }
            set { textBlockPackage.Text = value; }
        }

        /// <summary>
        /// The available target types.
        /// </summary>
        public string[] TargetTypes
        {
            get { return comboBoxTargetType.ItemsSource as string[]; }
            set { comboBoxTargetType.ItemsSource = value; }
        }

        /// <summary>
        /// The currently selected target type.
        /// </summary>
        public string SelectedTargetType
        {
            get { return comboBoxTargetType.SelectedItem as string; }
            set { comboBoxTargetType.SelectedItem = value; }
        }

        /// <summary>
        /// The targets available for a given target type.
        /// </summary>
        public IDictionary<string, string[]> Targets
        {
            get { return _targets; }
        }

        /// <summary>
        /// The currently selected target.
        /// </summary>
        public string SelectedTarget
        {
            get { return comboBoxTarget.SelectedItem as string; }
            set { comboBoxTarget.SelectedItem = value; }
        }

        /// <summary>
        /// The options flags available for a given target type.
        /// </summary>
        public IDictionary<string, string[]> TargetTypeOptions
        {
            get { return _targetTypeOptions; }
        }

        /// <summary>
        /// The target type options that have been selected.
        /// </summary>
        public string[] SelectedTargetTypeOptions
        {
            get
            {
                List<string> result = new List<string>();
                foreach (object option in stackPanelOptions.Children)
                {
                    if (option is CheckBox)
                    {
                        CheckBox checkBox = option as CheckBox;
                        if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                            result.Add(checkBox.Content as string);
                    }
                }

                return result.ToArray();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the available targets.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxTargetType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboBoxTarget.ItemsSource = null;
                comboBoxTarget.SelectedItem = null;
                stackPanelOptions.Children.Clear();

                if (Targets.ContainsKey(SelectedTargetType))
                    comboBoxTarget.ItemsSource = Targets[SelectedTargetType];

                if (TargetTypeOptions.ContainsKey(SelectedTargetType))
                    foreach (string option in TargetTypeOptions[SelectedTargetType])
                        stackPanelOptions.Children.Add(new CheckBox()
                        {
                            Content = option
                        });

                textBlockOptions.Visibility = (stackPanelOptions.Children.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                stackPanelOptions.Visibility = (stackPanelOptions.Children.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update enabled state of deploy button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SelectedTarget == "browse...")
                {
                    System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
                    dlg.Description = "Select the folder to deploy the package to.";
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!comboBoxTarget.Items.Contains(dlg.SelectedPath))
                        {
                            List<string> items = new List<string>(comboBoxTarget.ItemsSource as string[]);
                            items.Insert(0, dlg.SelectedPath);
                            comboBoxTarget.ItemsSource = items.ToArray();
                        }
                        
                        comboBoxTarget.SelectedItem = dlg.SelectedPath;
                    }
                    else
                    {
                        comboBoxTarget.SelectedItem = _lastSelectedTarget;
                    }
                }
                else
                {
                    buttonDeploy.IsEnabled = (SelectedTarget != null);
                    _lastSelectedTarget = SelectedTarget;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close the dialog with an ok status.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonDeploy_Click(object sender, RoutedEventArgs e)
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
        /// <param name="sender">The object that raised the event.</param>
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
