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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Search for text in code files.
    /// </summary>
    public class SearchFilesFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "FindInFiles.png");
                presenter.ToolTip = "Search files...";
            }
            else
            {
                presenter.Header = "Search files...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "FindInFiles.png");
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
        /// Comment out selected lines.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                EnterValueWindow dlg = new EnterValueWindow();
                dlg.Title = "Search Files";
                dlg.InputLabel = "Enter the text to search for\n(only files that have been indexed will be searched):";
                dlg.ActionLabel = "Search";
                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Searching"))
                    {
                        using (SearchIndex searchIndex = new SearchIndex(App.Instance.SalesForceApp.CurrentProject.SearchFolder))
                        {
                            SearchFilesResultDocument document = new SearchFilesResultDocument(
                                App.Instance.SalesForceApp.CurrentProject,
                                dlg.EnteredValue,
                                searchIndex.DocumentCount,
                                searchIndex.Search(String.Format("\"{0}\"", dlg.EnteredValue)));

                            App.Instance.Content.OpenDocument(document);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
