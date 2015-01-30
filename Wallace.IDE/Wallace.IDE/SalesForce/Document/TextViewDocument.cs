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
    /// Document used to view diffs.
    /// </summary>
    public class TextViewDocument : SourceFileEditorDocumentBase<TextViewControl>
    {
        #region Fields

        /// <summary>
        /// Holds the title for the document.
        /// </summary>
        private string _title;

        /// <summary>
        /// The icon to display.
        /// </summary>
        private string _icon;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="file">File.</param>
        /// <param name="text">Text.</param>
        /// <param name="title">The title to display.</param>
        /// <param name="icon">The icon to display.</param>
        /// <param name="highlightDiffs">If true, the diffs in the text are highlighted.</param>
        public TextViewDocument(Project project, SourceFile file, string text, string title, string icon, bool highlightDiffs)
            : base(project, file)
        {
            View.Text = text;
            View.HighlightDiffs = highlightDiffs;
            _title = title;
            _icon = icon;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the document is for diffs.
        /// </summary>
        public bool IsDiffView
        {
            get { return View.HighlightDiffs; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Do nothing on update.
        /// </summary>
        /// <param name="isFirstUpdate">Ignored.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                Presenter.Header = VisualHelper.CreateIconHeader(_title, _icon);
                Presenter.ToolTip = _title;
                Presenter.Content = View;
            }
        }

        /// <summary>
        /// Do nothing if save is called.
        /// </summary>
        public override void Save()
        {
        }

        /// <summary>
        /// Do nothing if reload is called.
        /// </summary>
        /// <returns>false.</returns>
        public override bool Reload()
        {
            return false;
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="entity">Ignored.</param>
        /// <returns>false.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return false;
        }

        /// <summary>
        /// Navigate to the next diff.
        /// </summary>
        public void GotToNextDiff()
        {
            View.GotToNextDiff();
        }

        /// <summary>
        /// Navigate to the previous diff.
        /// </summary>
        public void GotToPreviousDiff()
        {
            View.GotToPreviousDiff();
        }

        #endregion
    }
}
