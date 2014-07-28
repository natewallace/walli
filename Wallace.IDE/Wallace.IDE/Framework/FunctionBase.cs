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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Implements IFunction using default logic.
    /// </summary>
    public class FunctionBase : IFunction
    {
        #region Fields

        /// <summary>
        /// Supports Id property.
        /// </summary>
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// Supports the IsEnabled property.
        /// </summary>
        private bool _isEnabled = true;

        /// <summary>
        /// Supports the IsVisible property.
        /// </summary>
        private bool _isVisible = true;

        #endregion

        #region Methods

        /// <summary>
        /// Raises the IsEnabledChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnIsEnabledChanged(EventArgs e)
        {
            if (IsEnabledChanged != null)
                IsEnabledChanged(this, e);
        }

        /// <summary>
        /// Raises the IsVisibleChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnIsVisibleChanged(EventArgs e)
        {
            if (IsVisibleChanged != null)
                IsVisibleChanged(this, e);
        }

        #endregion

        #region IFunction Members

        /// <summary>
        /// Id that uniquely identifies this function.
        /// </summary>
        public virtual string Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// Gets/Sets if the function is visible.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnIsEnabledChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets/Sets if the function is visible.
        /// </summary>
        public bool IsVisible 
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnIsVisibleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called by the UI when the function is ready to be displayed in a host.
        /// This method is called once for each host the function is displayed in.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public virtual void Init(FunctionHost host, IFunctionPresenter presenter)
        {
        }

        /// <summary>
        /// Update the menu item with current values.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public virtual void Update(FunctionHost host, IFunctionPresenter presenter)
        {
        }

        /// <summary>
        /// Called by the UI when the user executes this function.
        /// </summary>
        public virtual void Execute()
        {
        }

        /// <summary>
        /// Raised when the IsEnabled property is changed.
        /// </summary>
        public event EventHandler IsEnabledChanged;

        /// <summary>
        /// Raised when the IsVisible property is changed.
        /// </summary>
        public event EventHandler IsVisibleChanged;

        #endregion
    }
}
