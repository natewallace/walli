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
using SalesForceData;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Save changes that have been made to a source file.
    /// </summary>
    public class SaveSourceFileFunction : TextFunctionBase
    {
        #region Methods

        /// <summary>
        /// Set the visibility of the header.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            bool canSave = false;
            bool isDirty = false;

            if (CurrentDocument != null)
            {
                SourceFile file = CurrentDocument.File;
                isDirty = CurrentDocument.IsDirty;

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Save.png");
                    presenter.ToolTip = String.Format("Save {0}", file.Name);
                }
                else
                {
                    presenter.Header = String.Format("Save {0}", file.Name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Save.png");
                    presenter.InputGestureText = "Ctrl+S";
                }

                canSave = true;
            }

            IsVisible = canSave;
            IsEnabled = isDirty;
        }

        /// <summary>
        /// Save the document changes.
        /// </summary>
        public override void Execute()
        {
            if (CurrentDocument != null)
                CurrentDocument.Save();
        }

        #endregion
    }
}
