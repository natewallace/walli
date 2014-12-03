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
using Wallace.IDE.Framework.UI;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Log viewer document.
    /// </summary>
    public class LogViewerDocument : DocumentBase
    {
        #region Fields

        /// <summary>
        /// Used to display a log as nodes in a tree.
        /// </summary>
        private INodeManager _logNodeManager;

        #endregion

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
            _logNodeManager = new TreeViewNodeManager(View.LogContentData);
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

        /// <summary>
        /// The currently selected log.
        /// </summary>
        public Log SelectedLog
        {
            get { return View.SelectedLog; }
        }

        /// <summary>
        /// The logs that are currently displayed.
        /// </summary>
        public IEnumerable<Log> Logs
        {
            get { return View.Logs; }
        }

        /// <summary>
        /// The currently selected log unit if there is one.
        /// </summary>
        public LogUnit SelectedLogUnit
        {
            get
            {
                LogUnitNode node = _logNodeManager.ActiveNode as LogUnitNode;
                if (node == null)
                    return null;

                return node.Unit;
            }
        }

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
            App.Instance.UpdateWorkspaces();
        }

        /// <summary>
        /// Select the text that corresponds to the given unit.
        /// </summary>
        /// <param name="unit">The unit to select the text for.</param>
        public void SelectText(LogUnit unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit");

            View.SelectLine(unit.LineNumber);
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
                    {
                        View.LogContentText = String.Empty;
                        _logNodeManager.Nodes.Clear();
                    }
                    else
                    {
                        View.LogContentText = Project.Client.GetLogContent(View.SelectedLog);

                        LogData data = new LogData(View.LogContentText);
                        _logNodeManager.Nodes.Clear();
                        foreach (LogUnit unit in data.Units)
                            _logNodeManager.Nodes.Add(new LogUnitNode(unit));
                    }

                    App.Instance.UpdateWorkspaces();
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
