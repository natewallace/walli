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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for PackageDeployStatusControl.xaml
    /// </summary>
    public partial class PackageDeployStatusControl : UserControl
    {

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public PackageDeployStatusControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The status text displayed.
        /// </summary>
        public string StatusText
        {
            get { return textBlockStatus.Text; }
            set { textBlockStatus.Text = value; }
        }

        /// <summary>
        /// The package text displayed.
        /// </summary>
        public string PackageText
        {
            get { return textBlockPackage.Text; }
            set { textBlockPackage.Text = value; }
        }

        /// <summary>
        /// The target text displayed.
        /// </summary>
        public string TargetText
        {
            get { return textBlockTarget.Text; }
            set { textBlockTarget.Text = value; }
        }

        /// <summary>
        /// The notes text displayed.
        /// </summary>
        public string NotesText
        {
            get { return textBlockNotes.Text; }
            set { textBlockNotes.Text = value; }
        }

        /// <summary>
        /// The the total number for the components progress.
        /// </summary>
        public int ComponentProgressMaximum
        {
            get { return (int)progressBarComponents.Maximum; }
            set { progressBarComponents.Maximum = value; }
        }

        /// <summary>
        /// The component progress value.
        /// </summary>
        public int ComponentProgressValue
        {
            get { return (int)progressBarComponents.Value; }
            set { progressBarComponents.Value = value; }
        }

        /// <summary>
        /// The brush for the component progress foreground.
        /// </summary>
        public Brush ComponentProgressForeground
        {
            get { return progressBarComponents.Foreground ; }
            set { progressBarComponents.Foreground = value; }
        }

        /// <summary>
        /// The brush for the component progress background.
        /// </summary>
        public Brush ComponentProgressBackground
        {
            get { return progressBarComponents.Background; }
            set { progressBarComponents.Background = value; }
        }

        /// <summary>
        /// A component progress message.
        /// </summary>
        public string ComponentProgressMessage
        {
            get { return textBlockProgressComponents.Text; }
            set { textBlockProgressComponents.Text = value; }
        }

        /// <summary>
        /// Sets the visibility of the tests section.
        /// </summary>
        public bool IsTestSectionVisible
        {
            get
            {
                return (textBlockProgressTests.Visibility == Visibility.Visible);
            }
            set
            {
                textBlockHeaderTests.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                progressBarTests.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                textBlockProgressTests.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The the total number for the test progress.
        /// </summary>
        public int TestProgressMaximum
        {
            get { return (int)progressBarTests.Maximum; }
            set { progressBarTests.Maximum = value; }
        }

        /// <summary>
        /// The test progress value.
        /// </summary>
        public int TestProgressValue
        {
            get { return (int)progressBarTests.Value; }
            set { progressBarTests.Value = value; }
        }

        /// <summary>
        /// The brush for the test progress foreground.
        /// </summary>
        public Brush TestProgressForeground
        {
            get { return progressBarTests.Foreground; }
            set { progressBarTests.Foreground = value; }
        }

        /// <summary>
        /// The brush for the test progress background.
        /// </summary>
        public Brush TestProgressBackground
        {
            get { return progressBarTests.Background; }
            set { progressBarTests.Background = value; }
        }

        /// <summary>
        /// A test progress message.
        /// </summary>
        public string TestProgressMessage
        {
            get { return textBlockProgressTests.Text; }
            set { textBlockProgressTests.Text = value; }
        }

        /// <summary>
        /// The results text.
        /// </summary>
        public string ResultText
        {
            get { return textBoxResults.Text; }
            set { textBoxResults.Text = value; }
        }

        #endregion
    }
}
