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

using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Create a new component.
    /// </summary>
    public class NewComponentFunction : FunctionBase
    {
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "NewDocumentPage.png");
                presenter.ToolTip = "New component...";
            }
            else
            {
                presenter.Header = "New component...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "NewDocumentPage.png");
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Create a new component.
        /// </summary>
        public override void Execute()
        {
            EnterValueWindow dlg = new EnterValueWindow();
            dlg.Title = "Create Component";
            dlg.InputLabel = "Component Name:";
            dlg.ActionLabel = "Create";
            dlg.InputMaxLength = 80;
            if (App.ShowDialog(dlg))
            {
                Project project = App.Instance.SalesForceApp.CurrentProject;
                if (project != null)
                {
                    using (App.Wait("Creating Component"))
                    {
                        SourceFile file = project.Client.CreateComponent(dlg.EnteredValue);
                        ApexComponentFolderNode folder = App.Instance.Navigation.GetNode<ApexComponentFolderNode>();
                        if (folder != null)
                            folder.AddApexComponent(file);

                        App.Instance.Content.OpenDocument(new VisualForceEditorDocument(project, file));
                    }
                }
            }
        }

        #endregion
    }
}
