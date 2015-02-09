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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Manages all the snippets that can be inserted.
    /// </summary>
    public class InsertSnippetContainerFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current document.
        /// </summary>
        private ITextEditorDocument CurrentDocument
        {
            get { return App.Instance.Content.ActiveDocument as ITextEditorDocument; }
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                presenter.ToolTip = "Insert snippet";
            }
            else
            {
                presenter.Header = "Insert snippet";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument != null);
        }

        /// <summary>
        /// Build out the sub menu items.
        /// </summary>
        /// <param name="manager">The manager that this </param>
        public void Refresh(IFunctionManager manager)
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                FunctionGrouping systemGroup = new FunctionGrouping("SALESFORCE_SYSTEM_SNIPPET", "System");
                manager.RemoveFunction(systemGroup);
                manager.AddFunction(systemGroup, Id);
                foreach (string snippet in project.GetSystemSnippets())
                    manager.AddFunction(new InsertSnippetFunction(snippet), "SALESFORCE_SYSTEM_SNIPPET");

                FunctionGrouping projectGroup = new FunctionGrouping("SALESFORCE_PROJECT_SNIPPET", "Project");
                manager.RemoveFunction(projectGroup);
                manager.AddFunction(projectGroup, Id);
                foreach (string snippet in project.GetProjectSnippets())
                    manager.AddFunction(new InsertSnippetFunction(snippet), "SALESFORCE_PROJECT_SNIPPET");
            }
            
        }

        #endregion
    }
}
