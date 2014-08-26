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
using System.Data;
using System.Text;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Document that is used to view and edit data.
    /// </summary>
    public class DataEditorDocument : DocumentBase
    {
        #region Fields

        /// <summary>
        /// Supports the DataResult field.
        /// </summary>
        private DataSelectResult _dataResult;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project"></param>
        public DataEditorDocument(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;
            View = new DataEditControl();
            View.ExecuteClick += DataEdit_ExecuteClick;
            View.CommitClick += DataEdit_CommitClick;
            View.NextClick += DataEdit_NextClick;
            View.SOQLTextChanged += DataEdit_SOQLTextChanged;
            View.IsExecuteEnabled = false;
            DataResult = null;

            UpdateDataDisplay();
            UpdateDisplay();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project to edit data on.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The control that is used for display.
        /// </summary>
        private DataEditControl View { get; set; }

        /// <summary>
        /// The current data being displayed.
        /// </summary>
        private DataSelectResult DataResult
        {
            get
            {
                return _dataResult;
            }
            set
            {
                _dataResult = value;

                if (_dataResult != null && _dataResult.Data != null)
                {
                    _dataResult.Data.TableNewRow += Data_Changed;
                    _dataResult.Data.RowChanged += Data_Changed;
                    _dataResult.Data.RowDeleted += Data_Changed;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader("SOQL", "Table.png");
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Check for data loss before closing.
        /// </summary>
        /// <returns>true if this document can be closed.  false if it can't.</returns>
        public override bool Closing()
        {
            return DataLossCheck();
        }

        /// <summary>
        /// Give focus to the text input when activated.
        /// </summary>
        public override void Activated()
        {
            View.FocusText();
        }

        /// <summary>
        /// Check for data loss if document is closed.
        /// </summary>
        /// <returns>true if there will be no loss or the user is ok with it.</returns>
        private bool DataLossCheck()
        {
            if (IsDataModified)
            {
                return (App.MessageUser(
                    "You have uncommitted changes which will be lost.  Do you want to proceed?",
                    "Data Loss",
                    System.Windows.MessageBoxImage.Warning,
                    new string[] { "Yes", "No" }) == "Yes");
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates the data displayed.
        /// </summary>
        private void UpdateDataDisplay()
        {
            if (DataResult != null && DataResult.Data != null)
            {
                View.Data = DataResult.Data.DefaultView;
                if (DataResult.Data.Rows.Count == 0)
                    View.DataResultText = String.Format("{0} total records.", DataResult.Size);
                else
                    View.DataResultText = String.Format("Displaying records {0} through {1} of {2} total records.",
                        DataResult.Index + 1,
                        DataResult.Data.Rows.Count + DataResult.Index,
                        DataResult.Size);

                if (String.IsNullOrWhiteSpace(DataResult.Data.TableName))
                {
                    Presenter.Header = VisualHelper.CreateIconHeader("SOQL", "Table.png");
                    Text = "SOQL";
                }
                else
                {
                    Presenter.Header = VisualHelper.CreateIconHeader(DataResult.Data.TableName, "Table.png");
                    Text = DataResult.Data.TableName;
                }
            }
            else
            {
                View.Data = null;
                View.DataResultText = String.Empty;
            }
        }

        /// <summary>
        /// Indicates if any data has been changed by the user.
        /// </summary>
        private bool IsDataModified
        {
            get
            {
                if (DataResult == null)
                    return false;

                foreach (DataRow row in DataResult.Data.Rows)
                    if (row.RowState != DataRowState.Unchanged)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Update the display based on the current state.
        /// </summary>
        private void UpdateDisplay()
        {
            if (DataResult == null)
            {
                View.IsNextVisible = false;
                View.IsCommitEnabled = false;
            }
            else
            {
                View.IsNextVisible = DataResult.IsMore;
                View.IsCommitEnabled = IsDataModified;
            }
            
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Execute the query and display the results.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DataEdit_ExecuteClick(object sender, EventArgs e)
        {
            try
            {
                if (DataLossCheck())
                {
                    using (App.Wait("Executing query..."))
                    {
                        DataResult = Project.Client.DataSelect(View.SOQLText, true);
                        UpdateDataDisplay();
                        UpdateDisplay();
                    }
                }
            }
            catch (Exception err)
            {
                App.MessageUser(err.Message, "Error", System.Windows.MessageBoxImage.Error, new string[] { "OK" });
            }
        }

        /// <summary>
        /// Commit changes to the database.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DataEdit_CommitClick(object sender, EventArgs e)
        {
            if (DataResult != null && DataResult.Data != null)
            {
                Dictionary<DataRow, DataRow> rowMap = new Dictionary<DataRow, DataRow>();

                DataTable addTable = DataResult.Data.Clone();
                DataTable updateTable = DataResult.Data.Clone();
                DataTable deleteTable = DataResult.Data.Clone();

                foreach (DataRow row in DataResult.Data.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            addTable.ImportRow(row);
                            rowMap.Add(addTable.Rows[addTable.Rows.Count - 1], row);
                            break;

                        case DataRowState.Modified:
                            updateTable.ImportRow(row);
                            break;

                        case DataRowState.Deleted:
                            deleteTable.ImportRow(row);
                            break;

                        default:
                            break;
                    }
                }

                StringBuilder sb = new StringBuilder();
                if (addTable.Rows.Count > 0)
                    sb.AppendLine(String.Format("{0} record(s) will be added.", addTable.Rows.Count));
                if (updateTable.Rows.Count > 0)
                    sb.AppendLine(String.Format("{0} record(s) will be updated.", updateTable.Rows.Count));
                if (deleteTable.Rows.Count > 0)
                    sb.AppendLine(String.Format("{0} record(s) will be deleted.", deleteTable.Rows.Count));

                if (sb.Length > 0)
                {
                    sb.Insert(0, String.Format("The following changes will be commited:{0}{0}", Environment.NewLine));
                    sb.AppendLine();
                    sb.AppendLine("Do you want to proceed?");

                    if (App.MessageUser(sb.ToString(), "Confirm Commit", System.Windows.MessageBoxImage.Warning, new string[] { "Yes", "No" }) == "Yes")
                    {
                        try
                        {
                            using (App.Wait("Saving..."))
                            {
                                if (addTable.Rows.Count > 0)
                                    Project.Client.DataInsert(addTable);
                                if (updateTable.Rows.Count > 0)
                                    Project.Client.DataUpdate(updateTable);
                                if (deleteTable.Rows.Count > 0)
                                    Project.Client.DataDelete(deleteTable);

                                // set the id fields for adds
                                foreach (DataRow row in addTable.Rows)
                                {
                                    DataRow originalRow = rowMap[row];
                                    originalRow["Id"] = row["Id"];
                                }

                                DataResult.Data.AcceptChanges();
                                UpdateDisplay();
                            }
                        }
                        catch (Exception err)
                        {
                            App.HandleException(err);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the next batch of results and displays them.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DataEdit_NextClick(object sender, EventArgs e)
        {
            if (DataResult != null && DataResult.IsMore)
            {
                try
                {
                    if (DataLossCheck())
                    {
                        using (App.Wait("Fetching next page..."))
                        {
                            DataResult = Project.Client.DataSelect(DataResult);
                            UpdateDataDisplay();
                            UpdateDisplay();
                        }
                    }
                }
                catch (Exception err)
                {
                    App.HandleException(err);
                }
            }
        }

        /// <summary>
        /// Update the execute button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DataEdit_SOQLTextChanged(object sender, EventArgs e)
        {
            View.IsExecuteEnabled = !String.IsNullOrWhiteSpace(View.SOQLText);
        }

        /// <summary>
        /// Update the display when data is changed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Data_Changed(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        #endregion
    }
}
