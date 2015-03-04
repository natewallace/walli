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
using Microsoft.Win32;
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
        public ITextEditorDocument CurrentDocument
        {
            get
            {
                return (App.Instance.Content.ActiveDocument as ITextEditorDocument);
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
            if (CurrentDocument != null)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "ExportDocument.png");
                    presenter.ToolTip = "Export file...";
                }
                else
                {
                    presenter.Header = "Export file...";
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
                ITextEditorDocument document = CurrentDocument;
                if (document != null)
                {
                    string name = "file";
                    string extension = "txt";

                    if (document is ISourceFileEditorDocument)
                    {
                        name = System.IO.Path.GetFileName((document as ISourceFileEditorDocument).File.FileName);
                        extension = System.IO.Path.GetExtension(name);
                        if (extension.StartsWith("."))
                            extension = extension.Substring(1);
                    }

                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Title = "Export file";
                    dlg.OverwritePrompt = true;
                    dlg.AddExtension = true;
                    dlg.Filter = String.Format("{0} Files|*.{1}|All Files|*.*", extension.ToUpper(), extension);
                    dlg.FileName = name;

                    bool? result = dlg.ShowDialog();
                    if (result.HasValue && result.Value)
                        System.IO.File.WriteAllText(dlg.FileName, document.Content);
                }
            }
        }

        #endregion
    }
}
