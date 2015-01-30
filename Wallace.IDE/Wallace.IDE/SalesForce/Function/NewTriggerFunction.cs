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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Create a new trigger.
    /// </summary>
    public class NewTriggerFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "NewDocumentTrigger.png");
                presenter.ToolTip = "New trigger...";
            }
            else
            {
                presenter.Header = "New trigger...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "NewDocumentTrigger.png");
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
        /// Create a new trigger.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                NewTriggerWindow dlg = new NewTriggerWindow();
                dlg.Title = "Create Trigger";

                using (App.Wait("Getting Objects"))
                    dlg.TriggerObjects = project.Client.Data.DescribeGlobal();

                if (App.ShowDialog(dlg))
                {
                    if (String.IsNullOrWhiteSpace(dlg.TriggerName))
                        throw new Exception("A trigger name is required.");
                    if (dlg.TriggerObject == null)
                        throw new Exception("You must select an object for the trigger.");
                    if (dlg.TriggerEvents == TriggerEvents.None)
                        throw new Exception("You must select at least one trigger event.");

                    using (App.Wait("Creating Trigger"))
                    {
                        SourceFile file = project.Client.Meta.CreateTrigger(
                            dlg.TriggerName,
                            (dlg.TriggerObject as SObjectTypePartial).Name,
                            dlg.TriggerEvents,
                            EditorSettings.ApexSettings.CreateHeader());

                        ApexTriggerFolderNode folder = App.Instance.Navigation.GetNode<ApexTriggerFolderNode>();
                        if (folder != null)
                            folder.AddApexTrigger(file);

                        App.Instance.Content.OpenDocument(new TriggerEditorDocument(project, file));
                    }
                }
            }
        }

        #endregion
    }
}
