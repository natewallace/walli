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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Wallace.IDE.Framework;
using Wallace.IDE.Framework.UI;

namespace Wallace.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// Supports the IsNavigationHidden property.
        /// </summary>
        private bool _isNavigationHidden;

        /// <summary>
        /// Holds the last navigation width before being hidden.
        /// </summary>
        private double _lastNavigationWidth;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _lastNavigationWidth = 300;
            _isNavigationHidden = true;
            IsNavigationHidden = false;
            SessionTitle = null;

            updateNotice.Visibility = UpdateNoticeControl.IsUpdateAvailable() ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Text that will be displayed after the window title and in the navigation title.
        /// </summary>
        public string SessionTitle
        {
            get
            {
                return textBlockTitle.Text;
            }
            set
            {
                // remove update notice when something gets opened
                if (updateNotice != null && value != null)
                {
                    gridMain.Children.Remove(updateNotice);
                    updateNotice = null;
                }

                textBlockTitle.Text = value;

                StringBuilder sb = new StringBuilder("Walli");
                if (!String.IsNullOrWhiteSpace(textBlockTitle.Text))
                    sb.AppendFormat(" - {0}", textBlockTitle.Text);

                Title = sb.ToString();

                dockPanelNavigation.Visibility = (value == null) ? Visibility.Collapsed : Visibility.Visible;
                if (value == null)
                    IsNavigationHidden = false;
            }
        }

        /// <summary>
        /// Hides or unhides the navigation control.
        /// </summary>
        public bool IsNavigationHidden
        {
            get
            {
                return _isNavigationHidden;
            }
            set
            {
                if (_isNavigationHidden != value)
                {
                    _isNavigationHidden = value;

                    if (_isNavigationHidden)
                    {
                        _lastNavigationWidth = gridMain.ColumnDefinitions[0].ActualWidth;
                        gridSplitterMain.Visibility = Visibility.Collapsed;
                        nodesTabTree.Visibility = Visibility.Collapsed;
                        textBlockTitle.Visibility = Visibility.Collapsed;
                        borderNavigation.BorderThickness = new Thickness(1);
                        buttonHideNavigation.Content = VisualHelper.CreateIconHeader(null, "PinAlt.png", new Thickness(1));
                        buttonHideNavigation.ToolTip = "Unhide";
                        gridMain.ColumnDefinitions[0].Width = GridLength.Auto;
                    }
                    else
                    {
                        buttonHideNavigation.Content = VisualHelper.CreateIconHeader(null, "Pin.png", new Thickness(1));
                        buttonHideNavigation.ToolTip = "Hide";
                        borderNavigation.BorderThickness = new Thickness(1, 1, 1, 0);
                        textBlockTitle.Visibility = Visibility.Visible;
                        nodesTabTree.Visibility = Visibility.Visible;
                        gridSplitterMain.Visibility = Visibility.Visible;
                        gridMain.ColumnDefinitions[0].Width = new GridLength(_lastNavigationWidth);
                    }
                }
            }
        }

        /// <summary>
        /// The main menu.
        /// </summary>
        public Menu MainMenu
        {
            get { return menuMain; }
        }

        /// <summary>
        /// The main toolbar.
        /// </summary>
        public ToolBar MainToolBar
        {
            get { return toolBarMain; }
        }

        /// <summary>
        /// The tab control used to display nodes.
        /// </summary>
        public TabControl Nodes
        {
            get { return nodesTabTree; }
        }

        /// <summary>
        /// The tab control used to display documents.
        /// </summary>
        public TabControl Documents
        {
            get { return documentsTabControl; }
        }

        /// <summary>
        /// The text displayed in the status bar.
        /// </summary>
        public string StatusText
        {
            get { return statusText.Text; }
            set { statusText.Text = value; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Toggle the visibility of the navigation control.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonHideNavigation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsNavigationHidden = !IsNavigationHidden;
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
