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
    /// Delete a snippet.
    /// </summary>
    public class DeleteSnippetFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected snippets.
        /// </summary>
        /// <returns>The currently selected snippets.</returns>
        private string[] GetSelectedSnippets()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                List<string> result = new List<string>();
                foreach (INode node in App.Instance.Navigation.SelectedNodes)
                {
                    if (node is SnippetNode)
                        result.Add((node as SnippetNode).Path);
                    else
                        return new string[0];
                }

                return result.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Set the header based on the currently selected snippet.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canDelete = false;

            string[] snippets = GetSelectedSnippets();
            if (snippets.Length > 0)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Delete.png");
                    presenter.ToolTip = (snippets.Length == 1) ?
                        String.Format("Delete {0}", System.IO.Path.GetFileNameWithoutExtension(snippets[0])) :
                        "Delete selected snippets";
                }
                else
                {
                    presenter.Header = (snippets.Length == 1) ?
                        String.Format("Delete {0}", System.IO.Path.GetFileNameWithoutExtension(snippets[0])) :
                        "Delete selected snippets";
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Delete.png");
                }

                canDelete = true;
            }

            IsVisible = canDelete;
        }

        /// <summary>
        /// Delete the selected snippet.
        /// </summary>
        public override void Execute()
        {
            string[] snippets = GetSelectedSnippets();
            if (snippets.Length > 0 && App.Instance.SalesForceApp.CurrentProject != null)
            {
                if (App.MessageUser((snippets.Length == 1) ?
                                        String.Format("Are you sure you want to delete the {0} snippet?", System.IO.Path.GetFileNameWithoutExtension(snippets[0])) :
                                        String.Format("Are you sure you want to delete the {0} selected snippets?", snippets.Length),
                                    "Confirm delete",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    foreach (string snippet in snippets)
                    {
                        using (App.Wait("Deleting snippet(s)"))
                        {
                            // close any open documents
                            IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(snippet);
                            foreach (IDocument document in documents)
                                if (!App.Instance.Content.CloseDocument(document))
                                    return;

                            // delete the manifest
                            System.IO.File.Delete(snippet);

                            // remove nodes
                            INode[] nodes = App.Instance.Navigation.GetNodesByEntity(snippet);
                            foreach (INode node in nodes)
                                node.Presenter.Remove();
                        }
                    }
                }

            }
        }

        #endregion
    }
}
