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

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Launches the apex documentation site.
    /// </summary>
    public class ApexDocumentationFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set header displayed.
        /// </summary>
        /// <param name="host">The type of the host.</param>
        /// <param name="presenter">Presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            presenter.Header = VisualHelper.CreateIconHeader("Apex reference...", null);
        }

        /// <summary>
        /// Display apex documentation site.
        /// </summary>
        public override void Execute()
        {
            try
            {
                using (App.Wait("Opening apex reference..."))
                    System.Diagnostics.Process.Start("http://www.salesforce.com/us/developer/docs/apexcode/index_Left.htm");
            }
            catch
            {
            }
        }

        #endregion
    }
}
