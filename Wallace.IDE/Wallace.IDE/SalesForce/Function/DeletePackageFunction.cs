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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Node;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Delete a package.
    /// </summary>
    public class DeletePackageFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected manifest or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected manifest or null if there isn't one.</returns>
        private Package GetSelectedPackage()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.Navigation.SelectedNodes.Count == 1 &&
                App.Instance.Navigation.SelectedNodes[0] is PackageNode)
            {
                return (App.Instance.Navigation.SelectedNodes[0] as PackageNode).Package;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the header based on the currently selected package.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canDelete = false;

            Package package = GetSelectedPackage();
            if (package != null)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Delete.png");
                    presenter.ToolTip = String.Format("Delete {0}", package.Name);
                }
                else
                {
                    presenter.Header = String.Format("Delete {0}", package.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Delete.png");
                }

                canDelete = true;
            }

            IsVisible = canDelete;
        }

        /// <summary>
        /// Delete the selected manifest.
        /// </summary>
        public override void Execute()
        {
            Package package = GetSelectedPackage();
            if (package != null && App.Instance.SalesForceApp.CurrentProject != null)
            {
                if (App.MessageUser(String.Format("Are you sure you want to delete the {0} package?", package.Name),
                                    "Confirm delete",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    // close any open documents
                    IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(package);
                    foreach (IDocument document in documents)
                        if (!App.Instance.Content.CloseDocument(document))
                            return;

                    using (App.Wait("Deleting package"))
                    {
                        // delete the package
                        package.Delete();

                        // remove nodes
                        INode[] nodes = App.Instance.Navigation.GetNodesByEntity(package);
                        foreach (INode node in nodes)
                            node.Presenter.Remove();
                    }
                }

            }
        }

        #endregion
    }
}
