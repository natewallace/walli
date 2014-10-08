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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Function that creates a new checkpoint.
    /// </summary>
    public class NewCheckpointFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// Get the current file.
        /// </summary>
        private SourceFile CurrentFile
        {
            get
            {
                if (App.Instance.Content.ActiveDocument is ClassEditorDocument)
                    return (App.Instance.Content.ActiveDocument as ClassEditorDocument).File;
                else if (App.Instance.Content.ActiveDocument is TriggerEditorDocument)
                    return (App.Instance.Content.ActiveDocument as TriggerEditorDocument).File;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the current line number of the current document or -1 if the current document isn't a text document.
        /// </summary>
        private int CurrentLineNumber
        {
            get
            {
                if (App.Instance.Content.ActiveDocument is ISourceFileEditorDocument)
                    return (App.Instance.Content.ActiveDocument as ISourceFileEditorDocument).CurrentLineNumber;
                else
                    return -1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the headers.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            if (CurrentFile != null)
            {
                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Checkpoint.png");
                    presenter.ToolTip = "Create checkpoint...";
                }
                else
                {
                    presenter.Header = "Create checkpoint...";
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Checkpoint.png");
                }

                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
        }

        /// <summary>
        /// Create a new checkpoint.
        /// </summary>
        public override void Execute()
        {
            SourceFile file = CurrentFile;
            int lineNumber = CurrentLineNumber;
            Project project = App.Instance.SalesForceApp.CurrentProject;

            if (file != null && project != null)
            {
                EditCheckpointWindow dlg = new EditCheckpointWindow();
                dlg.Title = "New Checkpoint";
                dlg.ActionText = "Create";
                dlg.FileName = file.Name;
                dlg.LineNumber = lineNumber.ToString();
                if (App.ShowDialog(dlg))
                {
                    using (App.Wait("Create checkpoint"))
                    {
                        Checkpoint checkpoint = project.Client.CreateCheckpoint(
                            file,
                            lineNumber,
                            dlg.Iteration,
                            dlg.HeapDump,
                            dlg.Script,
                            dlg.ScriptType);
                    }
                }
            }
        }

        #endregion
    }
}
