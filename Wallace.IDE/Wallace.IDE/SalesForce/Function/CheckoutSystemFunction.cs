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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Enables and disables the checkout system.
    /// </summary>
    public class CheckOutSystemFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the header based on the currently selected log.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project == null)
            {
                IsVisible = false;
            }
            else
            {
                string text = project.IsCheckoutEnabled ? "Disable checkout system" : "Enable checkout system";

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Empty.png");
                    presenter.ToolTip = text;
                }
                else
                {
                    presenter.Header = text;
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
                }

                IsVisible = true;
            }
        }

        /// <summary>
        /// Toggle the checkout enabled state.
        /// </summary>
        public override void Execute()
        {
            Project project = App.Instance.SalesForceApp.CurrentProject;
            if (project != null)
            {
                if (App.MessageUser("Changing the checkout system will impact all users of the organization.  Are you sure you want to proceed?",
                                    "Checkout system update",
                                    System.Windows.MessageBoxImage.Warning,
                                    new string[] { "Yes", "No" }) == "Yes")
                {
                    using (App.Wait("Updating checkout system"))
                    {
                        project.EnableCheckout(!project.IsCheckoutEnabled);
                        App.Instance.UpdateWorkspaces();
                    }
                }
            }
        }

        #endregion
    }
}
