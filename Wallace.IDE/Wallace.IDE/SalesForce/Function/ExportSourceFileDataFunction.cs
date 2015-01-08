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
using Microsoft.Win32;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Export source file data.
    /// </summary>
    public class ExportSourceFileDataFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current document if there is one, null if there isn't.
        /// </summary>
        public SourceFileEditorDocument CurrentDocument
        {
            get
            {
                if (App.Instance.Content.ActiveDocument is SourceFileEditorDocument)
                    return App.Instance.Content.ActiveDocument as SourceFileEditorDocument;
                else
                    return null;
            }
        }

        /// <summary>
        /// The current data if there is any, null if there isn't.
        /// </summary>
        public SourceFileData CurrentData
        {
            get
            {
                if (CurrentDocument != null && !CurrentDocument.IsTextVisible)
                    return CurrentDocument.Data;
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
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            if (CurrentData != null)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "ExportDocument.png");
                    presenter.ToolTip = String.Format("Export {0} data", CurrentData.Name);
                }
                else
                {
                    presenter.Header = String.Format("Export {0} data", CurrentData.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "ExportDocument.png");
                }

                IsEnabled = true;
                IsVisible = true;
            }
            else
            {
                IsEnabled = false;
                IsVisible = false;
            }
        }

        /// <summary>
        /// Export the data.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                SourceFileEditorDocument document = CurrentDocument;
                if (document != null && CurrentData != null)
                {
                    StringBuilder name = new StringBuilder(CurrentData.Name);
                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                        name.Replace(c, '_');

                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Title = "Export data";
                    dlg.OverwritePrompt = true;
                    dlg.AddExtension = true;
                    dlg.Filter = "XML Files|*.xml|All Files|*.*";
                    dlg.FileName = name.ToString();
                    
                    bool? result = dlg.ShowDialog();
                    if (result.HasValue && result.Value)
                        document.ExportData(dlg.FileName);
                }
            }
        }

        #endregion
    }
}
