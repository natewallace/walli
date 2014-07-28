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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Refresh the current document.
    /// </summary>
    public class RefreshSourceFileFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            if (App.Instance.Content.ActiveDocument is ISourceFileEditorDocument)
            {
                ISourceFileEditorDocument document = App.Instance.Content.ActiveDocument as ISourceFileEditorDocument;
                IsVisible = true;

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Refresh.png");
                    presenter.ToolTip = String.Format("Refresh {0}", document.File.Name);
                }
                else
                {
                    presenter.Header = String.Format("Refresh {0}", document.File.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Refresh.png");
                }
            }
            else
            {
                IsVisible = false;
            }
        }

        /// <summary>
        /// Reload the document.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.Content.ActiveDocument is ISourceFileEditorDocument)
            {
                (App.Instance.Content.ActiveDocument as ISourceFileEditorDocument).Reload();
            }
        }

        #endregion
    }
}
