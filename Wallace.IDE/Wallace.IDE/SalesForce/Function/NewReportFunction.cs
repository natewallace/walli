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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// This function generates a new report.
    /// </summary>
    public class NewReportFunction : FunctionBase
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
                presenter.Header = VisualHelper.CreateIconHeader(null, "Report.png");
                presenter.ToolTip = "New report...";
            }
            else
            {
                presenter.Header = "New report...";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Report.png");
            }
        }

        /// <summary>
        /// Update the visibility.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Generate a new report.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                // setup type names
                SourceFileType[] types = null;
                using (App.Wait("Getting types"))
                    types = App.Instance.SalesForceApp.CurrentProject.Client.GetSourceFileTypes();

                List<string> typeNames = new List<string>();
                foreach (SourceFileType sft in types)
                    typeNames.Add(sft.Name);
                typeNames.Sort();

                NewReportWindow dlg = new NewReportWindow();
                dlg.Project = App.Instance.SalesForceApp.CurrentProject;
                dlg.Exclusions = typeNames.ToArray();

                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Creating report"))
                    {
                        string[] exclusions = dlg.SelectedExclusions;
                        List<SourceFileType> typeList = new List<SourceFileType>();
                        foreach (SourceFileType sft in types)
                            if (!exclusions.Contains(sft.Name))
                                typeList.Add(sft);

                        SourceFile[] allFiles = dlg.Project.Client.GetSourceFiles(
                            typeList.ToArray(),
                            true);
                        SourceFile[] filteredFiles = dlg.ReportFilter.Filter(allFiles);
                        ReportDocument document = new ReportDocument(filteredFiles);
                        App.Instance.Content.OpenDocument(document);
                    }
                }
            }
        }

        #endregion
    }
}
