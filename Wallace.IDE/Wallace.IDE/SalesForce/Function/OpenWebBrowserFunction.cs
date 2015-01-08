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
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Open the salesforce web browser.
    /// </summary>
    public class OpenWebBrowserFunction : FunctionBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="browser"></param>
        public OpenWebBrowserFunction(ClientBrowser browser)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            Browser = browser;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The browser that will be used.
        /// </summary>
        public ClientBrowser Browser { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            // setup image
            Image image = new Image();
            image.Height = 16;
            image.Width = 16;

            if (Browser.Image == null)
            {
                image.Source = VisualHelper.LoadBitmap("WebBrowser.png");
            }
            else
            {
                image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Browser.Image.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }

            // setup presenter
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = image;
                presenter.ToolTip = String.Format("Open {0}", Browser.Name);
            }
            else
            {
                presenter.Header = String.Format("Open {0}", Browser.Name);
                presenter.Icon = image;
            }
        }

        /// <summary>
        /// Update the visibility.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);
        }

        /// <summary>
        /// Opens a new salesforce browser window.
        /// </summary>
        public override void Execute()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null)
            {
                try
                {
                    using (App.Wait("Opening SalesForce browser..."))
                        Browser.OpenUrl(App.Instance.SalesForceApp.CurrentProject.Client.GetWebsiteAutoLoginUri());

                    Properties.Settings.Default.LastWebBrowser = Browser.Name;
                    Properties.Settings.Default.Save();

                    App.Instance.UpdateWorkspaces();
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
