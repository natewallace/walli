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
using System.Threading.Tasks;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Document for generic source file edits.
    /// </summary>
    public class SourceFileEditorDocument : SourceFileEditorDocumentBase<SourceFileEditorControl>
    {
        #region Fields

        /// <summary>
        /// Supports Data property.
        /// </summary>
        private SourceFileData _data;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">The project to edit the apex on.</param>
        /// <param name="file">The file that is being edited.</param>
        public SourceFileEditorDocument(Project project, SourceFile file)
            : base(project, file)
        {
            View.ViewChanged += View_ViewChanged;
        }

        #endregion

        #region Property

        /// <summary>
        /// The data for this object if there is any.
        /// </summary>
        public SourceFileData Data
        {
            get 
            {
                if (View.DataView is ProfileEditorControl)
                    (View.DataView as ProfileEditorControl).CommitChanges();

                return _data; 
            }
            set
            {
                _data = value;
                if (_data is ProfileData)
                    View.DataView = new ProfileEditorControl(_data as ProfileData);
                else if (_data == null)
                {
                    if (View.DataView is ProfileEditorControl)
                        (View.DataView as ProfileEditorControl).Profile = null;

                    View.DataView = null;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load data if there is any and it's supported.
        /// </summary>
        /// <returns>The reload result.</returns>
        public override bool Reload()
        {
            // start loading data right away
            Task<SourceFileData> dataTask = null;
            switch (File.FileType.Name)
            {
                case "Profile":
                    dataTask = Task.Run<SourceFileData>(() => Project.Client.Meta.GetSourceFileData(File));
                    break;

                default:
                    break;
            }

            // do load of source
            bool proceed = base.Reload();

            if (proceed)
            {
                using (App.Wait("Refreshing data"))
                {
                    // wait for data to load
                    if (dataTask != null)
                    {
                        dataTask.Wait(System.TimeSpan.FromMinutes(1));
                        if (dataTask.Exception != null)
                            throw new Exception(dataTask.Exception.Message, dataTask.Exception);

                        Data = dataTask.Result;
                        View.IsTabStripVisible = true;
                        View.IsDataVisible = true;
                    }
                    else
                    {
                        View.IsTabStripVisible = false;
                        View.IsSourceVisible = true;
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Save the data object.
        /// </summary>
        public void SaveData()
        {
            if (Data != null)
            {
                using (App.Wait("Saving data"))
                    Project.Client.Meta.SaveSourceFileData(Data);
            }
        }

        /// <summary>
        /// Export the data to the given file.
        /// </summary>
        /// <param name="file">The file to export the data to.</param>
        public void ExportData(string file)
        {
            if (Data == null)
                throw new Exception("There is no data to export.");

            using (System.IO.Stream stream = System.IO.File.Open(file, System.IO.FileMode.Create))
                Data.WriteToStream(stream);
        }

        /// <summary>
        /// Import data from the given file.
        /// </summary>
        /// <param name="file">The file to import the data from.</param>
        public void ImportData(string file)
        {
            if (Data == null)
                throw new Exception("There is no data to import to.");

            SourceFileData data = Data;
            Data = null;

            using (System.IO.Stream stream = System.IO.File.Open(file, System.IO.FileMode.Open))
                data.ReadFromStream(stream);

            Data = data;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the document IsTextVisible property.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void View_ViewChanged(object sender, EventArgs e)
        {
            if (IsTextVisible != View.IsSourceVisible)
            {
                IsTextVisible = View.IsSourceVisible;
                App.Instance.UpdateWorkspaces();
            }
        }

        #endregion
    }
}
