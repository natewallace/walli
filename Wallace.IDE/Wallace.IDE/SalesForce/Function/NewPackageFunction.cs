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
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Function that creates a new package from a manifest.
    /// </summary>
    public class NewPackageFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current manifest document or null if there isn't one.
        /// </summary>
        private ManifestEditorDocument CurrentDocument
        {
            get
            {
                if (App.Instance.SalesForceApp.CurrentProject != null)
                    return App.Instance.Content.ActiveDocument as ManifestEditorDocument;
                else
                    return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canCreate = false;
            bool isDirty = false;

            if (CurrentDocument != null)
            {
                Manifest manifest = CurrentDocument.Manifest;
                isDirty = CurrentDocument.IsDirty;

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "NewPackage.png");
                    presenter.ToolTip = "Create package.";
                }
                else
                {
                    presenter.Header = "Create package";
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "NewPackage.png");
                }

                canCreate = true;
            }

            IsVisible = canCreate;
            IsEnabled = !isDirty;
        }

        /// <summary>
        /// Opens a new data edit view.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null && CurrentDocument != null)
            {
                Project project = App.Instance.SalesForceApp.CurrentProject;
                Manifest manifest = CurrentDocument.Manifest;

                NewPackageWindow dlg = new NewPackageWindow();
                dlg.PackageManifestName = manifest.Name;
                if (App.ShowDialog(dlg))
                {
                    string fileName = System.IO.Path.Combine(project.PackageFolder, String.Format("{0}.zip", dlg.PackageName));
                    if (System.IO.File.Exists(fileName))
                        throw new Exception("A package with the given name already exists: " + dlg.PackageName);

                    using (App.Wait("Creating package"))
                    {
                        Package package = new Package(fileName, dlg.IsPackageDestructive, manifest);
                        package.Save(project.Client);

                        PackageFolderNode packageFolderNode = App.Instance.Navigation.GetNode<PackageFolderNode>();
                        if (packageFolderNode != null)
                            packageFolderNode.AddPackage(package);

                        //App.Instance.Content.OpenDocument(new ManifestEditorDocument(project, manifest));
                    }
                }
            }
        }

        #endregion
    }
}
