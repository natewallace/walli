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
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements IFunctionManager using a ToolBar.
    /// </summary>
    public class ToolBarFunctionManager : HostBase<ToolBar>, IFunctionManager
    {
        #region Fields

        /// <summary>
        /// Handler for button clicks.
        /// </summary>
        private RoutedEventHandler _buttonClickHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ToolBarFunctionManager()
        {
            _buttonClickHandler = new RoutedEventHandler(Button_Click);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        public ToolBarFunctionManager(ToolBar host)
            : this()
        {
            Host = host;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove event handlers.
        /// </summary>
        protected override void BeforeHostSet()
        {
            base.BeforeHostSet();

            if (Host != null)
                Host.RemoveHandler(Button.ClickEvent, _buttonClickHandler);
        }

        /// <summary>
        /// Setup event handlers.
        /// </summary>
        protected override void AfterHostSet()
        {
            base.AfterHostSet();

            if (Host != null)
                Host.AddHandler(Button.ClickEvent, _buttonClickHandler);
        }

        /// <summary>
        /// Gets the display element for the given function.
        /// </summary>
        /// <param name="function">The function to get the display element for.</param>
        /// <returns>The display element for the given function or null if it isn't found.</returns>
        private FrameworkElement GetElement(IFunction function)
        {
            EnsureHost();
            if (function == null)
                throw new ArgumentNullException("function");

            foreach (object item in Host.Items)
            {
                if (item is FrameworkElement)
                {
                    FrameworkElement element = item as FrameworkElement;
                    if (element.Tag is IFunction && (element.Tag as IFunction).Id == function.Id)
                        return element;
                }
            }

            return null;
        }

        /// <summary>
        /// Create and configure a display element to display for the given function.
        /// </summary>
        /// <param name="function">The function to build up a display element for.</param>
        /// <returns>The newly built display element.</returns>
        private FrameworkElement BuildUpElement(IFunction function)
        {
            FrameworkElement element = null;

            if (function is FunctionSeparator)
            {
                Separator separator = new Separator();
                separator.Tag = function;
                separator.Visibility = function.IsVisible ? Visibility.Visible : Visibility.Collapsed;
                separator.IsEnabled = function.IsEnabled;

                element = separator;
            }
            else
            {
                Button button = new Button();
                button.Tag = function;
                ButtonFunctionPresenter presenter = new ButtonFunctionPresenter(button);
                function.Init(FunctionHost.Toolbar, presenter);
                function.Update(FunctionHost.Toolbar, presenter);
                presenter.IsVisible = function.IsVisible;
                presenter.IsEnabled = function.IsEnabled;

                element = button;
            }

            function.IsEnabledChanged += function_IsEnabledChanged;
            function.IsVisibleChanged += function_IsVisibleChanged;

            return element;
        }

        /// <summary>
        /// Tear down a function that was previously built up.
        /// </summary>
        /// <param name="function">The function to tear down.</param>
        private void TearDownElement(IFunction function)
        {
            function.IsEnabledChanged -= function_IsEnabledChanged;
            function.IsVisibleChanged -= function_IsVisibleChanged;
        }

        #endregion

        #region IFunctionManager Members

        /// <summary>
        /// Add the given function to the root of the control.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(IFunction function)
        {
            AddFunction(function, int.MaxValue);
        }

        /// <summary>
        /// Add the given function to the root of the control.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="position">The position where the function will be inserted.</param>
        public void AddFunction(IFunction function, int position)
        {
            EnsureHost();

            if (function == null)
                throw new ArgumentException("function");

            if (GetElement(function) != null)
                return;

            if (position < 0)
                position = 0;
            if (position > Host.Items.Count)
                Host.Items.Add(BuildUpElement(function));
            else
                Host.Items.Insert(position, BuildUpElement(function));
        }

        /// <summary>
        /// Add the given function to the sub items of the function with the given id.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="parentId">The id of the parent to add the function to.</param>
        public void AddFunction(IFunction function, string parentId)
        {
            AddFunction(function, parentId, int.MaxValue);
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="parentId">The id of the parent to add the function to.</param>
        /// <param name="position">The position where the function will be inserted.</param>
        public void AddFunction(IFunction function, string parentId, int position)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove the function from the control.
        /// </summary>
        /// <param name="function">The function to remove.</param>
        public void RemoveFunction(IFunction function)
        {
            EnsureHost();

            FrameworkElement element = GetElement(function);
            if (element != null)
            {
                ItemsControl parent = element.Parent as ItemsControl;
                if (parent != null)
                    parent.Items.Remove(element);

                TearDownElement(function);
            }
        }

        /// <summary>
        /// Update all functions.
        /// </summary>
        public void UpdateFunctions()
        {
            EnsureHost();

            foreach (FrameworkElement item in Host.Items)
            {
                if (item.Tag is IFunction && item is Button)
                    (item.Tag as IFunction).Update(FunctionHost.Toolbar, new ButtonFunctionPresenter(item as Button));
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Execute function that was clicked.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = VisualHelper.GetAncestor<Button>(e.OriginalSource as DependencyObject);
                if (button != null && button.Tag is IFunction)
                    (button.Tag as IFunction).Execute();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the visiblity of the function.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void function_IsVisibleChanged(object sender, EventArgs e)
        {
            try
            {
                IFunction function = sender as IFunction;
                FrameworkElement element = GetElement(function);
                if (element != null)
                    element.Visibility = function.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the enabled state of the function.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void function_IsEnabledChanged(object sender, EventArgs e)
        {
            try
            {
                IFunction function = sender as IFunction;
                Button button = GetElement(function) as Button;
                if (button != null)
                    new ButtonFunctionPresenter(button).IsEnabled = function.IsEnabled;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
