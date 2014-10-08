using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.Function
{
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
    public class OpenRecentWebBrowserFunction : FunctionBase
    {
        #region Fields

        /// <summary>
        /// Keeps track of the last name to see if it has changed.
        /// </summary>
        private string _lastName = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenRecentWebBrowserFunction()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The browser that will be used.
        /// </summary>
        public ClientBrowser Browser
        {
            get
            {
                ClientBrowser result = ClientBrowser.GetBrowser(Properties.Settings.Default.LastWebBrowser);
                if (result == null)
                    return ClientBrowser.GetDefaultBrowser();
                else
                    return result;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the visibility.
        /// </summary>
        /// <param name="host">The type of host for this function.</param>
        /// <param name="presenter">The presenter to use when updating the view.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (App.Instance.SalesForceApp.CurrentProject != null);

            if (_lastName != Browser.Name)
            {
                _lastName = Browser.Name;

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
