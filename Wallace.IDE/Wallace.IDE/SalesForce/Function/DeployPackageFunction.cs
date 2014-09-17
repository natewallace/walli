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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Function that deploys a package.
    /// </summary>
    public class DeployPackageFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current package document or null if there isn't one.
        /// </summary>
        private PackageViewDocument CurrentDocument
        {
            get
            {
                if (App.Instance.SalesForceApp.CurrentProject != null)
                    return App.Instance.Content.ActiveDocument as PackageViewDocument;
                else
                    return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "DeployPackage.png");
                presenter.ToolTip = "Deploy package...";
            }
            else
            {
                presenter.Header = "Deploy package...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "DeployPackage.png");
            }
        }

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument != null);
        }

        /// <summary>
        /// Deploy the package.
        /// </summary>
        public override void Execute()
        {
            if (CurrentDocument != null)
            {
                Package package = CurrentDocument.Package;

                PackageDeployWindow dlg = new PackageDeployWindow();
                dlg.PackageName = package.Name;

                // targets
                dlg.Targets.Add(
                    "SalesForce Project",
                    Project.Projects);

                // target type options
                dlg.TargetTypeOptions.Add(
                    "SalesForce Project",
                    new string[] 
                    {
                        "Check Only",
                        "Run All Tests"
                    });

                // target types
                dlg.TargetTypes = new string[]
                {
                    "SalesForce Project",
                };
                dlg.SelectedTargetType = "SalesForce Project";

                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Deploying package..."))
                    {
                        if (dlg.SelectedTargetType == "SalesForce Project")
                        {
                            PackageDeployToProjectStatusDocument document = new PackageDeployToProjectStatusDocument(
                                package,
                                dlg.SelectedTarget,
                                dlg.SelectedTargetTypeOptions.Contains("Check Only"),
                                dlg.SelectedTargetTypeOptions.Contains("Run All Tests"));

                            App.Instance.Content.OpenDocument(document);
                        }
                        else if (dlg.SelectedTargetType == "Git Repository")
                        {

                        }
                    }
                }
            }
        }

        #endregion
    }
}
