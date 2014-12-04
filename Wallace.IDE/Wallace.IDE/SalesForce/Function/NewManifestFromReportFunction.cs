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
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Create a new manifest from a report.
    /// </summary>
    public class NewManifestFromReportFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current manifest document or null if there isn't one.
        /// </summary>
        private ReportDocument CurrentDocument
        {
            get
            {
                if (App.Instance.SalesForceApp.CurrentProject != null)
                    return App.Instance.Content.ActiveDocument as ReportDocument;
                else
                    return null;
            }
        }

        #endregion

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
                presenter.Header = VisualHelper.CreateIconHeader(null, "NewManifest.png");
                presenter.ToolTip = "New manifest from selected report items...";
            }
            else
            {
                presenter.Header = "New manifest from selected report items...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "NewManifest.png");
            }
        }

        /// <summary>
        /// Update the visibility.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument != null);
        }

        /// <summary>
        /// Opens a new data edit view.
        /// </summary>
        public override void Execute()
        {
            if (CurrentDocument != null)
            {
                Project project = App.Instance.SalesForceApp.CurrentProject;

                EnterValueWindow dlg = new EnterValueWindow();
                dlg.Title = "Create Manifest";
                dlg.ActionLabel = "Enter Manifest Name:";
                dlg.ActionLabel = "Create";
                if (App.ShowDialog(dlg))
                {
                    Manifest manifest = new Manifest(System.IO.Path.Combine(
                        project.ManifestFolder,
                        String.Format("{0}.xml", dlg.EnteredValue)));

                    if (App.Instance.SalesForceApp.CurrentProject.GetManifests().Contains(manifest))
                        throw new Exception("There is already a manifest named: " + manifest.Name);

                    foreach (SourceFile file in CurrentDocument.SelectedReportItems)
                        manifest.AddItem(file);
                    manifest.Save();

                    ManifestFolderNode manifestFolderNode = App.Instance.Navigation.GetNode<ManifestFolderNode>();
                    if (manifestFolderNode != null)
                        manifestFolderNode.AddManifest(manifest);

                    App.Instance.Content.OpenDocument(new ManifestEditorDocument(project, manifest));
                }
            }
        }

        #endregion
    }
}
