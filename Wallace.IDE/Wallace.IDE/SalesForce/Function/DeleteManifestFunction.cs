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
    /// Delete a manifest.
    /// </summary>
    public class DeleteManifestFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected manifests.
        /// </summary>
        /// <returns>The currently selected manifests.</returns>
        private Manifest[] GetSelectedManifests()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                List<Manifest> result = new List<Manifest>();
                foreach (INode node in App.Instance.Navigation.SelectedNodes)
                {
                    if (node is ManifestNode)
                        result.Add((node as ManifestNode).Manifest);
                    else
                        return new Manifest[0];
                }

                return result.ToArray();
            }
            else
            {
                return new Manifest[0];
            }
        }

        /// <summary>
        /// Set the header based on the currently selected manifest.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canDelete = false;

            Manifest[] manifests = GetSelectedManifests();
            if (manifests.Length > 0)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Delete.png");
                    presenter.ToolTip = (manifests.Length == 1 ) ?
                        String.Format("Delete {0}", manifests[0].Name) :
                        "Delete selected manifests";
                }
                else
                {
                    presenter.Header = (manifests.Length == 1) ?
                        String.Format("Delete {0}", manifests[0].Name) :
                        "Delete selected manifests";
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
            Manifest[] manifests = GetSelectedManifests();
            if (manifests.Length > 0 && App.Instance.SalesForceApp.CurrentProject != null)
            {
                if (App.MessageUser((manifests.Length == 1) ?
                                        String.Format("Are you sure you want to delete the {0} manifest?", manifests[0].Name) :
                                        String.Format("Are you sure you want to delete the {0} selected manifests?", manifests.Length),
                                    "Confirm delete",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    foreach (Manifest manifest in manifests)
                    {
                        using (App.Wait("Deleting manifest(s)"))
                        {
                            // close any open documents
                            IDocument[] documents = App.Instance.Content.GetDocumentsByEntity(manifest);
                            foreach (IDocument document in documents)
                                if (!App.Instance.Content.CloseDocument(document))
                                    return;

                            // delete the manifest
                            manifest.Delete();

                            // remove nodes
                            INode[] nodes = App.Instance.Navigation.GetNodesByEntity(manifest);
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
