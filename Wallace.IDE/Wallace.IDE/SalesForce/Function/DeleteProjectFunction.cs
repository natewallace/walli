﻿/*
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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Delete a project.
    /// </summary>
    public class DeleteProjectFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "Delete.png");
                presenter.ToolTip = "Delete project...";
            }
            else
            {
                presenter.Header = "Delete project...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Delete.png");
            }
        }

        /// <summary>
        /// Delete a project.
        /// </summary>
        public override void Execute()
        {
            SelectProjectWindow dlg = new SelectProjectWindow();
            dlg.ProjectNames = Project.Projects;
            dlg.SelectLabel = "Delete";
            if (App.ShowDialog(dlg))
            {
                if (App.MessageUser(
                        String.Format("Are you sure you want to delete project '{0}'", dlg.SelectedProjectName),
                        "Confirm Delete",
                        System.Windows.MessageBoxImage.Warning,
                        new string[] { "Yes", "No" }) == "Yes")
                {
                    if (App.Instance.SalesForceApp.CurrentProject != null &&
                        App.Instance.SalesForceApp.CurrentProject.ProjectName == dlg.SelectedProjectName)
                        App.Instance.SalesForceApp.CloseProject();

                    Project.DeleteProject(dlg.SelectedProjectName);
                }
            }
        }

        #endregion
    }
}
