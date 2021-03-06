﻿/*
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
    /// Import source file data.
    /// </summary>
    public class ImportSourceFileDataFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current document if there is one, null if there isn't.
        /// </summary>
        public ISourceFileEditorDocument CurrentDocument
        {
            get
            {
                return (App.Instance.Content.ActiveDocument as ISourceFileEditorDocument);
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
                    presenter.Header = VisualHelper.CreateIconHeader(null, "ImportDocument.png");
                    presenter.ToolTip = "Import file...";
                }
                else
                {
                    presenter.Header = "Import file...";
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "ImportDocument.png");
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
        /// Import the data.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                ISourceFileEditorDocument document = CurrentDocument;
                if (document != null)
                {
                    string extension = System.IO.Path.GetExtension(document.File.FileName);
                    if (extension.StartsWith("."))
                        extension = extension.Substring(1);

                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Import file";
                    dlg.Multiselect = false;
                    dlg.Filter = String.Format("{0} Files|*.{1}|All Files|*.*", extension.ToUpper(), extension);

                    bool? result = dlg.ShowDialog();
                    if (result.HasValue && result.Value)
                        document.Content = System.IO.File.ReadAllText(dlg.FileName);
                }
            }
        }

        #endregion
    }
}
