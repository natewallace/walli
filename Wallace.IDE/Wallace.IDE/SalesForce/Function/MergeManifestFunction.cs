﻿/*
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
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Merge one manifest into another.
    /// </summary>
    public class MergeManifestFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected manifest or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected manifest or null if there isn't one.</returns>
        private Manifest GetSelectedManifest()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.Navigation.SelectedNodes.Count == 1 &&
                App.Instance.Navigation.SelectedNodes[0] is ManifestNode)
            {
                return (App.Instance.Navigation.SelectedNodes[0] as ManifestNode).Manifest;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the header .
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "MergeManifest.png");
                presenter.ToolTip = "Merge Manifests...";
            }
            else
            {
                presenter.Header = "Merge Manifests...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "MergeManifest.png");
            }           
        }

        /// <summary>
        /// Set the header based on the currently selected manifest.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Merge manifests.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                Manifest[] manifests = project.GetManifests();
                Manifest manifest = GetSelectedManifest();

                MergeManifestWindow dlg = new MergeManifestWindow();
                dlg.ManifestSources = manifests;
                dlg.ManifestTargets = manifests;
                dlg.ManifestSource = GetSelectedManifest();
                if (App.ShowDialog(dlg))
                {
                    IDocument[] targetDocuments = App.Instance.Content.GetDocumentsByEntity(dlg.ManifestTarget);
                    ManifestEditorDocument targetDocument = targetDocuments.FirstOrDefault(d => d is ManifestEditorDocument) as ManifestEditorDocument;
                    if (targetDocument == null)
                        targetDocument = new ManifestEditorDocument(project, dlg.ManifestTarget);

                    targetDocument.Merge(dlg.ManifestSource);
                    App.Instance.Content.OpenDocument(targetDocument);
                }
            }
        }

        #endregion
    }
}