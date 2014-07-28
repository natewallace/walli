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
using Wallace.IDE.SalesForce.Node;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Delete the currently selected class.
    /// </summary>
    public class DeleteSourceFileFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected source file or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected source file or null if there isn't one.</returns>
        private SourceFile GetSelectedSourceFile()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.Navigation.SelectedNodes.Count == 1 &&
                App.Instance.Navigation.SelectedNodes[0] is SourceFileNode)
            {
                SourceFile file = (App.Instance.Navigation.SelectedNodes[0] as SourceFileNode).SourceFile;
                switch (file.FileType.Name)
                {
                    case "ApexClass":
                    case "ApexTrigger":
                    case "ApexPage":
                    case "ApexComponent":
                        return file;

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the header based on the currently selected class.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canDelete = false;

            SourceFile file = GetSelectedSourceFile();
            if (file != null)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Delete.png");
                    presenter.ToolTip = String.Format("Delete {0}", file.Name);
                }
                else
                {
                    presenter.Header = String.Format("Delete {0}", file.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Delete.png");
                }

                canDelete = true;
            }

            IsVisible = canDelete;
        }

        /// <summary>
        /// Delete the selected class.
        /// </summary>
        public override void Execute()
        {
            SourceFile file = GetSelectedSourceFile();
            if (file != null && App.Instance.SalesForceApp.CurrentProject != null)
            {
                if (App.MessageUser(String.Format("Are you sure you want to delete the {0} file?", file.Name),
                                    "Confirm delete",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    // close any open documents
                    IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(file);
                    foreach (IDocument document in documents)
                        if (!App.Instance.Content.CloseDocument(document))
                            return;

                    using (App.Wait("Deleting source file"))
                    {
                        // delete the file
                        App.Instance.SalesForceApp.CurrentProject.Client.DeleteSourceFile(file);

                        // remove nodes
                        INode[] nodes = App.Instance.Navigation.GetNodesByEntity(file);
                        foreach (INode node in nodes)
                            node.Presenter.Remove();
                    }
                }
                
            }
        }

        #endregion
    }
}
