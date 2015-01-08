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
using System.Windows.Controls;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Document used to view checkpoints.
    /// </summary>
    public class ViewCheckpointsDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public ViewCheckpointsDocument(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;
            View = new StackPanel();
            View.Margin = new System.Windows.Thickness(5);

            Reload();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this document is displayed in.
        /// </summary>
        private Project Project { get; set; }

        /// <summary>
        /// The view used to display this document.
        /// </summary>
        public StackPanel View { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader("Checkpoints", "Checkpoints.png");
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Load the checkpoints.
        /// </summary>
        public void Reload()
        {
            while (View.Children.Count > 0)
                Remove((View.Children[0] as ViewCheckpointItemControl).Tag as Checkpoint);

            foreach (Checkpoint checkpoint in Project.Client.GetCheckpoints())
                Add(checkpoint);
        }

        /// <summary>
        /// Remove a checkpoint from the view.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to remove.</param>
        private void Remove(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                throw new ArgumentNullException("checkpoint");

            for (int i = 0; i < View.Children.Count; i++)
            {
                ViewCheckpointItemControl item = View.Children[i] as ViewCheckpointItemControl;
                if (checkpoint.Id == (item.Tag as Checkpoint).Id)
                {
                    item.EditClick += item_EditClick;
                    item.DeleteClick += item_DeleteClick;
                    item.ResultsClick += item_ResultsClick;

                    View.Children.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Add a checkpoint to the view.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to add.</param>
        private void Add(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                throw new ArgumentNullException("checkpoint");

            ViewCheckpointItemControl item = new ViewCheckpointItemControl();
            item.FileName = checkpoint.FileName;
            item.LineNumber = checkpoint.LineNumber.ToString();
            item.Tag = checkpoint;

            item.EditClick += item_EditClick;
            item.DeleteClick += item_DeleteClick;
            item.ResultsClick += item_ResultsClick;

            View.Children.Add(item);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Show results for the checkpoint.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void item_ResultsClick(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Delete the checkpoint.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void item_DeleteClick(object sender, EventArgs e)
        {
            if (App.MessageUser(
                "Are you sure you want to delete this checkpoint?",
                "Delete checkpoint",
                System.Windows.MessageBoxImage.Warning,
                new string[] { "Yes", "No" }) == "Yes")
            {
                ViewCheckpointItemControl item = sender as ViewCheckpointItemControl;
                Checkpoint checkpoint = item.Tag as Checkpoint;
                using (App.Wait("Deleting checkpoint"))
                    Project.Client.DeleteCheckpoint(checkpoint);
                Remove(checkpoint);
            }
        }

        /// <summary>
        /// Edit the checkpoint.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void item_EditClick(object sender, EventArgs e)
        {
            ViewCheckpointItemControl item = sender as ViewCheckpointItemControl;
            Checkpoint checkpoint = item.Tag as Checkpoint;

            EditCheckpointWindow dlg = new EditCheckpointWindow();
            dlg.Title = "Edit Checkpoint";
            dlg.ActionText = "Save";
            dlg.FileName = checkpoint.FileName;
            dlg.LineNumber = checkpoint.LineNumber.ToString();
            dlg.Iteration = checkpoint.Iteration;
            dlg.HeapDump = checkpoint.HeapDump;
            dlg.ScriptType = checkpoint.ScriptType;
            dlg.Script = checkpoint.Script;
            if (App.ShowDialog(dlg))
            {
                using (App.Wait("Saving checkpoint"))
                {
                    checkpoint.Iteration = dlg.Iteration;
                    checkpoint.HeapDump = dlg.HeapDump;
                    checkpoint.ScriptType = dlg.ScriptType;
                    checkpoint.Script = dlg.Script;
                    Project.Client.SaveCheckpoint(checkpoint);
                }
            }
        }

        #endregion
    }
}
