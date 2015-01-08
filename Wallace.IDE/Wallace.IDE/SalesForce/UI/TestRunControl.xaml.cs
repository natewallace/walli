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

using SalesForceData;
using System;
using System.Collections;
using System.Windows.Controls;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for TestRunControl.xaml
    /// </summary>
    public partial class TestRunControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestRunControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The title text for the view.
        /// </summary>
        public string TitleText
        {
            get { return textBlockTitle.Text; }
            set { textBlockTitle.Text = value; }
        }

        /// <summary>
        /// The test run items.
        /// </summary>
        public IEnumerable TestRunItems
        {
            get { return listViewClasses.ItemsSource; }
            set { listViewClasses.ItemsSource = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the view.
        /// </summary>
        public void UpdateView()
        {
            TestRunItem item = listViewClasses.SelectedItem as TestRunItem;
            if (item != null)
            {
                if (item.Results.Length > 0 && listViewMethods.ItemsSource == null)
                    listViewMethods.ItemsSource = item.Results;
            }

            TestRunItemResult result = listViewMethods.SelectedItem as TestRunItemResult;
            if (result != null)
            {
                textBoxResults.Text = String.Format("{1}{0}{0}{2}", Environment.NewLine, result.Message, result.StackTrace);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Display the results for the given test run item.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TestRunItem item = listViewClasses.SelectedItem as TestRunItem;
                if (item == null || item.Results.Length == 0)
                    listViewMethods.ItemsSource = null;
                else
                    listViewMethods.ItemsSource = item.Results;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Display the results for the given test run item result.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listViewMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TestRunItemResult result = listViewMethods.SelectedItem as TestRunItemResult;
                if (result == null)
                    textBoxResults.Text = null;
                else
                    textBoxResults.Text = String.Format("{1}{0}{0}{2}", Environment.NewLine, result.Message, result.StackTrace);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
