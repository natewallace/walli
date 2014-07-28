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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements IFunctionManager using a Menu.
    /// </summary>
    public class MenuFunctionManager : HostBase<MenuBase>, IFunctionManager
    {
        #region Fields

        /// <summary>
        /// Handler for menu item clicks.
        /// </summary>
        private RoutedEventHandler _menuItemClickHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuFunctionManager()
        {
            _menuItemClickHandler = new RoutedEventHandler(MenuItem_Click);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        public MenuFunctionManager(MenuBase host)
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
                Host.RemoveHandler(MenuItem.ClickEvent, _menuItemClickHandler);
        }

        /// <summary>
        /// Setup event handlers.
        /// </summary>
        protected override void AfterHostSet()
        {
            base.AfterHostSet();

            if (Host != null)
                Host.AddHandler(MenuItem.ClickEvent, _menuItemClickHandler);
        }

        /// <summary>
        /// Get all of the display elements that are in this manager.
        /// </summary>
        /// <param name="element">The element to start from or null to start from the root.</param>
        /// <param name="items">
        /// This list will be populated with all of the display elements that are in this manager.
        /// </param>
        private void GetAllElements(FrameworkElement element, List<FrameworkElement> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (element == null)
            {
                foreach (FrameworkElement mi in Host.Items)
                {
                    items.Add(mi);
                    GetAllElements(mi, items);
                }
            }
            else
            {
                if (element is MenuItem)
                {
                    foreach (object child in (element as MenuItem).Items)
                    {
                        items.Add(child as FrameworkElement);
                        GetAllElements(child as FrameworkElement, items);
                    }
                }
            }
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

            return GetElement(function.Id, Host.Items);
        }

        /// <summary>
        /// Gets the display element for the given function.
        /// </summary>
        /// <param name="functionId">The id of the function to get the element for.</param>
        /// <returns>The element for the given function or null if it isn't found.</returns>
        private FrameworkElement GetElement(string functionId)
        {
            EnsureHost();
            return GetElement(functionId, Host.Items);
        }

        /// <summary>
        /// Gets the display element for the given function.
        /// </summary>
        /// <param name="functionId">The id of the function to get the element for.</param>
        /// <param name="items">The items to look through.</param>
        /// <returns>The element for the given function or null if it isn't found.</returns>
        private FrameworkElement GetElement(string functionId, ItemCollection items)
        {
            if (functionId == null)
                return null;

            if (items == null)
                return null;

            foreach (object item in items)
            {
                FrameworkElement element = item as FrameworkElement;
                if (element != null)
                {
                    if (element.Tag is IFunction && (element.Tag as IFunction).Id == functionId)
                        return element;

                    if (element is MenuItem)
                    {
                        element = GetElement(functionId, (element as MenuItem).Items);
                        if (element != null)
                            return element;
                    }
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
                MenuItem menuItem = new MenuItem();
                menuItem.Tag = function;
                MenuItemFunctionPresenter presenter = new MenuItemFunctionPresenter(menuItem);
                function.Init(FunctionHost.Menu, presenter);
                function.Update(FunctionHost.Menu, presenter);
                presenter.IsVisible = function.IsVisible;
                presenter.IsEnabled = function.IsEnabled;

                element = menuItem;
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

        /// <summary>
        /// Update the visibility of function grouping menu items based on if they have child items or not.
        /// </summary>
        private void UpdateFunctionGroupingDisplays()
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            GetAllElements(null, elements);
            foreach (FrameworkElement element in elements)
            {
                if (element is MenuItem && element.Tag is FunctionGrouping)
                {
                    bool isVisible = false;
                    foreach (object item in (element as MenuItem).Items)
                    {
                        if (item is FrameworkElement && (item as FrameworkElement).Visibility == Visibility.Visible)
                        {
                            isVisible = true;
                            break;
                        }
                    }

                    element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
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

            UpdateFunctionGroupingDisplays();
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
        /// Add the given function to the sub items of the function with the given id.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="parentId">The id of the parent to add the function to.</param>
        /// <param name="position">The position where the function will be inserted.</param>
        public void AddFunction(IFunction function, string parentId, int position)
        {
            EnsureHost();

            if (function == null)
                throw new ArgumentNullException("function");
            if (parentId == null)
                throw new ArgumentNullException("parentId");

            if (GetElement(function) != null)
                return;

            MenuItem menuItem = GetElement(parentId) as MenuItem;
            if (menuItem == null)
                return;

            if (position < 0)
                position = 0;
            if (position > menuItem.Items.Count)
                menuItem.Items.Add(BuildUpElement(function));
            else
                menuItem.Items.Insert(position, BuildUpElement(function));

            UpdateFunctionGroupingDisplays();
        }

        /// <summary>
        /// Remove the function from the control.
        /// </summary>
        /// <param name="function">The function to remove.</param>
        public void RemoveFunction(IFunction function)
        {
            EnsureHost();

            FrameworkElement menuItem = GetElement(function);
            if (menuItem != null)
            {
                ItemsControl parent = menuItem.Parent as ItemsControl;
                if (parent != null)
                    parent.Items.Remove(menuItem);

                TearDownElement(function);

                UpdateFunctionGroupingDisplays();
            }
        }

        /// <summary>
        /// Update all functions.
        /// </summary>
        public void UpdateFunctions()
        {
            EnsureHost();

            List<FrameworkElement> items = new List<FrameworkElement>();
            GetAllElements(null, items);
            foreach (FrameworkElement item in items)
            {
                if (item is MenuItem && item.Tag is IFunction)
                    (item.Tag as IFunction).Update( 
                        FunctionHost.Menu, 
                        new MenuItemFunctionPresenter(item as MenuItem));
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Execute function that was clicked.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = VisualHelper.GetAncestor<MenuItem>(e.OriginalSource as DependencyObject);
                if (menuItem != null && menuItem.Tag is IFunction && (menuItem.Tag as IFunction).IsEnabled)
                    (menuItem.Tag as IFunction).Execute();
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
                FrameworkElement element = GetElement(function) as FrameworkElement;
                if (element != null)
                    element.Visibility = function.IsVisible ? Visibility.Visible : Visibility.Collapsed;

                UpdateFunctionGroupingDisplays();
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
                MenuItem menuItem = GetElement(function) as MenuItem;
                if (menuItem != null)
                    new MenuItemFunctionPresenter(menuItem).IsEnabled = function.IsEnabled;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
