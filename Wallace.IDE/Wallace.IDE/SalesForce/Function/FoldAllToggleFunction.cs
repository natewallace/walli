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
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Fold or unfold all folding sections.
    /// </summary>
    public class FoldAllToggleFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// Get/Set the fold all toggle value for the current document.
        /// </summary>
        private bool? FoldAllToggle
        {
            get
            {
                if (App.Instance.Content.ActiveDocument is ClassEditorDocument)
                    return (App.Instance.Content.ActiveDocument as ClassEditorDocument).FoldAllToggle;
                else if (App.Instance.Content.ActiveDocument is TriggerEditorDocument)
                    return (App.Instance.Content.ActiveDocument as TriggerEditorDocument).FoldAllToggle;
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                {
                    if (App.Instance.Content.ActiveDocument is ClassEditorDocument)
                        (App.Instance.Content.ActiveDocument as ClassEditorDocument).FoldAllToggle = value.Value;
                    else if (App.Instance.Content.ActiveDocument is TriggerEditorDocument)
                        (App.Instance.Content.ActiveDocument as TriggerEditorDocument).FoldAllToggle = value.Value;
                }
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
            bool? toggle = FoldAllToggle;
            if (!toggle.HasValue)
            {
                IsVisible = false;
            }
            else
            {
                if (host == FunctionHost.Toolbar)
                {
                    if (toggle.Value)
                    {
                        presenter.Header = VisualHelper.CreateIconHeader(null, "FoldExpand.png");
                        presenter.ToolTip = String.Format("Expand all folded sections");
                    }
                    else
                    {
                        presenter.Header = VisualHelper.CreateIconHeader(null, "FoldCollapse.png");
                        presenter.ToolTip = String.Format("Collapse all unfolded sections");
                    }
                }
                else
                {
                    if (toggle.Value)
                    {
                        presenter.Header = String.Format("Expand all folded sections");
                        presenter.Icon = VisualHelper.CreateIconHeader(null, "FoldExpand.png");
                    }
                    else
                    {
                        presenter.Header = String.Format("Collapse all unfolded sections");
                        presenter.Icon = VisualHelper.CreateIconHeader(null, "FoldCollapse.png");
                    }
                }

                IsVisible = true;
            }
        }

        /// <summary>
        /// Toggle the fold sections.
        /// </summary>
        public override void Execute()
        {
            bool? toggle = FoldAllToggle;
            if (toggle.HasValue)
            {
                FoldAllToggle = !toggle.Value;
                App.Instance.UpdateWorkspaces();
            }
        }

        #endregion
    }
}
