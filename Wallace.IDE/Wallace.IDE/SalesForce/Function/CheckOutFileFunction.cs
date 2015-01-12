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

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Check in or out a file.
    /// </summary>
    public class CheckOutFileFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected item or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected item or null if there isn't one.</returns>
        private SourceFileNode GetSelectedNode()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.Navigation.SelectedNodes.Count == 1)
            {
                INode node = App.Instance.Navigation.SelectedNodes[0];
                if (node is SourceFileNode)
                    return node as SourceFileNode;

                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the header based on the currently selected file.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool isCheckIn = false;
            bool isCheckOut = false;
            string name = String.Empty;

            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.IsCheckoutEnabled)
            {
                SourceFileNode node = GetSelectedNode();
                if (node != null && node.SourceFile.Id != null)
                {
                    name = node.SourceFile.Name;

                    if (node.SourceFile.CheckedOutBy != null)
                    {
                        if (node.SourceFile.CheckedOutBy.Equals(project.Client.User))
                            isCheckIn = true;
                    }
                    else
                    {
                        isCheckOut = true;
                    }
                }
            }

            if (host == FunctionHost.Toolbar)
            {
                if (isCheckIn)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                    presenter.ToolTip = String.Format("Check in {0}", name);
                }
                else if (isCheckOut)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                    presenter.ToolTip = String.Format("Check out {0}", name);
                }
            }
            else
            {
                name = name.Replace("_", "__");
                if (isCheckIn)
                {
                    presenter.Header = String.Format("Check in {0}", name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
                }
                else if (isCheckOut)
                {
                    presenter.Header = String.Format("Check out {0}", name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
                }
            }

            IsVisible = isCheckIn || isCheckOut;
        }


        /// <summary>
        /// Check in or check out the file.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.IsCheckoutEnabled)
            {
                using (App.Wait("Updating checkout state..."))
                {
                    SourceFileNode node = GetSelectedNode();
                    if (node != null && node.SourceFile.Id != null)
                    {
                        if (node.SourceFile.CheckedOutBy != null)
                        {
                            if (node.SourceFile.CheckedOutBy.Equals(project.Client.User))
                                project.Client.CheckInFile(node.SourceFile);
                        }
                        else
                        {
                            project.Client.CheckOutFile(node.SourceFile);
                        }
                    }

                    node.UpdateHeader();
                    App.Instance.UpdateWorkspaces();
                }
            }
        }

        #endregion
    }
}
