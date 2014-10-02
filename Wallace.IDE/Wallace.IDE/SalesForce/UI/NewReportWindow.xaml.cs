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
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for NewReportWindow.xaml
    /// </summary>
    public partial class NewReportWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewReportWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project to use for setting report filters.
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// The report filter entered by the user.
        /// </summary>
        public IReportFilter ReportFilter { get; private set; }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Set filter and close the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Project == null)
                    throw new Exception("Project has not been set for window.");

                if (tabItemBasic.IsSelected)
                {
                    // get user id
                    DataSelectResult user = Project.Client.DataSelect(String.Format("SELECT Id FROM User WHERE Username = '{0}'", Project.Credential.Username));
                    string userId = null;
                    if (user.Data != null && user.Data.Rows.Count > 0)
                        userId = user.Data.Rows[0]["Id"] as string;

                    if (radioButtonBasicUserModifiedToday.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(userId, DateTime.Today);
                    else if (radioButtonBasicUserModifiedWeek.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(userId, DateTime.Today.AddDays(-7));
                    else
                        throw new Exception("No report selected.");
                }
                else
                {
                    throw new Exception("No report selected.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Set filter and close the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                ReportFilter = null;
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
