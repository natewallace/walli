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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Editor document for classes and interfaces.
    /// </summary>
    public class ClassEditorDocument : SourceFileEditorDocumentBase<ApexEditorControl>
    {
        #region Fields

        /// <summary>
        /// Supports the FoldAllToggle property.
        /// </summary>
        private bool _foldAllToggle;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the apex on.</param>
        /// <param name="classFile">The apex file that is being edited.</param>
        public ClassEditorDocument(Project project, SourceFile classFile)
            : base(project, classFile)
        {
            View.ParseDataChanged += View_ParseDataChanged;
            View.MarginDoubleClick += View_MarginDoubleClick;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if this document is for a test class.
        /// </summary>
        public bool IsTest
        {
            get
            {
                if (View.ParseData == null || View.ParseData.Symbols == null)
                    return false;

                return View.ParseData.Symbols.IsTest;
            }
        }

        /// <summary>
        /// Get/Set the toggle property that folds or unfolds all folding sections.
        /// </summary>
        public bool FoldAllToggle
        {
            get
            {
                return _foldAllToggle;
            }
            set
            {
                _foldAllToggle = value;
                View.FoldAll(_foldAllToggle);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the class icon.
        /// </summary>
        /// <returns>The class icon.</returns>
        protected override string GetIcon()
        {
            return "DocumentClass.png";
        }

        /// <summary>
        /// Set the language manager on the view.
        /// </summary>
        protected override void OnViewCreated()
        {
            View.LanguageManager = Project.Language;
        }

        /// <summary>
        /// Cache symbols when the document gets loaded from the server.
        /// </summary>
        /// <returns>The result of the reload.</returns>
        public override bool Reload()
        {
            bool result = base.Reload();
            if (result && View.ParseData != null)
            {
                Project.Language.UpdateSymbols(View.ParseData.Symbols, true, true);
            }

            return result;
        }

        /// <summary>
        /// Handle name changes.
        /// </summary>
        public override void Save()
        {
            // get node before save
            INode[] nodes = App.Instance.Navigation.GetNodesByEntity(File);
            INode node = null;
            foreach (INode n in nodes)
            {
                if (n is ApexClassNode)
                {
                    node = n;
                    break;
                }
            }

            string oldName = File.Name;

            // save
            base.Save();

            // cache the symbols
            if (View.ParseData != null && View.ParseData.Symbols != null)
                Project.Language.UpdateSymbols(View.ParseData.Symbols, true, true);

            // process name change
            if (File.IsNameUpdated)
            {
                Project.Language.RemoveSymbols(oldName);

                node.Presenter.Remove();
                App.Instance.Navigation.GetNode<ApexClassFolderNode>().AddApexClass(File);
            }
        }

        /// <summary>
        /// Comment or uncomment the currently selected text.
        /// </summary>
        /// <param name="flag">If true the selected text is commented.  If false the selected text is uncommented.</param>
        public void CommentSelectedText(bool flag)
        {
            View.CommentSelectedText(flag);
        }

        /// <summary>
        /// Update the editor settings.
        /// </summary>
        public void UpdateEditorSettings()
        {
            View.ApplyEditorSettings();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the workspace view when the parse data has been changed.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_ParseDataChanged(object sender, System.EventArgs e)
        {
            App.Instance.UpdateWorkspaces();
        }

        /// <summary>
        /// Create a new checkpoint when a user clicks in the margin.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_MarginDoubleClick(object sender, System.EventArgs e)
        {
            App.Instance.GetFunction<NewCheckpointFunction>().Execute();
        }

        #endregion
    }
}
