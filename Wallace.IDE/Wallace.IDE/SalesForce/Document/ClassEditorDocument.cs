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
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Editor document for classes and interfaces.
    /// </summary>
    public class ClassEditorDocument : SourceFileEditorDocumentBase<ApexEditorControl>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the apex on.</param>
        /// <param name="classFile">The apex file that is being edited.</param>
        public ClassEditorDocument(Project project, SourceFile classFile)
            : base(project, classFile)
        {
            View.LanguageManager = project.Language;
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

        #endregion
    }
}
