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
using System;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Function that runs apex tests for a class.
    /// </summary>
    public class RunTestsFunction : TextFunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool isVisible = false;
            bool isEnabled = false;

            if (CurrentDocument is ClassEditorDocument)
            {
                isVisible = (CurrentDocument as ClassEditorDocument).IsTest;
                isEnabled = !CurrentDocument.IsDirty;

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "RunTests.png");
                    presenter.ToolTip = String.Format("Run tests for {0}.", CurrentDocument.File.Name);
                }
                else
                {
                    presenter.Header = String.Format("Run tests for {0}.", CurrentDocument.File.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "RunTests.png");
                }
            }

            IsVisible = isVisible;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Start the test run.
        /// </summary>
        public override void Execute()
        {
            using (App.Wait("Starting tests..."))
            {
                TestRun testRun = App.Instance.SalesForceApp.CurrentProject.Client.StartTests(
                    new SourceFile[] { CurrentDocument.File });

                App.Instance.Content.OpenDocument(new TestRunDocument(App.Instance.SalesForceApp.CurrentProject, testRun));
            }
        }

        #endregion
    }
}
