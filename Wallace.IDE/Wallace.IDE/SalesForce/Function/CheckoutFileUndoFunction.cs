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
    /// Undo a checkout.
    /// </summary>
    public class CheckoutFileUndoFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected files.
        /// </summary>
        /// <returns>The currently selected files.</returns>
        private SourceFile[] GetSelectedFiles()
        {
            List<SourceFile> result = new List<SourceFile>();

            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Client.Checkout.IsEnabled())
            {
                foreach (INode node in App.Instance.Navigation.SelectedNodes)
                {
                    if (node is SourceFileNode && project.Client.User.Equals((node as SourceFileNode).SourceFile.CheckedOutBy))
                        result.Add((node as SourceFileNode).SourceFile);
                    else
                        return new SourceFile[0];
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Check to see if the root node is the currently selected node.
        /// </summary>
        /// <returns>true if the root node is the currently selected node.</returns>
        private bool IsRootNodeSelected()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Client.Checkout.IsEnabled())
            {
                if (App.Instance.Navigation.SelectedNodes.Count == 1 &&
                    App.Instance.Navigation.SelectedNodes[0] is SourceFolderNode)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Set the header based on the currently selected file(s).
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool isRootNodeSelected = IsRootNodeSelected();
            SourceFile[] selectedFiles = GetSelectedFiles();

            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "Undo.png");
                presenter.ToolTip = (isRootNodeSelected || selectedFiles.Length > 1) ? "Undo file check outs..." : "Undo file check out...";
            }
            else
            {
                presenter.Header = (isRootNodeSelected || selectedFiles.Length > 1) ? "Undo file check outs..." : "Undo file check out...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Undo.png");
            }

            IsVisible = (isRootNodeSelected || selectedFiles.Length > 0);
        }

        /// <summary>
        /// Check in the file(s).
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && project.Client.Checkout.IsEnabled())
            {
                // get all checkouts
                IDictionary<string, SourceFile> checkoutTable = null;
                using (App.Wait("Getting check outs."))
                    checkoutTable = project.Client.Checkout.GetCheckouts();

                // filter checkouts to current user
                List<SourceFile> userCheckouts = new List<SourceFile>();
                foreach (KeyValuePair<string, SourceFile> kvp in checkoutTable)
                    if (project.Client.User.Equals(kvp.Value.CheckedOutBy))
                        userCheckouts.Add(kvp.Value);

                // show dialog to collect user input
                CheckInWindow dlg = new CheckInWindow();
                dlg.Title = "Undo checkout";
                dlg.CommitTitle = "Undo";
                dlg.ShowComment = false;
                dlg.Files = userCheckouts.ToArray();
                dlg.SelectedFiles = IsRootNodeSelected() ? userCheckouts.ToArray() : GetSelectedFiles();

                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Undoing file(s) checkout."))
                    {
                        // commit to salesforce
                        project.Client.Checkout.CheckinFiles(dlg.SelectedFiles);

                        // update UI
                        HashSet<string> ids = new HashSet<string>();
                        foreach (SourceFile f in dlg.SelectedFiles)
                            ids.Add(f.Id);

                        IEnumerable<SourceFileNode> fileNodes = App.Instance.Navigation.GetNodes<SourceFileNode>();
                        foreach (SourceFileNode fileNode in fileNodes)
                        {
                            if (ids.Contains(fileNode.SourceFile.Id))
                            {
                                fileNode.SourceFile.CheckedOutBy = null;
                                fileNode.UpdateHeader();
                            }
                        }
                    }

                    App.Instance.UpdateWorkspaces();
                }
            }
        }

        #endregion
    }
}
