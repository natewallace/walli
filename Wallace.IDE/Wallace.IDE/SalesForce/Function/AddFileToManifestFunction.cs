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
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Manually add a file to a salesforce manifest.
    /// </summary>
    public class AddFileToManifestFunction : FunctionBase
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
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "AddFileToManifest.png");
                presenter.ToolTip = "Add manual entry...";
            }
            else
            {
                presenter.Header = "Add manual entry...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "AddFileToManifest.png");
            }
        }

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument != null);
        }

        /// <summary>
        /// Add file to the manifest.
        /// </summary>
        public override void Execute()
        {
            EnterValueWindow dlg = new EnterValueWindow();
            dlg.Title = "Manually Enter File";
            dlg.InputLabel = "Enter category and API name separated by a slash\n (for example: ApexClass/BranchController):";
            dlg.ActionLabel = "Add";
            if (App.ShowDialog(dlg))
            {
                if (String.IsNullOrWhiteSpace(dlg.EnteredValue))
                    throw new Exception("You must enter a category and API name to create a manual file entry.");

                string[] parts = dlg.EnteredValue.Split(new char[] { '/' });
                if (parts.Length != 2)
                    throw new Exception("Invalid entry.  You must enter a category and API name in the format CATEGORY/API_NAME.");

                if (CurrentDocument != null)
                    CurrentDocument.AddFile(new SourceFile(parts[0], parts[1]));
            }
        }

        #endregion
    }
}
