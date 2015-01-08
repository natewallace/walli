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

using SalesForceData;
using SalesForceLanguage.Apex.CodeModel;
using System.Collections.Generic;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Opens the test manager.
    /// </summary>
    public class TestManagerFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "Test.png");
                presenter.ToolTip = "Test manager...";
            }
            else
            {
                presenter.Header = "Test manager...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Test.png");
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
        /// Open the test manager.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                List<string> names = new List<string>();
                foreach (SymbolTable symbol in App.Instance.SalesForceApp.CurrentProject.Language.GetAllSymbols())
                {
                    if (symbol.HasAttribute("IsTest"))
                        names.Add(symbol.Name);
                }
                names.Sort();

                TestManagerWindow dlg = new TestManagerWindow();
                dlg.TestNames = names;

                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Starting tests..."))
                    {
                        TestRun testRun = App.Instance.SalesForceApp.CurrentProject.Client.StartTests(dlg.SelectedTestNames);
                        App.Instance.Content.OpenDocument(new TestRunDocument(App.Instance.SalesForceApp.CurrentProject, testRun));
                    }
                }
            }
        }

        #endregion
    }
}
