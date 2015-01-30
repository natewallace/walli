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
using System.Windows;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Function;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;
using Wallace.IDE.SalesForce.Settings;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// The main salesforce application.
    /// </summary>
    public class SalesForceApplication
    {
        #region Properties

        /// <summary>
        /// The currently open project.
        /// </summary>
        public Project CurrentProject { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Close any open projects.
        /// </summary>
        /// <returns>true if the application can close, false if it can't.</returns>
        public bool ApplicationClosing()
        {
            return CloseProject();
        }

        /// <summary>
        /// Load all settings.
        /// </summary>
        public void LoadSettings()
        {
            App.Instance.Settings.RegisterSettings(new FontAndColorApexSettings());
            App.Instance.Settings.RegisterSettings(new FontAndColorSOQLSettings());
            App.Instance.Settings.RegisterSettings(new FontAndColorVisualForceSettings());
            App.Instance.Settings.RegisterSettings(new DefaultHeaderApexSettings());
            App.Instance.Settings.RegisterSettings(new DefaultHeaderVisualForceSettings());
        }

        /// <summary>
        /// Load functions for this application.
        /// </summary>
        public void LoadFunctions()
        {
            //
            // SYSTEM
            //

            NewProjectFunction newLocalProject = new NewProjectFunction();
            App.Instance.Menu.AddFunction(newLocalProject, "SYSTEM", 0);
            App.Instance.ToolBar.AddFunction(newLocalProject);

            OpenProjectFunction openLocalProject = new OpenProjectFunction();
            App.Instance.Menu.AddFunction(openLocalProject, "SYSTEM", 1);
            App.Instance.ToolBar.AddFunction(openLocalProject);

            CloseProjectFunction closeLocalProject = new CloseProjectFunction();
            App.Instance.Menu.AddFunction(closeLocalProject, "SYSTEM", 2);

            DeleteProjectFunction deleteProject = new DeleteProjectFunction();
            App.Instance.Menu.AddFunction(deleteProject, "SYSTEM", 3);

            //
            // PROJECT | New
            //

            App.Instance.Menu.AddFunction(new FunctionGrouping("NEWSALESFORCE", "New", true), "PROJECT");

            NewClassFunction newClassFunction = new NewClassFunction();
            App.Instance.Menu.AddFunction(newClassFunction, "NEWSALESFORCE");
            App.Instance.RegisterFunction(newClassFunction);

            NewTriggerFunction newTriggerFunction = new NewTriggerFunction();
            App.Instance.Menu.AddFunction(newTriggerFunction, "NEWSALESFORCE");
            App.Instance.RegisterFunction(newTriggerFunction);

            NewPageFunction newPageFunction = new NewPageFunction();
            App.Instance.Menu.AddFunction(newPageFunction, "NEWSALESFORCE");
            App.Instance.RegisterFunction(newPageFunction);

            NewComponentFunction newComponentFunction = new NewComponentFunction();
            App.Instance.Menu.AddFunction(newComponentFunction, "NEWSALESFORCE");
            App.Instance.RegisterFunction(newComponentFunction);

            NewManifestFunction newManifestFunction = new NewManifestFunction();
            App.Instance.Menu.AddFunction(newManifestFunction, "NEWSALESFORCE");
            App.Instance.RegisterFunction(newManifestFunction);

            //
            // PROJECT | Team
            //

            App.Instance.Menu.AddFunction(new FunctionGrouping("TEAMSALESFORCE", "Team", true), "PROJECT");

            ConfigureSourceControlFunction sourceControlFunction = new ConfigureSourceControlFunction();
            App.Instance.Menu.AddFunction(sourceControlFunction, "TEAMSALESFORCE");

            CheckoutFolderFunction checkoutFolderFunction = new CheckoutFolderFunction();
            App.Instance.Menu.AddFunction(checkoutFolderFunction, "TEAMSALESFORCE");

            CheckoutHistoryFunction checkoutHistoryFunction = new CheckoutHistoryFunction();
            App.Instance.Menu.AddFunction(new FunctionSeparator(checkoutHistoryFunction), "TEAMSALESFORCE");
            App.Instance.Menu.AddFunction(checkoutHistoryFunction, "TEAMSALESFORCE");
            App.Instance.RegisterFunction(checkoutHistoryFunction);

            CheckoutFileFunction checkoutFileFunction = new CheckoutFileFunction();
            App.Instance.Menu.AddFunction(checkoutFileFunction, "TEAMSALESFORCE");
            App.Instance.RegisterFunction(checkoutFileFunction);

            CheckinFileFunction checkinFileFunction = new CheckinFileFunction();
            App.Instance.Menu.AddFunction(checkinFileFunction, "TEAMSALESFORCE");
            App.Instance.RegisterFunction(checkinFileFunction);

            CheckoutFileUndoFunction checkoutFileUndoFunction = new CheckoutFileUndoFunction();
            App.Instance.Menu.AddFunction(checkoutFileUndoFunction, "TEAMSALESFORCE");
            App.Instance.RegisterFunction(checkoutFileUndoFunction);

            CheckoutFileHistoryFunction checkoutFileHistoryFunction = new CheckoutFileHistoryFunction();
            App.Instance.Menu.AddFunction(checkoutFileHistoryFunction, "TEAMSALESFORCE");
            App.Instance.RegisterFunction(checkoutFileHistoryFunction);

            //
            // PROJECT | Open Web Browser
            //

            App.Instance.Menu.AddFunction(new FunctionGrouping("Open Web Browser", "Open Web Browser", true), "PROJECT");
            if (ClientBrowser.GetInstalledBrowsers().Length == 0)
            {
                OpenWebBrowserFunction webBrowser = new OpenWebBrowserFunction(ClientBrowser.GetDefaultBrowser());
                App.Instance.Menu.AddFunction(webBrowser, "Open Web Browser");
            }
            else
            {
                foreach (ClientBrowser cb in ClientBrowser.GetInstalledBrowsers())
                {
                    OpenWebBrowserFunction webBrowser = new OpenWebBrowserFunction(cb);
                    App.Instance.Menu.AddFunction(webBrowser, "Open Web Browser");
                }
            }

            OpenRecentWebBrowserFunction recentWebBrowserFunction = new OpenRecentWebBrowserFunction();
            App.Instance.ToolBar.AddFunction(recentWebBrowserFunction);

            DataEditFunction dataEdit = new DataEditFunction();
            App.Instance.ToolBar.AddFunction(dataEdit);
            App.Instance.Menu.AddFunction(dataEdit, "PROJECT");

            NewReportFunction newReport = new NewReportFunction();
            App.Instance.ToolBar.AddFunction(newReport);
            App.Instance.Menu.AddFunction(newReport, "PROJECT");

            MergeManifestFunction mergeManifestFunction = new MergeManifestFunction();
            App.Instance.Menu.AddFunction(mergeManifestFunction, "PROJECT");
            App.Instance.RegisterFunction(mergeManifestFunction);

            TestManagerFunction testManager = new TestManagerFunction();
            App.Instance.Menu.AddFunction(testManager, "PROJECT");

            LogViewerFunction logViewerFunction = new LogViewerFunction();
            App.Instance.Menu.AddFunction(logViewerFunction, "PROJECT");
            App.Instance.ToolBar.AddFunction(logViewerFunction);

            ReloadSymbolsFunction reloadSymbols = new ReloadSymbolsFunction();
            App.Instance.Menu.AddFunction(reloadSymbols, "PROJECT");

            PropertiesFunction propertiesSourceFileFunction = new PropertiesFunction();
            App.Instance.Menu.AddFunction(new FunctionSeparator(propertiesSourceFileFunction), "PROJECT");

            DeleteSourceFileFunction deleteSourceFileFunction = new DeleteSourceFileFunction();
            App.Instance.Menu.AddFunction(deleteSourceFileFunction, "PROJECT");
            App.Instance.RegisterFunction(deleteSourceFileFunction);

            DeleteManifestFunction deleteManifestFunction = new DeleteManifestFunction();
            App.Instance.Menu.AddFunction(deleteManifestFunction, "PROJECT");
            App.Instance.RegisterFunction(deleteManifestFunction);

            DeletePackageFunction deletePackageFunction = new DeletePackageFunction();
            App.Instance.Menu.AddFunction(deletePackageFunction, "PROJECT");
            App.Instance.RegisterFunction(deletePackageFunction);

            RefreshFolderFunction refreshFolderFunction = new RefreshFolderFunction();
            App.Instance.RegisterFunction(refreshFolderFunction);

            App.Instance.Menu.AddFunction(propertiesSourceFileFunction, "PROJECT");
            App.Instance.RegisterFunction(propertiesSourceFileFunction);            

            //ViewCheckpointsFunction viewCheckpointsFunction = new ViewCheckpointsFunction();
            //App.Instance.Menu.AddFunction(viewCheckpointsFunction, "PROJECT");
            //App.Instance.ToolBar.AddFunction(viewCheckpointsFunction);

            //
            // DOCUMENT
            //

            ExecuteDataQueryFunction executeQueryFunction = new ExecuteDataQueryFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(executeQueryFunction));
            App.Instance.ToolBar.AddFunction(executeQueryFunction);
            App.Instance.Menu.AddFunction(executeQueryFunction, "DOCUMENT");

            CommitDataChangesFunction commitDataFunction = new CommitDataChangesFunction();
            App.Instance.ToolBar.AddFunction(commitDataFunction);
            App.Instance.Menu.AddFunction(commitDataFunction, "DOCUMENT");

            ExportDataResultFunction exportData = new ExportDataResultFunction();
            App.Instance.ToolBar.AddFunction(exportData);
            App.Instance.Menu.AddFunction(exportData, "DOCUMENT");

            NewManifestFromReportFunction manifestReport = new NewManifestFromReportFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(manifestReport));
            App.Instance.ToolBar.AddFunction(manifestReport);
            App.Instance.Menu.AddFunction(manifestReport, "DOCUMENT");

            MergeManifestFromReportFunction manifestMergeReport = new MergeManifestFromReportFunction();
            App.Instance.ToolBar.AddFunction(manifestMergeReport);
            App.Instance.Menu.AddFunction(manifestMergeReport, "DOCUMENT");

            SelectReportItemsNoneFunction selectNoneReportFunction = new SelectReportItemsNoneFunction();
            App.Instance.ToolBar.AddFunction(selectNoneReportFunction);
            App.Instance.Menu.AddFunction(selectNoneReportFunction, "DOCUMENT");

            SelectReportItemsAllFunction selectAllReportFunction = new SelectReportItemsAllFunction();
            App.Instance.ToolBar.AddFunction(selectAllReportFunction);
            App.Instance.Menu.AddFunction(selectAllReportFunction, "DOCUMENT");

            SaveSourceFileFunction saveSourceFileFunction = new SaveSourceFileFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(saveSourceFileFunction));
            App.Instance.ToolBar.AddFunction(saveSourceFileFunction);
            App.Instance.Menu.AddFunction(saveSourceFileFunction, "DOCUMENT");

            SaveSourceFileDataFunction saveSourceFileDataFunction = new SaveSourceFileDataFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(saveSourceFileDataFunction));
            App.Instance.ToolBar.AddFunction(saveSourceFileDataFunction);
            App.Instance.Menu.AddFunction(saveSourceFileDataFunction, "DOCUMENT");

            SaveManifestFunction saveManifestFunction = new SaveManifestFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(saveManifestFunction));
            App.Instance.ToolBar.AddFunction(saveManifestFunction);
            App.Instance.Menu.AddFunction(saveManifestFunction, "DOCUMENT");
            App.Instance.RegisterFunction(saveManifestFunction);

            AddFileToManifestFunction addFileManifestFunction = new AddFileToManifestFunction();
            App.Instance.ToolBar.AddFunction(addFileManifestFunction);
            App.Instance.Menu.AddFunction(addFileManifestFunction, "DOCUMENT");
            App.Instance.RegisterFunction(addFileManifestFunction);

            NewPackageFunction newPackageFunction = new NewPackageFunction();
            App.Instance.ToolBar.AddFunction(newPackageFunction);
            App.Instance.Menu.AddFunction(newPackageFunction, "DOCUMENT");

            DeployPackageFunction deployPackageFunction = new DeployPackageFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(deployPackageFunction));
            App.Instance.ToolBar.AddFunction(deployPackageFunction);
            App.Instance.Menu.AddFunction(deployPackageFunction, "DOCUMENT");

            CopyPackageDeployResultsFunction copyPackageResultsFunction = new CopyPackageDeployResultsFunction();
            App.Instance.Menu.AddFunction(copyPackageResultsFunction, "DOCUMENT");

            CancelDeployPackageFunction cancelDeployFunction = new CancelDeployPackageFunction();
            App.Instance.Menu.AddFunction(cancelDeployFunction, "DOCUMENT");

            RefreshSourceFileFunction refreshDocumentFunction = new RefreshSourceFileFunction();
            App.Instance.ToolBar.AddFunction(refreshDocumentFunction);
            App.Instance.Menu.AddFunction(refreshDocumentFunction, "DOCUMENT");

            ExportSourceFileDataFunction exportDataFunction = new ExportSourceFileDataFunction();
            App.Instance.ToolBar.AddFunction(exportDataFunction);
            App.Instance.Menu.AddFunction(exportDataFunction, "DOCUMENT");

            ImportSourceFileDataFunction importDataFunction = new ImportSourceFileDataFunction();
            App.Instance.ToolBar.AddFunction(importDataFunction);
            App.Instance.Menu.AddFunction(importDataFunction, "DOCUMENT");

            TextUndoFunction undoTextFunction = new TextUndoFunction();
            App.Instance.Menu.AddFunction(new FunctionSeparator(undoTextFunction), "DOCUMENT");
            App.Instance.Menu.AddFunction(undoTextFunction, "DOCUMENT");

            TextRedoFunction redoTextFunction = new TextRedoFunction();
            App.Instance.Menu.AddFunction(redoTextFunction, "DOCUMENT");
            App.Instance.Menu.AddFunction(new FunctionSeparator(redoTextFunction), "DOCUMENT");

            TextSelectAllFunction selectAllTextFunction = new TextSelectAllFunction();
            App.Instance.Menu.AddFunction(selectAllTextFunction, "DOCUMENT");

            TextCutFunction cutTextFunction = new TextCutFunction();
            App.Instance.Menu.AddFunction(cutTextFunction, "DOCUMENT");

            TextCopyFunction copyTextFunction = new TextCopyFunction();
            App.Instance.Menu.AddFunction(copyTextFunction, "DOCUMENT");

            TextPasteFunction pasteTextFunction = new TextPasteFunction();
            App.Instance.Menu.AddFunction(pasteTextFunction, "DOCUMENT");

            AddCommentFunction addCommentFunction = new AddCommentFunction();
            App.Instance.Menu.AddFunction(new FunctionSeparator(addCommentFunction), "DOCUMENT");
            App.Instance.Menu.AddFunction(addCommentFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(addCommentFunction);

            RemoveCommentFunction removeCommentFunction = new RemoveCommentFunction();
            App.Instance.Menu.AddFunction(removeCommentFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(removeCommentFunction);

            TextGoToLineFunction goToLineFunction = new TextGoToLineFunction();
            App.Instance.RegisterFunction(goToLineFunction);
            App.Instance.Menu.AddFunction(goToLineFunction, "DOCUMENT");

            TextSearchFunction searchTextFunction = new TextSearchFunction();
            App.Instance.Menu.AddFunction(searchTextFunction, "DOCUMENT");

            FoldAllToggleFunction foldAllFunction = new FoldAllToggleFunction();
            App.Instance.Menu.AddFunction(foldAllFunction, "DOCUMENT");

            NewCheckpointFunction newCheckpointFunction = new NewCheckpointFunction();
            //App.Instance.Menu.AddFunction(newCheckpointFunction, "DOCUMENT");
            //App.Instance.ToolBar.AddFunction(newCheckpointFunction);
            App.Instance.RegisterFunction(newCheckpointFunction);

            RunTestsFunction runTestsFunction = new RunTestsFunction();
            App.Instance.Menu.AddFunction(runTestsFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(runTestsFunction);

            RefreshLogsFunction refreshLogsFunction = new RefreshLogsFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(refreshLogsFunction));
            App.Instance.ToolBar.AddFunction(refreshLogsFunction);
            App.Instance.Menu.AddFunction(refreshLogsFunction, "DOCUMENT");

            LogTextSearchFunction logSearchFunction = new LogTextSearchFunction();
            App.Instance.Menu.AddFunction(logSearchFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(logSearchFunction);

            DeleteLogFunction deleteLogFunction = new DeleteLogFunction();
            App.Instance.ToolBar.AddFunction(deleteLogFunction);
            App.Instance.Menu.AddFunction(deleteLogFunction, "DOCUMENT");

            DeleteAllLogsFunction deleteAllLogsFunction = new DeleteAllLogsFunction();
            App.Instance.ToolBar.AddFunction(deleteAllLogsFunction);
            App.Instance.Menu.AddFunction(deleteAllLogsFunction, "DOCUMENT");

            SelectLogUnitLineFunction selectLogUnitTextFunction = new SelectLogUnitLineFunction();
            App.Instance.RegisterFunction(selectLogUnitTextFunction);

            CommitFileOpenFunction commitOpenFunction = new CommitFileOpenFunction();
            App.Instance.Menu.AddFunction(commitOpenFunction, "DOCUMENT");
            App.Instance.RegisterFunction(commitOpenFunction);

            CommitFileCompareFunction commitCompareFunction = new CommitFileCompareFunction();
            App.Instance.Menu.AddFunction(commitCompareFunction, "DOCUMENT");
            App.Instance.RegisterFunction(commitCompareFunction);

            CommitFileShaCopyFunction commitCopyShaFunction = new CommitFileShaCopyFunction();
            App.Instance.Menu.AddFunction(commitCopyShaFunction, "DOCUMENT");
            App.Instance.RegisterFunction(commitCopyShaFunction);

            DiffPreviousFunction diffPreviousFunction = new DiffPreviousFunction();
            App.Instance.Menu.AddFunction(diffPreviousFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(diffPreviousFunction);

            DiffNextFunction diffNextFunction = new DiffNextFunction();
            App.Instance.Menu.AddFunction(diffNextFunction, "DOCUMENT");
            App.Instance.ToolBar.AddFunction(diffNextFunction);

            CloseDocumentFunction closeDocumentFunction = new CloseDocumentFunction();
            App.Instance.Menu.AddFunction(new FunctionSeparator(closeDocumentFunction), "DOCUMENT");
            App.Instance.Menu.AddFunction(closeDocumentFunction, "DOCUMENT");

            CloseAllDocumentsFunction closeAllDocumentsFunction = new CloseAllDocumentsFunction();
            App.Instance.Menu.AddFunction(closeAllDocumentsFunction, "DOCUMENT");

            //
            // HELP
            //

            ApexDocumentationFunction apexDocFunction = new ApexDocumentationFunction();
            App.Instance.Menu.AddFunction(apexDocFunction, "HELP", 0);
        }

        /// <summary>
        /// Open a new project.
        /// </summary>
        /// <param name="project">The project to open.</param>
        public void OpenProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            // check for existing project
            if (CurrentProject != null)
            {
                if (App.MessageUser("The current project must be closed before opening a new one.  Do you want to close the current project?",
                                    "Close Project",
                                    MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) != "Yes")
                    return;

                CloseProject();
            }

            // test the credential
            SalesForceCredential credential = project.Credential;
            while (true)
            {
                try
                {
                    using (App.Wait("Verifying credentials..."))
                        SalesForceClient.TestLogin(credential);

                    project.Repository.AuthorName = project.Client.User.Name;
                    project.Repository.AuthorEmail = project.Client.UserEmail;
                    project.LoadSymbolsAsync(false);

                    break;
                }
                catch (Exception err)
                {
                    App.MessageUser(err.Message, "Login Failed", MessageBoxImage.Error, new string[] { "OK" });

                    EditSalesForceCredentialWindow dlg = new EditSalesForceCredentialWindow();
                    dlg.Credential = credential;
                    dlg.Title = "Enter credentials";

                    if (!App.ShowDialog(dlg))
                        return;

                    credential = dlg.Credential;
                    project.Credential = credential;
                    project.Save();
                }
            }

            // open the project
            using (App.Wait("opening project..."))
            {
                CurrentProject = project;
                App.Instance.SessionTitle = project.Credential.Username;

                App.Instance.Navigation.Nodes.Add(new SourceFolderNode(project));
                App.Instance.Navigation.Nodes.Add(new DataFolderNode(project));
                App.Instance.Navigation.Nodes.Add(new DeployFolderNode(project));
                //App.Instance.Navigation.Nodes.Add(new SnippetsFolderNode(project));

                App.Instance.Menu.UpdateFunctions();
                App.Instance.ToolBar.UpdateFunctions();
            }
        }

        /// <summary>
        /// Close the currently open project.
        /// </summary>
        /// <returns>true if there is no open project.</returns>
        public bool CloseProject()
        {
            bool result = true;

            if (CurrentProject != null)
            {
                if (App.Instance.Content.CloseAllDocuments())
                {
                    App.Instance.Navigation.Nodes.Clear();
                    CurrentProject.Dispose();
                    CurrentProject = null;
                    App.Instance.SessionTitle = null;
                }
                else
                {
                    result = false;
                }

                App.Instance.Menu.UpdateFunctions();
                App.Instance.ToolBar.UpdateFunctions();
            }

            return result;
        }

        #endregion
    }
}
