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

using SalesForceData;
using System;
using System.Linq;
using System.Timers;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Displays a test run.
    /// </summary>
    public class TestRunDocument : DocumentBase
    {        
        #region Fields

        /// <summary>
        /// Used to poll the server for updates to the test run.
        /// </summary>
        private Timer _refreshTimer;

        /// <summary>
        /// Set to true after the document has been closed.
        /// </summary>
        private bool _isClosed;

        /// <summary>
        /// Keeps track of the number of update calls made.
        /// </summary>
        private int _updateCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="testRun">TestRun.</param>
        public TestRunDocument(Project project, TestRun testRun)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (testRun == null)
                throw new ArgumentNullException("testRun");

            Project = project;
            TestRun = testRun;
            Text = "Test Run";

            View = new TestRunControl();

            _refreshTimer = new Timer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            _refreshTimer.AutoReset = false;
            _refreshTimer.Elapsed += refreshTimer_Elapsed;

            _isClosed = false;

            View.TitleText = String.Format("Tests started  at {1}.{0} ",
                                           Environment.NewLine,
                                           TestRun.Started);

            View.TestRunItems = TestRun.Items.OrderBy(i => i.Name);
            UpdateTestRun();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this document is displayed in.
        /// </summary>
        private Project Project { get; set; }

        /// <summary>
        /// The test run being displayed.
        /// </summary>
        private TestRun TestRun { get; set; }

        /// <summary>
        /// The view used to display this document.
        /// </summary>
        public TestRunControl View { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the latest status of the test run and update the view.
        /// </summary>
        private void UpdateTestRun()
        {
            if (_isClosed)
                return;

            Project.Client.UpdateTests(TestRun);
            View.UpdateView();
            _updateCount++;

            if (!TestRun.IsDone)
            {
                if (_updateCount == 11)
                    _refreshTimer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
                else if (_updateCount == 21)
                    _refreshTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;

                _refreshTimer.Start();
            }
            else
            {
                View.TitleText = String.Format("Tests started  at {1}.{0}Tests finished at {2}.", 
                                               Environment.NewLine, 
                                               TestRun.Started, 
                                               TestRun.Finished);
            }
        }

        /// <summary>
        /// Set flag to stop the updates.
        /// </summary>
        public override void Closed()
        {
            base.Closed();
            _isClosed = true;
        }

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader("Test Run", "RunTests.png");
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
            if (entity is TestRun)
                return ((entity as TestRun).JobId == TestRun.JobId);
            else
                return false;
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
            App.Instance.Dispatcher.Invoke(UpdateTestRun);
        }

        #endregion
    }
}
