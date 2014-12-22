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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Settings
{
    /// <summary>
    /// Settings for the SOQL editor.
    /// </summary>
    public class SOQLEditorSettings : ISettings
    {
        #region Fields

        /// <summary>
        /// The view for these settings.
        /// </summary>
        private EditorSettingsControl _view;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SOQLEditorSettings()
        {
            CreateView();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the view.
        /// </summary>
        private void CreateView()
        {
            _view = new EditorSettingsControl();
            _view.SettingFontFamily = EditorSettings.SOQLSettings.FontFamily;
            _view.SettingFontSize = EditorSettings.SOQLSettings.FontSizeInPoints;
            _view.SettingSymbols = EditorSettings.SOQLSettings.Symbols;
        }

        #endregion

        #region ISettings Members

        /// <summary>
        /// Get the path for these settings.
        /// </summary>
        /// <returns>The path for these settings.</returns>
        public string GetPath()
        {
            return "Font and Color/SOQL";
        }

        /// <summary>
        /// Get the view used for these settings.
        /// </summary>
        /// <returns>The view used for these settings.</returns>
        public System.Windows.Controls.Control GetView()
        {
            return _view;
        }

        /// <summary>
        /// Save any changes made to the settings.
        /// </summary>
        public void Save()
        {
            EditorSettings.SOQLSettings.FontFamily = _view.SettingFontFamily;
            EditorSettings.SOQLSettings.FontSizeInPoints = _view.SettingFontSize;

            EditorSettings.SOQLSettings.Save();

            CreateView();

            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is DataEditorDocument || document is DataEditorDocument)
                    (document as DataEditorDocument).UpdateEditorSettings();
            }
        }

        /// <summary>
        /// Cancel any pending changes.
        /// </summary>
        public void Cancel()
        {
            EditorSettings.SOQLSettings.ResetSymbols();
            CreateView();
        }

        #endregion
    }
}
