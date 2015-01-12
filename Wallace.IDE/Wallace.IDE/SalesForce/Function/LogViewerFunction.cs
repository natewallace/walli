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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// The log viewer function.
    /// </summary>
    public class LogViewerFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "LogViewer.png");
                presenter.ToolTip = "Log viewer...";
            }
            else
            {
                presenter.Header = "Log viewer...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "LogViewer.png");
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Open the test manager.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                LogParameters[] lps = null;
                using (App.Wait("Loading log parameters"))
                {
                    lps = App.Instance.SalesForceApp.CurrentProject.Client.GetLogParameters();
                }

                LogParametersManagerWindow dlg = new LogParametersManagerWindow();
                dlg.LogParameters = lps;
                dlg.DeleteClick += DeleteLogParameters;
                dlg.NewClick += NewLogParameters;
                dlg.ViewLogsClick += ViewLogs;
                dlg.EditClick += EditLogParameters;

                App.ShowDialog(dlg);

                dlg.DeleteClick -= DeleteLogParameters;
                dlg.NewClick -= NewLogParameters;
                dlg.ViewLogsClick -= ViewLogs;
                dlg.EditClick -= EditLogParameters;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Delete log paramters selected by the user.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DeleteLogParameters(object sender, EventArgs e)
        {
            try
            {
                LogParametersManagerWindow dlg = sender as LogParametersManagerWindow;
                if (App.Instance.SalesForceApp.CurrentProject != null &&
                    dlg != null &&
                    dlg.SelectedLogParameters != null)
                {
                    if (App.MessageUser("Are you sure you want to delete the selected Log Parameters?",
                                        "Confirm Delete",
                                        System.Windows.MessageBoxImage.Question,
                                        new string[] { "Yes", "No" }) == "Yes")
                    {
                        using (App.Wait("Deleting Log Paramters"))
                        {
                            App.Instance.SalesForceApp.CurrentProject.Client.DeleteLogParameters(dlg.SelectedLogParameters);
                            List<LogParameters> list = new List<LogParameters>(dlg.LogParameters);
                            list.Remove(dlg.SelectedLogParameters);
                            dlg.LogParameters = list;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Create new log parameters.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NewLogParameters(object sender, EventArgs e)
        {
            try
            {
                LogParametersManagerWindow dlg = sender as LogParametersManagerWindow;

                if (App.Instance.SalesForceApp.CurrentProject != null && dlg != null)
                {
                    NewLogParametersWindow newDlg = new NewLogParametersWindow();
                    newDlg.Title = "New Log Parameters";
                    newDlg.SaveButtonText = "Create";
                    newDlg.TracedEntity = String.Format("{0} (user)", App.Instance.SalesForceApp.CurrentProject.Client.User.Name);
                    newDlg.Scope = String.Empty;
                    newDlg.ExpirationDate = DateTime.Now.AddDays(1);
                    newDlg.LogLevelCode = LogLevel.Info;
                    newDlg.LogLevelProfiling = LogLevel.Info;
                    newDlg.LogLevelDatabase = LogLevel.Info;

                    if (App.ShowDialog(newDlg))
                    {
                        using (App.Wait("Creating Log Paramters"))
                        {
                            LogParameters log = App.Instance.SalesForceApp.CurrentProject.Client.CreateLogParameters(
                                App.Instance.SalesForceApp.CurrentProject.Client.User.Id,
                                newDlg.TracedEntity,
                                String.Empty,
                                String.Empty,
                                (newDlg.ExpirationDate.HasValue) ? newDlg.ExpirationDate.Value : DateTime.Now.AddHours(4),
                                newDlg.LogLevelCode,
                                newDlg.LogLevelVisualForce,
                                newDlg.LogLevelProfiling,
                                newDlg.LogLevelCallout,
                                newDlg.LogLevelDatabase,
                                newDlg.LogLevelSystem,
                                newDlg.LogLevelValidation,
                                newDlg.LogLevelWorkflow);

                            List<LogParameters> list = new List<LogParameters>(dlg.LogParameters);
                            list.Add(log);
                            dlg.LogParameters = list;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Edit log parameters.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void EditLogParameters(object sender, EventArgs e)
        {
            try
            {
                LogParametersManagerWindow dlg = sender as LogParametersManagerWindow;
                if (App.Instance.SalesForceApp.CurrentProject != null &&
                    dlg != null &&
                    dlg.SelectedLogParameters != null)
                {
                    NewLogParametersWindow editDlg = new NewLogParametersWindow();
                    editDlg.Title = "Edit Log Parameters";
                    editDlg.SaveButtonText = "Save";
                    editDlg.TracedEntity = dlg.SelectedLogParameters.TracedEntityName;
                    editDlg.Scope = dlg.SelectedLogParameters.ScopeName;
                    editDlg.ExpirationDate = dlg.SelectedLogParameters.ExpirationDate;
                    editDlg.LogLevelCallout = dlg.SelectedLogParameters.CalloutLevel;
                    editDlg.LogLevelCode = dlg.SelectedLogParameters.CodeLevel;
                    editDlg.LogLevelDatabase = dlg.SelectedLogParameters.DatabaseLevel;
                    editDlg.LogLevelProfiling = dlg.SelectedLogParameters.ProfilingLevel;
                    editDlg.LogLevelSystem = dlg.SelectedLogParameters.SystemLevel;
                    editDlg.LogLevelValidation = dlg.SelectedLogParameters.ValidationLevel;
                    editDlg.LogLevelVisualForce = dlg.SelectedLogParameters.VisualForceLevel;
                    editDlg.LogLevelWorkflow = dlg.SelectedLogParameters.WorkflowLevel;

                    if (App.ShowDialog(editDlg))
                    {
                        using (App.Wait("Updating Log Paramters"))
                        {
                            dlg.SelectedLogParameters.CalloutLevel = editDlg.LogLevelCallout;
                            dlg.SelectedLogParameters.CodeLevel = editDlg.LogLevelCode;
                            dlg.SelectedLogParameters.DatabaseLevel = editDlg.LogLevelDatabase;
                            dlg.SelectedLogParameters.ProfilingLevel = editDlg.LogLevelProfiling;
                            dlg.SelectedLogParameters.SystemLevel = editDlg.LogLevelSystem;
                            dlg.SelectedLogParameters.ValidationLevel = editDlg.LogLevelValidation;
                            dlg.SelectedLogParameters.VisualForceLevel = editDlg.LogLevelVisualForce;
                            dlg.SelectedLogParameters.WorkflowLevel = editDlg.LogLevelWorkflow;

                            App.Instance.SalesForceApp.CurrentProject.Client.UpdateLogParameters(dlg.SelectedLogParameters);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// View the logs for the selected log parameters.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ViewLogs(object sender, EventArgs e)
        {
            try
            {
                LogParametersManagerWindow dlg = sender as LogParametersManagerWindow;
                if (App.Instance.SalesForceApp.CurrentProject != null &&
                    dlg != null &&
                    dlg.SelectedLogParameters != null)
                {
                    IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(dlg.SelectedLogParameters);
                    if (documents.Length > 0)
                    {
                        App.Instance.Content.ActiveDocument = documents[0];
                    }
                    else
                    {
                        using (App.Wait("Opening log viewer"))
                        {
                            LogViewerDocument document = new LogViewerDocument(
                                App.Instance.SalesForceApp.CurrentProject,
                                dlg.SelectedLogParameters);

                            document.Refresh();
                            App.Instance.Content.OpenDocument(document);
                        }
                    }
                }

                dlg.Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
