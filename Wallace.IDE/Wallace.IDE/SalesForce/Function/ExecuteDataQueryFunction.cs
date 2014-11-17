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

using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Execute a data query.
    /// </summary>
    public class ExecuteDataQueryFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current data document or null if there isn't one.
        /// </summary>
        private DataEditorDocument CurrentDocument
        {
            get { return App.Instance.Content.ActiveDocument as DataEditorDocument; }
        }

        #endregion

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
                presenter.Header = VisualHelper.CreateIconHeader(null, "Run.png");
                presenter.ToolTip = "Execute SOQL query";
            }
            else
            {
                presenter.Header = "Execute SOQL query";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Run.png");
                presenter.InputGestureText = "F5";
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument != null);
            IsEnabled = (CurrentDocument != null && CurrentDocument.IsExecuteQueryEnabled);
        }

        /// <summary>
        /// Comment out selected lines.
        /// </summary>
        public override void Execute()
        {
            if (CurrentDocument != null)
                CurrentDocument.ExecuteQuery();
        }

        #endregion
    }
}
