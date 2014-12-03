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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Delete all logs in the current document.
    /// </summary>
    public class DeleteAllLogsFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "DeleteLogAll.png");
                presenter.ToolTip = "Delete all logs...";
            }
            else
            {
                presenter.Header = "Delete all logs...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "DeleteLogAll.png");
            }
        }

        /// <summary>
        /// Set the header based on the currently selected log.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            LogViewerDocument document = App.Instance.Content.ActiveDocument as LogViewerDocument;
            IsVisible = (document != null);
            IsEnabled = (document != null && document.Logs.Count() > 0);
        }

        /// <summary>
        /// Delete the selected log.
        /// </summary>
        public override void Execute()
        {
            LogViewerDocument document = App.Instance.Content.ActiveDocument as LogViewerDocument;

            if (document != null && document.Logs.Count() > 0)
            {
                if (App.MessageUser("Are you sure you want to delete all of the logs?",
                                    "Confirm delete",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    using (App.Wait("Deleting logs"))
                    {
                        App.Instance.SalesForceApp.CurrentProject.Client.DeleteLogs(document.Logs);
                        document.Refresh();
                    }
                }
            }
        }

        #endregion
    }
}
