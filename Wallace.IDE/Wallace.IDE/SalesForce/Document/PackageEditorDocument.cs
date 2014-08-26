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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.Framework.UI;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Editor document for packages.
    /// </summary>
    public class PackageEditorDocument : DocumentBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="package">Package.</param>
        public PackageEditorDocument(Project project, Package package)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (package == null)
                throw new ArgumentNullException("package");

            Project = project;
            Package = package;
            Text = Package.Name;

            View = new PackageEditorControl();
            View.ListDragOver += View_DragOver;
            View.ListDrop += View_Drop;

            Files = new UniqueObservableCollection<SourceFile>();
            View.ItemsSource = Files;
            CollectionView cView = CollectionViewSource.GetDefaultView(View.ItemsSource) as CollectionView;
            cView.GroupDescriptions.Add(new PropertyGroupDescription("FileType"));
            cView.SortDescriptions.Add(new SortDescription("FileType", ListSortDirection.Ascending));
            cView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the package belongs to.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The package being edited.
        /// </summary>
        public Package Package { get; private set; }

        /// <summary>
        /// The view for this document.
        /// </summary>
        private PackageEditorControl View { get; set; }

        /// <summary>
        /// Holds the files that are displayed.
        /// </summary>
        private ObservableCollection<SourceFile> Files { get; set; }

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
                Presenter.Header = VisualHelper.CreateIconHeader(Package.Name, "Package.png");
                Presenter.ToolTip = Package.LocalPath;
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
            return (Package.CompareTo(entity) == 0);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Process the drag over event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            if (formats != null && formats.Contains("SalesForceData.SourceFile[]"))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Process the drop event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            if (formats != null && formats.Contains("SalesForceData.SourceFile[]"))
            {
                SalesForceData.SourceFile[] files = e.Data.GetData("SalesForceData.SourceFile[]") as SalesForceData.SourceFile[];
                foreach (SalesForceData.SourceFile file in files)
                    Files.Add(file);
            }
        }

        #endregion
    }
}
