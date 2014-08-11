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
using System.Windows;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Function;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

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
        /// Load functions for this application.
        /// </summary>
        public void LoadFunctions()
        {
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

            //Planned for future release
            //NewPackageFunction newPackageFunction = new NewPackageFunction();
            //App.Instance.Menu.AddFunction(newPackageFunction, "NEWSALESFORCE");
            //App.Instance.RegisterFunction(newPackageFunction);

            DataEditFunction dataEdit = new DataEditFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(dataEdit));
            App.Instance.ToolBar.AddFunction(dataEdit);
            App.Instance.Menu.AddFunction(dataEdit, "PROJECT");

            DeleteSourceFileFunction deleteSourceFileFunction = new DeleteSourceFileFunction();
            App.Instance.Menu.AddFunction(deleteSourceFileFunction, "PROJECT");
            App.Instance.RegisterFunction(deleteSourceFileFunction);

            RefreshFolderFunction refreshFolderFunction = new RefreshFolderFunction();
            App.Instance.RegisterFunction(refreshFolderFunction);

            PropertiesFunction propertiesSourceFileFunction = new PropertiesFunction();
            App.Instance.Menu.AddFunction(propertiesSourceFileFunction, "PROJECT");
            App.Instance.RegisterFunction(propertiesSourceFileFunction);

            SaveSourceFileFunction saveSourceFileFunction = new SaveSourceFileFunction();
            App.Instance.ToolBar.AddFunction(new FunctionSeparator(saveSourceFileFunction));
            App.Instance.ToolBar.AddFunction(saveSourceFileFunction);
            App.Instance.Menu.AddFunction(saveSourceFileFunction, "DOCUMENT");

            RefreshSourceFileFunction refreshDocumentFunction = new RefreshSourceFileFunction();
            App.Instance.ToolBar.AddFunction(refreshDocumentFunction);
            App.Instance.Menu.AddFunction(refreshDocumentFunction, "DOCUMENT");
            App.Instance.Menu.AddFunction(new FunctionSeparator(refreshDocumentFunction), "DOCUMENT");

            TextUndoFunction undoTextFunction = new TextUndoFunction();
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
            App.Instance.Menu.AddFunction(new FunctionSeparator(pasteTextFunction), "DOCUMENT");

            TextGoToLineFunction goToLineFunction = new TextGoToLineFunction();
            App.Instance.RegisterFunction(goToLineFunction);
            App.Instance.Menu.AddFunction(goToLineFunction, "DOCUMENT");

            TextSearchFunction searchTextFunction = new TextSearchFunction();
            App.Instance.Menu.AddFunction(searchTextFunction, "DOCUMENT");
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

                    project.LoadSymbolsAsync();

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
            CurrentProject = project;
            App.Instance.SessionTitle = project.Credential.Username;
            
            App.Instance.Navigation.Nodes.Add(new DataFolderNode(project));
            //Planned for future release
            //App.Instance.Navigation.Nodes.Add(new DeployFolderNode(project));
            App.Instance.Navigation.Nodes.Add(new SourceFolderNode(project));

            App.Instance.Menu.UpdateFunctions();
            App.Instance.ToolBar.UpdateFunctions();
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
