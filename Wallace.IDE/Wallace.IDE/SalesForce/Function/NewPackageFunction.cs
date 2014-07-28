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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Function that creates a new package.
    /// </summary>
    public class NewPackageFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "NewPackage.png");
                presenter.ToolTip = "Create a new package.";
            }
            else
            {
                presenter.Header = "New package...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "NewPackage.png");
            }
        }

        /// <summary>
        /// Update the visibility.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Opens a new data edit view.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                Project project = App.Instance.SalesForceApp.CurrentProject;
                DeployFolderNode deployNode = App.Instance.Navigation.GetNode<DeployFolderNode>();

                EnterValueWindow dlg = new EnterValueWindow();
                dlg.Title = "Create Package";
                dlg.ActionLabel = "Enter Package Name:";
                dlg.ActionLabel = "Create";
                if (App.ShowDialog(dlg))
                {
                    Package package = new Package(System.IO.Path.Combine(
                        project.DeployFolder,
                        String.Format("{0}.zip", dlg.EnteredValue)));

                    PackageNode packageNode = new PackageNode(project, package);

                    int index = 0;
                    foreach (INode node in deployNode.Presenter.Nodes)
                    {
                        if (node is PackageNode)
                        {
                            int result = package.CompareTo((node as PackageNode).Package);
                            if (result == 0)
                                throw new Exception("There is already a package named: " + package.Name);
                            else if (result < 0)
                                break;
                        }
                        index++;
                    }

                    deployNode.Presenter.Nodes.Insert(index, packageNode);
                    deployNode.Presenter.Expand();
                    deployNode.Presenter.NodeManager.ActiveNode = packageNode;
                    packageNode.DoubleClick();
                }
            }
        }

        #endregion
    }
}
