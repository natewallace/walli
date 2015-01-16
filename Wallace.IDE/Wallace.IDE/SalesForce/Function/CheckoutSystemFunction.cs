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

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Enables and disables the checkout system.
    /// </summary>
    public class CheckOutSystemFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the header based on the currently selected log.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project == null)
            {
                IsVisible = false;
            }
            else
            {
                string text = project.IsCheckoutEnabled ? "Disable check out system" : "Enable check out system";

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                    presenter.ToolTip = text;
                }
                else
                {
                    presenter.Header = text;
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
                }

                IsVisible = true;
            }
        }

        /// <summary>
        /// Toggle the checkout enabled state.
        /// </summary>
        public override void Execute()
        {
            //Project project = App.Instance.SalesForceApp.CurrentProject;

            //SimpleRepository repo = new SimpleRepository();
            //repo.WorkingPath = project.RepositoryFolder;
            //repo.RemoteUrl = "ssh://dux07@caexvd0096:22/app/git/SFDC-CI";
            //repo.Branch = "NateTest";
            //repo.Username = "dux07";
            //repo.Password = "kinghipp0";
            //repo.Clone();
            //repo.PushPackage(null);
            

            //Project project = App.Instance.SalesForceApp.CurrentProject;
            //if (project != null)
            //{
            //    string text = project.IsCheckoutEnabled ?
            //        "Disabling the check out system will delete a previously created custom object and impact all Walli users of the organization.  Are you sure you want to proceed?" :
            //        "Enabling the check out system requires a new custom object be created and will impact all Walli users of the organization.  Are you sure you want to proceed?";

            //    if (App.MessageUser(text,
            //                        "Check out system",
            //                        System.Windows.MessageBoxImage.Warning,
            //                        new string[] { "Yes", "No" }) == "Yes")
            //    {
            //        using (App.Wait("Updating check out system"))
            //        {
            //            project.EnableCheckout(!project.IsCheckoutEnabled);
            //            App.Instance.UpdateWorkspaces();

            //            // refresh open folders
            //            IFunction refreshFunction = App.Instance.GetFunction<RefreshFolderFunction>();
            //            INode currentActiveNode = App.Instance.Navigation.ActiveNode;

            //            ApexClassFolderNode classFolderNode = App.Instance.Navigation.GetNode<ApexClassFolderNode>();
            //            if (classFolderNode != null)
            //            {
            //                App.Instance.Navigation.ActiveNode = classFolderNode;
            //                refreshFunction.Execute();
            //            }

            //            ApexTriggerFolderNode triggerFolderNode = App.Instance.Navigation.GetNode<ApexTriggerFolderNode>();
            //            if (triggerFolderNode != null)
            //            {
            //                App.Instance.Navigation.ActiveNode = triggerFolderNode;
            //                refreshFunction.Execute();
            //            }

            //            ApexPageFolderNode pageFolderNode = App.Instance.Navigation.GetNode<ApexPageFolderNode>();
            //            if (pageFolderNode != null)
            //            {
            //                App.Instance.Navigation.ActiveNode = pageFolderNode;
            //                refreshFunction.Execute();
            //            }

            //            ApexComponentFolderNode componentFolderNode = App.Instance.Navigation.GetNode<ApexComponentFolderNode>();
            //            if (componentFolderNode != null)
            //            {
            //                App.Instance.Navigation.ActiveNode = componentFolderNode;
            //                refreshFunction.Execute();
            //            }

            //            App.Instance.Navigation.ActiveNode = currentActiveNode;
            //        }

            //        App.MessageUser("The check out system has been changed.  All users that currently have an open project in Walli for this organization will need to close those projects and then reopen them to see the change take effect.",
            //                        "Check out system changed",
            //                        System.Windows.MessageBoxImage.Information,
            //                        new string[] { "OK" });
            //    }
            //}
        }

        #endregion
    }
}
