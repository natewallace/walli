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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Configure source control settings.
    /// </summary>
    public class ConfigureSourceControlFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Setup the header.
        /// </summary>
        /// <param name="host">The host for the function.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                presenter.ToolTip = "Configure source control...";
            }
            else
            {
                presenter.Header = "Configure source control...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
            }
        }

        /// <summary>
        /// Set the visibility.
        /// </summary>
        /// <param name="host">The host for the function.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Open dialog and update configuration of source control.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                SourceControlSetupWindow dlg = new SourceControlSetupWindow();
                dlg.RepositoryUrl = project.Repository.RemoteUrl;
                dlg.Username = project.Repository.Username;
                dlg.Password = project.Repository.Password;
                dlg.BranchName = project.Repository.Branch;
                dlg.IsCheckoutEnabled = project.Client.Checkout.IsEnabled();

                dlg.ToggleCheckOutSystemClick += ToggleCheckOutSystem;

                try
                {
                    if (App.ShowDialog(dlg))
                    {
                        project.Repository.RemoteUrl = dlg.RepositoryUrl;
                        project.Repository.Username = dlg.Username;
                        project.Repository.Password = dlg.Password;
                        project.Repository.Branch = dlg.BranchName;
                        project.Save();
                    }
                }
                finally
                {
                    dlg.ToggleCheckOutSystemClick -= ToggleCheckOutSystem;
                }
            }
        }

        /// <summary>
        /// Toggle the checkout system on/off.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ToggleCheckOutSystem(object sender, EventArgs e)
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                string text = project.Client.Checkout.IsEnabled() ?
                    "Disabling the check out system will delete a previously created custom object and impact all Walli users of the organization.  Are you sure you want to proceed?" :
                    "Enabling the check out system requires a new custom object be created and will impact all Walli users of the organization.  Are you sure you want to proceed?";

                if (App.MessageUser(text,
                                    "Check out system",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    using (App.Wait("Updating check out system"))
                    {
                        project.Client.Checkout.Enable(!project.Client.Checkout.IsEnabled());
                        App.Instance.UpdateWorkspaces();

                        // refresh open folders
                        IFunction refreshFunction = App.Instance.GetFunction<RefreshFolderFunction>();
                        INode currentActiveNode = App.Instance.Navigation.ActiveNode;

                        ApexClassFolderNode classFolderNode = App.Instance.Navigation.GetNode<ApexClassFolderNode>();
                        if (classFolderNode != null)
                        {
                            App.Instance.Navigation.ActiveNode = classFolderNode;
                            refreshFunction.Execute();
                        }

                        ApexTriggerFolderNode triggerFolderNode = App.Instance.Navigation.GetNode<ApexTriggerFolderNode>();
                        if (triggerFolderNode != null)
                        {
                            App.Instance.Navigation.ActiveNode = triggerFolderNode;
                            refreshFunction.Execute();
                        }

                        ApexPageFolderNode pageFolderNode = App.Instance.Navigation.GetNode<ApexPageFolderNode>();
                        if (pageFolderNode != null)
                        {
                            App.Instance.Navigation.ActiveNode = pageFolderNode;
                            refreshFunction.Execute();
                        }

                        ApexComponentFolderNode componentFolderNode = App.Instance.Navigation.GetNode<ApexComponentFolderNode>();
                        if (componentFolderNode != null)
                        {
                            App.Instance.Navigation.ActiveNode = componentFolderNode;
                            refreshFunction.Execute();
                        }

                        App.Instance.Navigation.ActiveNode = currentActiveNode;
                    }

                    App.MessageUser("The check out system has been changed.  All users that currently have an open project in Walli for this organization will need to close those projects and then reopen them to see the change take effect.",
                                    "Check out system changed",
                                    System.Windows.MessageBoxImage.Information,
                                    new string[] { "OK" });
                }

                SourceControlSetupWindow dlg = sender as SourceControlSetupWindow;
                if (dlg != null)
                    dlg.IsCheckoutEnabled = project.Client.Checkout.IsEnabled();
            }
        }

        #endregion
    }
}
