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

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Close the current document.
    /// </summary>
    public class CloseDocumentFunction :FunctionBase
    {
        #region Methods

        /// <summary>
        /// Update the header.
        /// </summary>
        /// <param name="host">The host for this function.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            if (App.Instance.Content.ActiveDocument != null)
            {
                if (host == FunctionHost.Menu)
                {
                    presenter.Header = String.Format("Close {0}", App.Instance.Content.ActiveDocument.Text);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
                }
                else
                {
                    presenter.Header = String.Format("Close {0}", App.Instance.Content.ActiveDocument.Text);
                }

                IsVisible = true;
            }
            else
            {
                presenter.Header = null;
                IsVisible = false;
            }
        }

        /// <summary>
        /// Close the active document.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.Content.ActiveDocument != null)
                App.Instance.Content.CloseDocument(App.Instance.Content.ActiveDocument);
        }

        #endregion
    }
}
