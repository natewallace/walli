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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Create a new local project.
    /// </summary>
    public class NewProjectFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "NewProject.png");
                presenter.ToolTip = "Create a new project...";
            }
            else
            {
                presenter.Header = "New project...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "NewProject.png");
            }
        }        

        /// <summary>
        /// Create a new local project.
        /// </summary>
        public override void Execute()
        {
            SalesForceCredential credential = null;

            // get credentials for new project.
            EditSalesForceCredentialWindow dlg = new EditSalesForceCredentialWindow();
            dlg.Title = "New Project";
            while (App.ShowDialog(dlg))
            {
                try
                {
                    using (App.Wait("Verifying credentials..."))
                    {
                        credential = dlg.Credential;
                        SalesForceClient.TestLogin(credential);
                    }
                    break;
                }
                catch (Exception err)
                {
                    App.MessageUser(err.Message, "Login Failed", MessageBoxImage.Error, new string[] { "OK" });

                    dlg = new EditSalesForceCredentialWindow();
                    dlg.Title = "New Project";
                    dlg.Credential = credential;
                }
            }

            // create the new project or open an existing project that has the same credentials
            if (credential != null)
            {
                Project project = null;
                if (Project.ProjectExists(credential.Username))
                {
                    project = Project.OpenProject(credential.Username);
                    project.Credential = credential;
                }
                else
                {
                    project = new Project(credential);
                }

                project.Save();

                App.Instance.SalesForceApp.OpenProject(project);
            }
        }

        #endregion
    }
}
