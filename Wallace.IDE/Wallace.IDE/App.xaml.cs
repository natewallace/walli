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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.UI;
using Wallace.IDE.Framework;
using Wallace.IDE.Framework.Function;
using Wallace.IDE.Framework.UI;
using System.Windows.Threading;

namespace Wallace.IDE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        /// <summary>
        /// Holds the main window for the application.
        /// </summary>
        private MainWindow _window;

        /// <summary>
        /// Holds registered functions.
        /// </summary>
        private Dictionary<Type, IFunction> _functions;

        #endregion

        #region Properties

        /// <summary>
        /// Holds the one instance of this application.
        /// </summary>
        public static App Instance { get; private set; }

        /// <summary>
        /// Text to display for the current session.
        /// </summary>
        public string SessionTitle
        {
            get
            {
                return _window.SessionTitle;
            }
            set
            {
                _window.SessionTitle = value;
            }
        }

        /// <summary>
        /// The menu that displays functions.
        /// </summary>
        public IFunctionManager Menu { get; private set; }

        /// <summary>
        /// The toolbar that displays functions.
        /// </summary>
        public IFunctionManager ToolBar { get; private set; }

        /// <summary>
        /// The documents workspace.
        /// </summary>
        public IDocumentManager Content { get; private set; }

        /// <summary>
        /// The nodes workspace.
        /// </summary>
        public INodeManager Navigation { get; private set; }

        /// <summary>
        /// The salesforce application.
        /// </summary>
        public SalesForceApplication SalesForceApp { get; private set; }

        /// <summary>
        /// The settings manager for the app.
        /// </summary>
        public ISettingsManager Settings { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Launch the main window.
        /// </summary>
        /// <param name="e">Arguments passed with the event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Instance = this;
            _window = new MainWindow();
            _window.Closing += window_Closing;

            _window.WindowState = Wallace.IDE.Properties.Settings.Default.LastWindowState;

            _functions = new Dictionary<Type, IFunction>();

            Menu = new MenuFunctionManager(_window.MainMenu);
            ToolBar = new ToolBarFunctionManager(_window.MainToolBar);
            Navigation = new TabTreeNodeManager(_window.Nodes);
            Content = new TabControlDocumentManager(_window.Documents);
            Settings = new SettingsManager();
            SalesForceApp = new SalesForceApplication();

            Navigation.ActiveNodeChanged += Navigation_ActiveNodeChanged;
            Content.ActiveDocumentChanged += Content_ActiveDocumentChanged;

            InitializeFunctions();

            _window.Show();
        }

        /// <summary>
        /// Issue Update to all workspaces.
        /// </summary>
        public void UpdateWorkspaces()
        {
            App.Instance.Menu.UpdateFunctions();
            App.Instance.ToolBar.UpdateFunctions();
            App.Instance.Navigation.UpdateNodes();
        }

        /// <summary>
        /// Initialize functions for the application.
        /// </summary>
        private void InitializeFunctions()
        {
            Menu.AddFunction(new FunctionGrouping("SYSTEM", "SYSTEM", false));            
            Menu.AddFunction(new SettingsFunction(), "SYSTEM");
            Menu.AddFunction(new FunctionSeparator(), "SYSTEM");
            Menu.AddFunction(new ExitFunction(), "SYSTEM");

            Menu.AddFunction(new FunctionGrouping("PROJECT", "PROJECT", false));

            Menu.AddFunction(new FunctionGrouping("DOCUMENT", "DOCUMENT", false));

            Menu.AddFunction(new FunctionGrouping("HELP", "HELP", false));
            Menu.AddFunction(new UserGuideFunction(), "HELP");
            Menu.AddFunction(new AboutFunction(), "HELP");

            SalesForceApp.LoadSettings();
            SalesForceApp.LoadFunctions();
        }

        /// <summary>
        /// Shows the given window as a modal dialog.
        /// </summary>
        /// <param name="window">The window to display as a dialog.</param>
        /// <returns>The dialog result of the window that was displayed.</returns>
        public static bool ShowDialog(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            if (Instance._window.IsLoaded)
            {
                window.Owner = Instance._window;
                window.ShowInTaskbar = false;
            }

            bool? result = window.ShowDialog();

            return (result.HasValue && result.Value);
        }

        /// <summary>
        /// Sets app to waiting state and returns object that will set app to not waiting when disposed.
        /// </summary>
        /// <param name="message">The message to display while waiting.</param>
        /// <returns>The object that will set the app to not waiting when disposed.</returns>
        public static IDisposable Wait(string message)
        {
            return new DisposableWait();
        }

        /// <summary>
        /// Set the status of the application.
        /// </summary>
        /// <param name="text"></param>
        public static void SetStatus(string text)
        {
            Instance._window.StatusText = text;        
        }

        /// <summary>
        /// Set the app waiting state.
        /// </summary>
        /// <param name="flag">true to set the app to waiting, false to remove the waiting state.</param>
        public static void SetWaiting(bool flag)
        {
            if (flag)
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            else
                System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Handle an exception.
        /// </summary>
        /// <param name="err">The exception to handle.</param>
        public static void HandleException(Exception err)
        {
            if (err == null)
                App.MessageUser("Error message is null.", "Error", MessageBoxImage.Error, new string[] { "OK" });
            else
                App.MessageUser(err.Message, "Error", MessageBoxImage.Error, new string[] { "OK" });
        }

        /// <summary>
        /// Post a message to the user.
        /// </summary>
        /// <param name="message">The message to post.</param>
        /// <param name="caption">The caption for the message.</param>
        /// <param name="image">The image to display with the message.</param>
        /// <param name="answers">Possible answers the user can select in response to the message.</param>
        /// <returns>The answer the user selected.</returns>
        public static string MessageUser(string message, string caption, MessageBoxImage image, IEnumerable<string> answers)
        {
            AskUserWindow dlg = new AskUserWindow();
            dlg.Message = message;
            dlg.Title = caption;
            dlg.Image = image;
            dlg.Answers = answers;
            App.ShowDialog(dlg);
            return dlg.Answer;
        }

        /// <summary>
        /// Register a function for global use.
        /// </summary>
        /// <param name="function">The function to register.</param>
        public void RegisterFunction(IFunction function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (_functions.ContainsKey(function.GetType()))
                _functions.Remove(function.GetType());

            _functions.Add(function.GetType(), function);
        }

        /// <summary>
        /// Get a function that has been registered previously.
        /// </summary>
        /// <typeparam name="TType">The type of the function to get.</typeparam>
        /// <returns>The function if found or null if not found.</returns>
        public TType GetFunction<TType>() where TType : IFunction
        {
            if (_functions.ContainsKey(typeof(TType)))
                return (TType)_functions[typeof(TType)];
            else
                throw new Exception("Function is not registered: " + typeof(TType).Name);
        }

        /// <summary>
        /// Remove a function that has been registered previously.
        /// </summary>
        /// <typeparam name="TType">The type of the function to remove.</typeparam>
        public void UnregisterFunction<TType>() where TType : IFunction
        {
            if (_functions.ContainsKey(typeof(TType)))
                _functions.Remove(typeof(TType));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Notify apps of application exit.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Wallace.IDE.Properties.Settings.Default.LastWindowState = _window.WindowState;
                Wallace.IDE.Properties.Settings.Default.Save();

                if (!e.Cancel)
                {
                    // close salesforce app
                    if (!App.Instance.SalesForceApp.ApplicationClosing())
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Issue update to all workspaces.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Navigation_ActiveNodeChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateWorkspaces();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Issue update to all workspaces.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Content_ActiveDocumentChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateWorkspaces();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Handle the exception that occured.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            App.HandleException(e.Exception);
            e.Handled = true;
        }

        #endregion
    }
}
