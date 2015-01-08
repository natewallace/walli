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
using System.Windows.Controls;
using System.Windows.Media;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Used to display errors.
    /// </summary>
    public class ErrorProvider
    {
        #region Fields

        /// <summary>
        /// Holds previous border colors of controls that are displaying errors.
        /// </summary>
        private Dictionary<Control, Brush> _borderBrushes;

        /// <summary>
        /// Holds previous tool tips of controls that are displaying errors.
        /// </summary>
        private Dictionary<Control, object> _toolTips;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        public ErrorProvider()
        {
            _borderBrushes = new Dictionary<Control, Brush>();
            _toolTips = new Dictionary<Control, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display or clear an error message for a control.
        /// </summary>
        /// <param name="control">The control to set an error message for.</param>
        /// <param name="errorMessage">The error message to display.</param>
        public void SetError(Control control, string errorMessage)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (String.IsNullOrWhiteSpace(errorMessage))
            {
                if (_borderBrushes.ContainsKey(control))
                {
                    control.BorderBrush = _borderBrushes[control];
                    _borderBrushes.Remove(control);
                }

                if (_toolTips.ContainsKey(control))
                {
                    control.ToolTip = _toolTips[control];
                    _toolTips.Remove(control);
                }
            }
            else
            {
                if (!_borderBrushes.ContainsKey(control))
                    _borderBrushes.Add(control, control.BorderBrush);
                control.BorderBrush = Brushes.Red;

                if (!_toolTips.ContainsKey(control))
                    _toolTips.Add(control, control.ToolTip);
                control.ToolTip = errorMessage;
            }
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        public void ClearErrors()
        {
            Control[] controls = _borderBrushes.Keys.ToArray();
            foreach (Control control in controls)
                SetError(control, null);
        }

        #endregion
    }
}
