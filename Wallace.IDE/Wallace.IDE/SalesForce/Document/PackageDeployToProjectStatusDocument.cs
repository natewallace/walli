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
using System.Timers;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Display the status of a package deployment.
    /// </summary>
    public class PackageDeployToProjectStatusDocument : DocumentBase
    {
        #region Fields

        /// <summary>
        /// Used to poll the server for updates to the deployment status.
        /// </summary>
        private Timer _refreshTimer;

        /// <summary>
        /// Keeps track of the number of update calls made.
        /// </summary>
        private int _updateCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="packageName">PackageName.</param>
        /// <param name="target">Target.</param>
        /// <param name="isCheckOnly">IsCheckOnly.</param>
        /// <param name="isRunAlltests">IsRunAllTests.</param>
        public PackageDeployToProjectStatusDocument(
            Package package,
            string target,
            bool isCheckOnly,
            bool isRunAlltests)
        {
            if (package == null)
                throw new ArgumentNullException("package");
            if (String.IsNullOrWhiteSpace(target))
                throw new ArgumentException("target is null or whitespace.", "target");

            Package = package;
            Project = Project.OpenProject(target);

            DeployId = Project.Client.Meta.DeployPackage(package, isCheckOnly, isRunAlltests);
            IsCheckOnly = isCheckOnly;
            IsRunAllTests = isRunAlltests;
            Text = "Deployment";

            _refreshTimer = new Timer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            _refreshTimer.AutoReset = false;
            _refreshTimer.Elapsed += refreshTimer_Elapsed;
            _updateCount = 0;

            View = new PackageDeployStatusControl();

            IsDeploymentRunning = true;
            UpdateStatus(Project.Client.Meta.CheckPackageDeploy(DeployId));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the package is being deployed to.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The package that is being deployed.
        /// </summary>
        public Package Package { get; private set; }

        /// <summary>
        /// Set to true if the deployment is a check only.
        /// </summary>
        public bool IsCheckOnly { get; private set; }

        /// <summary>
        /// Set to true when all tests will be run as part of the deployment.
        /// </summary>
        public bool IsRunAllTests { get; private set; }

        /// <summary>
        /// The id of the deployment.
        /// </summary>
        public string DeployId { get; private set; }

        /// <summary>
        /// The view for this document.
        /// </summary>
        private PackageDeployStatusControl View { get; set; }

        /// <summary>
        /// The package deployment results as text.
        /// </summary>
        public string PackageDeployResultText
        {
            get { return View.ResultText; }
        }

        /// <summary>
        /// Flag that indicates if a deployment is running.
        /// </summary>
        public bool IsDeploymentRunning { get; private set; }

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
                Presenter.Header = VisualHelper.CreateIconHeader(Package.Name, "DeployPackage.png");
                Presenter.ToolTip = "Deployment Status";
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
            return false;
        }

        /// <summary>
        /// Warn user about closing document when a deployment is still running.
        /// </summary>
        /// <returns>true if it's ok to close this document, false if it isn't.</returns>
        public override bool Closing()
        {
            if (IsDeploymentRunning)
            {
                return (App.MessageUser("A deployment is still running.  Are you sure you want to close this document?",
                                        "Close",
                                        System.Windows.MessageBoxImage.Warning,
                                        new string[] { "Yes", "No" }) == "Yes");
            }

            return true;
        }

        /// <summary>
        /// Dispose of the project.
        /// </summary>
        public override void Closed()
        {
            Project.Dispose();
            Project = null;
        }

        /// <summary>
        /// Cancel the deployment.
        /// </summary>
        public void CancelDeployment()
        {
            if (IsDeploymentRunning)
            {
                Project.Client.Meta.CancelPackageDeploy(DeployId);
                IsDeploymentRunning = false;
                View.StatusText = "Canceling";
                App.Instance.UpdateWorkspaces();
            }
        }

        /// <summary>
        /// Update the status and set timer for next update.
        /// </summary>
        private void UpdateStatus(PackageDeployResult result)
        {
            // deployment section
            View.StatusText = result.Status.ToString();
            View.PackageText = Package.Name;
            View.TargetText = Project.ProjectName;
            View.NotesText = IsCheckOnly ? "This is a check only deployment" : null;

            // components section
            if (result.ComponentsTotalCount == 0)
            {
                View.ComponentProgressMaximum = 1;
                View.ComponentProgressValue = 0;
                View.ComponentProgressMessage = "Pending";
                View.ComponentProgressForeground = System.Windows.Media.Brushes.Green;
                View.ComponentProgressBackground = System.Windows.Media.Brushes.White;
            }
            else
            {
                // in progress
                if (!result.DeploymentComplete && result.ComponentsPassCount + result.ComponentsFailCount < result.ComponentsTotalCount)
                {
                    View.ComponentProgressMaximum = result.ComponentsTotalCount;
                    View.ComponentProgressValue = result.ComponentsPassCount + result.ComponentsFailCount;

                    View.ComponentProgressBackground = System.Windows.Media.Brushes.White;
                    if (result.ComponentsFailCount > 0)
                        View.ComponentProgressForeground = System.Windows.Media.Brushes.Red;
                    else
                        View.ComponentProgressForeground = System.Windows.Media.Brushes.Green;
                }
                // complete
                else
                {
                    View.ComponentProgressMaximum = 1;
                    View.ComponentProgressValue = 0;

                    if (result.ComponentsFailCount > 0)
                    {
                        View.ComponentProgressForeground = System.Windows.Media.Brushes.Red;
                        View.ComponentProgressBackground = System.Windows.Media.Brushes.Red;
                    }
                    else if (result.ComponentsPassCount > 0)
                    {
                        View.ComponentProgressForeground = System.Windows.Media.Brushes.Green;
                        View.ComponentProgressBackground = System.Windows.Media.Brushes.Green;
                    }
                }

                View.ComponentProgressMessage = String.Format("{0} of {1} components deployed with {2} failure(s).",
                    result.ComponentsPassCount,
                    result.ComponentsTotalCount,
                    result.ComponentsFailCount);
            }

            // tests section
            if (!IsRunAllTests)
            {
                View.IsTestSectionVisible = false;
            }
            else
            {
                View.IsTestSectionVisible = true;

                if (result.TestsPassCount == 0)
                {
                    View.TestProgressMaximum = 1;
                    View.TestProgressValue = 0;
                    View.TestProgressMessage = "Pending";
                    View.TestProgressForeground = System.Windows.Media.Brushes.Green;
                    View.TestProgressBackground = System.Windows.Media.Brushes.White;
                }
                else
                {
                    // in progress
                    if (!result.DeploymentComplete && result.TestsPassCount + result.TestsFailCount < result.TestsTotalCount)
                    {
                        View.TestProgressMaximum = result.TestsTotalCount;
                        View.TestProgressValue = result.TestsPassCount + result.TestsFailCount;

                        View.TestProgressBackground = System.Windows.Media.Brushes.White;
                        if (result.TestsFailCount > 0)
                            View.TestProgressForeground = System.Windows.Media.Brushes.Red;
                        else
                            View.TestProgressForeground = System.Windows.Media.Brushes.Green;
                    }
                    // complete
                    else
                    {
                        View.TestProgressMaximum = 1;
                        View.TestProgressValue = 0;

                        if (result.TestsFailCount > 0)
                        {
                            View.TestProgressForeground = System.Windows.Media.Brushes.Red;
                            View.TestProgressBackground = System.Windows.Media.Brushes.Red;
                        }
                        else if (result.TestsPassCount > 0)
                        {
                            View.TestProgressForeground = System.Windows.Media.Brushes.Green;
                            View.TestProgressBackground = System.Windows.Media.Brushes.Green;
                        }
                    }

                    View.TestProgressMessage = String.Format("{0} of {1} tests passed with {2} failure(s).",
                        result.TestsPassCount,
                        result.TestsTotalCount,
                        result.TestsFailCount);
                }
            }

            View.ResultText = result.ResultMessage;

            _updateCount++;
            if (!result.DeploymentComplete)
            {
                if (_updateCount == 11)
                    _refreshTimer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
                else if (_updateCount == 21)
                    _refreshTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;

                _refreshTimer.Start();
            }
            else
            {
                IsDeploymentRunning = false;
                App.Instance.UpdateWorkspaces();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the test run details.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Project != null)
            {
                PackageDeployResult result = Project.Client.Meta.CheckPackageDeploy(DeployId);
                App.Instance.Dispatcher.Invoke(new Action(() => UpdateStatus(result)));
            }
        }

        #endregion
    }
}
