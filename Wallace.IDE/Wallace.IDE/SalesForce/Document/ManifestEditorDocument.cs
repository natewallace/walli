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
    public class ManifestEditorDocument : DocumentBase
    {
        #region Fields

        /// <summary>
        /// Supports the IsDirty property.
        /// </summary>
        private bool _isDirty;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="manifest">Manifest.</param>
        public ManifestEditorDocument(Project project, Manifest manifest)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            Project = project;
            Manifest = manifest;
            Text = manifest.Name;
            _isDirty = false;

            View = new ManifestEditorControl();
            View.ListDragOver += View_DragOver;
            View.ListDrop += View_Drop;
            View.RemoveItemsClicked += View_RemoveItemsClicked;

            Files = new UniqueObservableCollection<SourceFile>();
            foreach (ManifestItemGroup group in Manifest.Groups)
                foreach (ManifestItem item in group.Items)
                    Files.Add(new SourceFile(group.Name, item.Name));

            View.ItemsSource = Files;
            CollectionView cView = CollectionViewSource.GetDefaultView(View.ItemsSource) as CollectionView;
            cView.GroupDescriptions.Add(new PropertyGroupDescription("FileType"));
            cView.SortDescriptions.Add(new SortDescription("FileType", ListSortDirection.Ascending));
            cView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project the manifest belongs to.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The manifest being edited.
        /// </summary>
        public Manifest Manifest { get; private set; }

        /// <summary>
        /// The view for this document.
        /// </summary>
        private ManifestEditorControl View { get; set; }

        /// <summary>
        /// Holds the files that are displayed.
        /// </summary>
        private ObservableCollection<SourceFile> Files { get; set; }

        /// <summary>
        /// Flag that indicates if there are unsaved changes.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    SetHeader();
                    App.Instance.UpdateWorkspaces();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header based on the current state.
        /// </summary>
        private void SetHeader()
        {
            if (Presenter != null)
            {
                Presenter.Header = _isDirty ? VisualHelper.CreateIconHeader(Manifest.Name, "Manifest.png", "*") :
                                              VisualHelper.CreateIconHeader(Manifest.Name, "Manifest.png");
                Presenter.ToolTip = Manifest.FileName;
            }
        }

        /// <summary>
        /// Merge in files from the given manifest.
        /// </summary>
        /// <param name="manifest">The manifest to merge with this one.</param>
        public void Merge(Manifest manifest)
        {
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            foreach (ManifestItemGroup group in manifest.Groups)
                foreach (ManifestItem item in group.Items)
                    AddFile(new SourceFile(group.Name, item.Name));
        }

        /// <summary>
        /// Add the given file to the manifest.
        /// </summary>
        /// <param name="file">The file to add.</param>
        public void AddFile(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            Files.Add(file);
            IsDirty = true;
        }

        /// <summary>
        /// Save the manifest to file.
        /// </summary>
        public void Save()
        {
            Manifest.Clear();
            foreach (SourceFile file in Files)
                Manifest.AddItem(file);

            Manifest.Save();
            IsDirty = false;
        }

        /// <summary>
        /// Set the title and view.
        /// </summary>
        /// <param name="isFirstUpdate">true if this is the first update.</param>
        public override void Update(bool isFirstUpdate)
        {
            if (isFirstUpdate)
            {
                SetHeader();
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
            return Manifest.Equals(entity);
        }

        /// <summary>
        /// Display warning if the document has unsaved changes.
        /// </summary>
        /// <returns>true if its ok to proceed with the closing.</returns>
        public override bool Closing()
        {
            if (IsDirty)
            {
                return (App.MessageUser(
                    "You have unsaved changes which will be lost.  Do you want to proceed?",
                    "Data Loss",
                    System.Windows.MessageBoxImage.Warning,
                    new string[] { "Yes", "No" }) == "Yes");
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Remove currently selected items.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_RemoveItemsClicked(object sender, EventArgs e)
        {
            object[] items = View.SelectedItems;
            foreach (object item in items)
                Files.Remove(item as SourceFile);

            if (items.Length > 0)
                IsDirty = true;
        }

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

            IsDirty = true;
        }

        #endregion
    }
}
