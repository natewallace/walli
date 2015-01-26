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

using SalesForceData;
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
    /// Check out a file.
    /// </summary>
    public class CheckoutFileFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected nodes.
        /// </summary>
        /// <returns>The currently selected nodes.</returns>
        private SourceFileNode[] GetSelectedNodes()
        {
            List<SourceFileNode> result = new List<SourceFileNode>();

            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Client.Checkout.IsEnabled())
            {
                foreach (INode node in App.Instance.Navigation.SelectedNodes)
                {
                    if (node is SourceFileNode &&
                        (node as SourceFileNode).SourceFile.Parent == null &&
                        (node as SourceFileNode).SourceFile.CheckedOutBy == null)
                        result.Add(node as SourceFileNode);
                    else
                        return new SourceFileNode[0];
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Set the header based on the currently selected file(s).
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            SourceFileNode[] selectedNodes = GetSelectedNodes();

            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "CheckIn.png");
                presenter.ToolTip = (selectedNodes.Length > 1) ? "Check out files" : "Check out file";
            }
            else
            {
                presenter.Header = (selectedNodes.Length > 1) ? "Check out files" : "Check out file";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "CheckIn.png");
            }

            IsVisible = (selectedNodes.Length > 0);
        }

        /// <summary>
        /// Check in or check out the file.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Client.Checkout.IsEnabled())
            {
                SourceFileNode[] nodes = GetSelectedNodes();
                if (nodes.Length > 0)
                {
                    List<SourceFile> files = new List<SourceFile>();
                    foreach (SourceFileNode node in nodes)
                        files.Add(node.SourceFile);

                    using (App.Wait("Checking out file."))
                    {
                        project.Client.Checkout.CheckoutFiles(files);
                    }
                }

                foreach (SourceFileNode node in nodes)
                    node.UpdateHeader();

                App.Instance.UpdateWorkspaces();
            }
        }

        #endregion
    }
}
