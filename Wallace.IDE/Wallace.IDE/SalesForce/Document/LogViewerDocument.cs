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
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Log viewer document.
    /// </summary>
    public class LogViewerDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public LogViewerDocument(Project project, LogParameters parameters)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            Project = project;
            View = new LogViewerControl();
            View.SelectedLogChanged += View_SelectedLogChanged;
            Parameters = parameters;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this document is displayed in.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The log parameters that this document is displaying logs for.
        /// </summary>
        public LogParameters Parameters { get; private set; }

        /// <summary>
        /// The view used by the document.
        /// </summary>
        private LogViewerControl View { get; set; }

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
                Presenter.Header = VisualHelper.CreateIconHeader("Logs", "Report.png");
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// If this document represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this document represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return Parameters.Equals(entity);
        }

        /// <summary>
        /// Refresh the logs that are being displayed.
        /// </summary>
        public void Refresh()
        {
            View.TracedEntity = Parameters.TracedEntityName;
            View.Logs = Project.Client.GetLogs(Parameters.TracedEntityId);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the log content displayed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_SelectedLogChanged(object sender, EventArgs e)
        {
            try
            {
                using (App.Wait("Downloading log"))
                {
                    if (View.SelectedLog == null)
                        View.LogContent = String.Empty;
                    else
                        View.LogContent = Project.Client.GetLogContent(View.SelectedLog);
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
