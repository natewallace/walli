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

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// This document shows overall code coverage.
    /// </summary>
    public class CodeCoverageDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        public CodeCoverageDocument(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            Project = project;

            View = new CodeCoverageControl();
            RefreshCodeCoverage();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the document is in.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The document view.
        /// </summary>
        public CodeCoverageControl View { get; private set; }

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
                Presenter.Header = VisualHelper.CreateIconHeader("Code Coverage", "CodeCoverage.png");
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Refresh the code coverage displayed.
        /// </summary>
        public void RefreshCodeCoverage()
        {
            View.CodeCoverage = Project.Client.Test.GetCodeCoverage().OrderBy(c => c.File.FullName).ToArray();
            View.OrganizationWideCoveragePercent = Project.Client.Test.GetOverallCodeCoveragePercent();
        }

        #endregion
    }
}
