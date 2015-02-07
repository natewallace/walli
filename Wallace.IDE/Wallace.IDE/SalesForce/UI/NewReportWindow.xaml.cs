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
using SalesForceData;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for NewReportWindow.xaml
    /// </summary>
    public partial class NewReportWindow : Window
    {
        #region Fields

        /// <summary>
        /// Supports the Project property.
        /// </summary>
        private Project _project;

        #endregion

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
        public Project Project
        {
            get { return _project; }
            set
            {
                _project = value;
                if (_project != null)
                {
                    buttonUserModifiedToday.Content = _project.Client.User;
                    buttonUserModifiedWeek.Content = _project.Client.User;
                    buttonUserModifiedAll.Content = _project.Client.User;
                }
            }
        }

        /// <summary>
        /// The report filter entered by the user.
        /// </summary>
        public IReportFilter ReportFilter { get; private set; }

        /// <summary>
        /// Exclusions the user can choose from.
        /// </summary>
        public string[] Exclusions
        {
            get
            {
                List<string> result = new List<string>();
                foreach (object item in buttonExclude.ContextMenu.Items)
                {
                    if (item is MenuItem)
                    {
                        MenuItem mi = item as MenuItem;
                        string header = mi.Header as string;
                        if (header != "Select All" && header != "Select None" && header != "Close")
                            result.Add(header);
                    }
                }

                return result.ToArray();
            }
            set
            {
                buttonExclude.ContextMenu.Items.Clear();

                MenuItem selectAll = new MenuItem();
                selectAll.Header = "Select All";
                selectAll.StaysOpenOnClick = true;
                buttonExclude.ContextMenu.Items.Add(selectAll);

                MenuItem selectNone = new MenuItem();
                selectNone.Header = "Select None";
                selectNone.StaysOpenOnClick = true;
                buttonExclude.ContextMenu.Items.Add(selectNone);

                MenuItem close = new MenuItem();
                close.Header = "Close";
                close.StaysOpenOnClick = true;
                buttonExclude.ContextMenu.Items.Add(close);

                buttonExclude.ContextMenu.Items.Add(new Separator());

                if (value != null)
                {
                    foreach (string exlude in value)
                    {
                        MenuItem item = new MenuItem();
                        item.Header = exlude;
                        item.IsCheckable = true;
                        item.StaysOpenOnClick = true;

                        buttonExclude.ContextMenu.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// The selected exclusions.
        /// </summary>
        public string[] SelectedExclusions
        {
            get
            {
                List<string> result = new List<string>();
                foreach (object item in buttonExclude.ContextMenu.Items)
                {
                    if (item is MenuItem)
                    {
                        MenuItem mi = item as MenuItem;
                        string header = mi.Header as string;
                        if (mi.IsChecked)
                            result.Add(header);
                    }
                }

                return result.ToArray();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the state of the UI.
        /// </summary>
        private void UpdateState()
        {
            int count = SelectedExclusions.Length;
            if (count == 0)
                textBlockExclude.Text = "No Exclusions";
            else if (count == 1)
                textBlockExclude.Text = "1 Exclusion";
            else
                textBlockExclude.Text = String.Format("{0} Exclusions", count);
        }

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
                    if (radioButtonBasicUserModifiedToday.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(buttonUserModifiedToday.Content as User, DateTime.Today);
                    else if (radioButtonBasicUserModifiedWeek.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(buttonUserModifiedWeek.Content as User, DateTime.Today.AddDays(-7));
                    else if (radioButtonBasicUserModifiedAll.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(buttonUserModifiedAll.Content as User, DateTime.MinValue);
                    else if (radioButtonBasicUserAnyModifiedToday.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(null, DateTime.Today);
                    else if (radioButtonBasicUserAnyModifiedWeek.IsChecked.Value)
                        ReportFilter = new ReportFilterUserDate(null, DateTime.Today.AddDays(-7));
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

        /// <summary>
        /// Show or hide the context menu.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonExclude_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                buttonExclude.ContextMenu.IsEnabled = true;
                buttonExclude.ContextMenu.PlacementTarget = buttonExclude;
                buttonExclude.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                buttonExclude.ContextMenu.IsOpen = true;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update UI state.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonExclude_CheckedChange(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateState();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Close, Check or Uncheck all.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = e.Source as MenuItem;
                if (item != null)
                {
                    string header = item.Header as string;
                    switch (header)
                    {
                        case "Select All":
                            foreach (object obj in buttonExclude.ContextMenu.Items)
                            {
                                MenuItem mi = obj as MenuItem;
                                if (mi != null && mi.IsCheckable)
                                    mi.IsChecked = true;
                            }
                            break;

                        case "Select None":
                            foreach (object obj in buttonExclude.ContextMenu.Items)
                            {
                                MenuItem mi = obj as MenuItem;
                                if (mi != null && mi.IsCheckable)
                                    mi.IsChecked = false;
                            }
                            break;

                        case "Close":
                            buttonExclude.ContextMenu.IsOpen = false;
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

        /// <summary>
        /// Do user search.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void dialog_UserSearch(object sender, UserSearchEventArgs e)
        {
            e.Results = Project.Client.SearchUsers(e.Query);
        }

        /// <summary>
        /// Let user select user.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonUserModified_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // user search
                UserSelectWindow dlg = new UserSelectWindow();
                dlg.UserSearch += dialog_UserSearch;
                if (App.ShowDialog(dlg))
                {
                    (sender as Button).Content = dlg.SelectedUser;

                    if (sender == buttonUserModifiedToday)
                        radioButtonBasicUserModifiedToday.IsChecked = true;
                    else if (sender == buttonUserModifiedWeek)
                        radioButtonBasicUserModifiedWeek.IsChecked = true;
                    else if (sender == buttonUserModifiedAll)
                        radioButtonBasicUserModifiedAll.IsChecked = true;
                }
                dlg.UserSearch -= dialog_UserSearch;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
