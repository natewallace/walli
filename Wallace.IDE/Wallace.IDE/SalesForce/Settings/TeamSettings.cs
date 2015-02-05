﻿/*
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
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Settings
{
    /// <summary>
    /// Team related settings.
    /// </summary>
    public class TeamSettings : ISettings
    {
        #region Fields

        /// <summary>
        /// The view for these settings.
        /// </summary>
        private TeamSettingsControl _view;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TeamSettings()
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
            _view = new TeamSettingsControl();
            _view.AutoCheckoutFile = (bool)Properties.Settings.Default["AutoCheckoutFile"];
            _view.SelectedDiffAlgorithm = Properties.Settings.Default["DiffAlgorithm"] as string;
        }

        #endregion

        #region ISettings Members

        /// <summary>
        /// Get the path for these settings.
        /// </summary>
        /// <returns>The path for these settings.</returns>
        public string GetPath()
        {
            return "Team";
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
            Properties.Settings.Default["AutoCheckoutFile"] = _view.AutoCheckoutFile;
            Properties.Settings.Default["DiffAlgorithm"] = _view.SelectedDiffAlgorithm;

            CreateView();
        }

        /// <summary>
        /// Cancel any pending changes.
        /// </summary>
        public void Cancel()
        {
            CreateView();
        }

        #endregion
    }
}
