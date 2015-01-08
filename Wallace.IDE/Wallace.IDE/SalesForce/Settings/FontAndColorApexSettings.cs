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
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Settings
{
    /// <summary>
    /// Settings for the apex editor.
    /// </summary>
    public class FontAndColorApexSettings : ISettings
    {
        #region Fields

        /// <summary>
        /// The view for these settings.
        /// </summary>
        private FontAndColorSettingsControl _view;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public FontAndColorApexSettings()
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
            if (_view != null)
                _view.ThemeClick -= view_ThemeClick;

            _view = new FontAndColorSettingsControl();
            _view.SettingFontFamily = EditorSettings.ApexSettings.FontFamily;
            _view.SettingFontSize = EditorSettings.ApexSettings.FontSizeInPoints;
            _view.SettingForeground = EditorSettings.ApexSettings.Foreground;
            _view.SettingBackground = EditorSettings.ApexSettings.Background;
            _view.SettingSelectionForeground = EditorSettings.ApexSettings.SelectionForeground;
            _view.SettingSelectionBackground = EditorSettings.ApexSettings.SelectionBackground;
            _view.SettingFindResultBackground = EditorSettings.ApexSettings.FindResultBackground;
            _view.SettingSymbols = EditorSettings.ApexSettings.Symbols;
            _view.ShowThemes = true;
            _view.ThemeClick += view_ThemeClick;
        }

        #endregion

        #region ISettings Members

        /// <summary>
        /// Get the path for these settings.
        /// </summary>
        /// <returns>The path for these settings.</returns>
        public string GetPath()
        {
            return "Font and Color/Apex";
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
            EditorSettings.ApexSettings.FontFamily = _view.SettingFontFamily;
            EditorSettings.ApexSettings.Foreground = _view.SettingForeground;
            EditorSettings.ApexSettings.Background = _view.SettingBackground;
            EditorSettings.ApexSettings.SelectionForeground = _view.SettingSelectionForeground;
            EditorSettings.ApexSettings.SelectionBackground = _view.SettingSelectionBackground;
            EditorSettings.ApexSettings.FindResultBackground = _view.SettingFindResultBackground;
            EditorSettings.ApexSettings.FontSizeInPoints = _view.SettingFontSize;
            EditorSettings.ApexSettings.UpdateSymbols(_view.SettingSymbols);

            EditorSettings.ApexSettings.Save();

            CreateView();

            foreach (IDocument document in App.Instance.Content.OpenDocuments)
            {
                if (document is ClassEditorDocument || document is TriggerEditorDocument)
                    (document as ISourceFileEditorDocument).UpdateEditorSettings();
            }
        }

        /// <summary>
        /// Cancel any pending changes.
        /// </summary>
        public void Cancel()
        {
            EditorSettings.ApexSettings.ResetSymbols();
            CreateView();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Show the theme selections.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void view_ThemeClick(object sender, EventArgs e)
        {
            SelectValueWindow dlg = new SelectValueWindow();
            dlg.Title = "Select a theme";
            dlg.InputLabel = "Themes:";
            foreach (EditorSettingsTheme theme in EditorSettings.ApexSettings.Themes)
                dlg.Items.Add(theme);

            if (App.ShowDialog(dlg))
            {
                EditorSettingsTheme theme = dlg.SelectedValue as EditorSettingsTheme;
                if (theme != null)
                {
                    _view.SettingFontFamily = theme.FontFamily;
                    _view.SettingForeground = theme.Foreground;
                    _view.SettingBackground = theme.Background;
                    _view.SettingSelectionForeground = theme.SelectionForeground;
                    _view.SettingSelectionBackground = theme.SelectionBackground;
                    _view.SettingFindResultBackground = theme.FindResultBackground;
                    _view.SettingSymbols = theme.Symbols;
                }
            }
        }

        #endregion
    }
}
