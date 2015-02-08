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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Execute a snippet.
    /// </summary>
    public class ExecuteSnippetFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current snippet document or null if there isn't one.
        /// </summary>
        private SnippetEditorDocument CurrentDocument
        {
            get { return App.Instance.Content.ActiveDocument as SnippetEditorDocument; }
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
            if (App.Instance.Content.ActiveDocument is SnippetEditorDocument)
            {
                IsVisible = true;
                string name = System.IO.Path.GetFileNameWithoutExtension(CurrentDocument.Path);

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Run.png");
                    presenter.ToolTip = String.Format("Execute {0}", name);
                }
                else
                {
                    presenter.Header = String.Format("Execute {0}", name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Run.png");
                }
            }
            else
            {
                IsVisible = false;
            }
        }

        /// <summary>
        /// Execute the snippet.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null && CurrentDocument != null)
            {
                if (App.MessageUser("You are about to execute your snippet code on the server.  Are you sure you want to continue?",
                                    "Execute Snippet",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    using (App.Wait("Executing snippet"))
                    {

                    }
                }
            }
        }

        #endregion
    }
}
