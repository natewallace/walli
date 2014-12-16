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

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements ISettingsManager using the SettingsWindow control.
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        #region Fields

        /// <summary>
        /// Holds all of the registered settings.
        /// </summary>
        private Dictionary<string, ISettings> _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsManager()
        {
            _settings = new Dictionary<string, ISettings>();
        }

        #endregion

        #region ISettingsManager Members

        /// <summary>
        /// Register the given settings.
        /// </summary>
        /// <param name="settings">The settings to register.</param>
        public void RegisterSettings(ISettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings.Add(settings.GetPath(), settings);
        }

        /// <summary>
        /// Show the manager.
        /// </summary>
        public void ShowManager()
        {
            SettingsWindow window = new SettingsWindow();
            window.SelectedSettingsPathChanged += window_SelectedSettingsPathChanged;

            try
            {
                foreach (string path in _settings.Keys)
                    window.AddSettingsPath(path);

                if (App.ShowDialog(window))
                {
                    foreach (ISettings settings in _settings.Values)
                        settings.Save();
                }
                else
                {
                    foreach (ISettings settings in _settings.Values)
                        settings.Cancel();
                }
            }
            finally
            {
                window.SelectedSettingsPathChanged -= window_SelectedSettingsPathChanged;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update the view.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void window_SelectedSettingsPathChanged(object sender, EventArgs e)
        {
            SettingsWindow window = sender as SettingsWindow;
            if (window != null)
            {
                string path = window.SelectedSettingsPath;
                if (path == null || !_settings.ContainsKey(path))
                    window.ShowSettingsView(null);
                else
                    window.ShowSettingsView(_settings[path].GetView());
            }
        }

        #endregion
    }
}
