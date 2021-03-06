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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// A node that represents an apex trigger.
    /// </summary>
    public class ApexTriggerNode : SourceFileNode
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project the trigger belongs to.</param>
        /// <param name="sourceFile">The trigger source file.</param>
        public ApexTriggerNode(Project project, SourceFile sourceFile) :
            base(project, sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException("sourceFile");
            if (sourceFile.FileType.Name != "ApexTrigger")
                throw new ArgumentException("sourceFile", "sourceFile must be of type ApexTrigger");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void UpdateHeader()
        {
            if (SourceFile.CheckedOutBy != null)
                if (SourceFile.CheckedOutBy.Equals(Project.Client.User))
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentTriggerEdit.png");
                else
                    Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentTriggerLock.png");
            else
                Presenter.Header = VisualHelper.CreateIconHeader(SourceFile.Name, "DocumentTrigger.png");
        }

        /// <summary>
        /// Open the apex trigger editor.
        /// </summary>
        public override void DoubleClick()
        {
            if (!OpenExistingEditorDocument())
            {
                using (App.Wait("Opening trigger..."))
                {
                    if (Project.Client.Checkout.IsEnabled())
                    {
                        Project.Client.Checkout.RefreshCheckoutStatus(SourceFile);
                        UpdateHeader();
                    }

                    TriggerEditorDocument triggerDocument = new TriggerEditorDocument(Project, SourceFile);
                    App.Instance.Content.OpenDocument(triggerDocument);
                }
            }
        }

        /// <summary>
        /// Get functions that show up in the nodes context menu.
        /// </summary>
        /// <returns>Functions that show up in the nodes context menu.</returns>
        public override IFunction[] GetContextFunctions()
        {
            return MergeFunctions(
                base.GetContextFunctions(),
                new IFunction[] 
                {
                    App.Instance.GetFunction<DeleteSourceFileFunction>()
                });
        }

        #endregion
    }
}
