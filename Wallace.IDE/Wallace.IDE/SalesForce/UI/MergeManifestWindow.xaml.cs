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
    /// Interaction logic for MergeManifestWindow.xaml
    /// </summary>
    public partial class MergeManifestWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeManifestWindow()
        {
            InitializeComponent();

            UpdateView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The manifest sources to choose between.
        /// </summary>
        public IEnumerable<Manifest> ManifestSources
        {
            get { return comboBoxSource.ItemsSource as IEnumerable<Manifest>; }
            set { comboBoxSource.ItemsSource = value; }
        }

        /// <summary>
        /// The manifest targets to choose between.
        /// </summary>
        public IEnumerable<Manifest> ManifestTargets
        {
            get { return comboBoxTarget.ItemsSource as IEnumerable<Manifest>; }
            set { comboBoxTarget.ItemsSource = value; }
        }

        /// <summary>
        /// The selected manifest source.
        /// </summary>
        public Manifest ManifestSource
        {
            get { return comboBoxSource.SelectedItem as Manifest; }
            set { comboBoxSource.SelectedItem = value; }
        }

        /// <summary>
        /// The selected manifest target.
        /// </summary>
        public Manifest ManifestTarget
        {
            get { return comboBoxTarget.SelectedItem as Manifest; }
            set { comboBoxTarget.SelectedItem = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the view based on the current state.
        /// </summary>
        private void UpdateView()
        {
            buttonMerge.IsEnabled = (ManifestSource != null && 
                                     ManifestTarget != null &&
                                     ManifestSource.Name != ManifestTarget.Name);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Set to cancel and close the dialog.
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

        /// <summary>
        /// Set to ok and close the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonMerge_Click(object sender, RoutedEventArgs e)
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
        /// Update the view.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateView();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
