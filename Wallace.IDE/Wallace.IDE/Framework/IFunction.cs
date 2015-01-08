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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// A function that can be executed from either a menu or toolbar.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Uniquely identifies this function.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets/Sets if the function is visible.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets/Sets if the function is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Called by the UI when the function is ready to be displayed in a host.
        /// This method is called once for each host the function is displayed in.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        void Init(FunctionHost host, IFunctionPresenter presenter);

        /// <summary>
        /// Update the view with current values.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        void Update(FunctionHost host, IFunctionPresenter presenter);

        /// <summary>
        /// Called by the UI when the user executes this function.
        /// </summary>
        void Execute();

        /// <summary>
        /// Raised when the IsEnabled property is changed.
        /// </summary>
        event EventHandler IsEnabledChanged;

        /// <summary>
        /// Raised when the IsVisible property is changed.
        /// </summary>
        event EventHandler IsVisibleChanged;
    }
}
