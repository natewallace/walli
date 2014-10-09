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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for UpdateNoticeControl.xaml
    /// </summary>
    public partial class UpdateNoticeControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateNoticeControl()
        {
            InitializeComponent();
            System.Threading.ThreadPool.QueueUserWorkItem(CheckForUpdate);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks to see if an update is available.
        /// </summary>
        /// <returns>true if an update is available.</returns>
        public static bool IsUpdateAvailable()
        {
            Version availableVersion = new Version(Wallace.IDE.Properties.Settings.Default.LatestVersion);
            Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            return (currentVersion < availableVersion);
        }

        /// <summary>
        /// Checks for a new version of the app.
        /// </summary>
        /// <param name="state">Not used.</param>
        private void CheckForUpdate(object state)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xml = client.DownloadString(Wallace.IDE.Properties.Settings.Default.LatestVersionEndpoint);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xml);
                    XmlNodeList nodes = xmlDocument.SelectNodes("/rss/channel/item/title");
                    foreach (XmlNode node in nodes)
                    {
                        if (node != null && node.InnerText != null && Regex.IsMatch(node.InnerText, "Release", RegexOptions.IgnoreCase))
                        {
                            Match match = Regex.Match(node.InnerText, "Walli[ ]+v[0-9]+.[0-9]+(.[0-9]+)?(.[0-9]+)?", RegexOptions.IgnoreCase);
                            if (match != null)
                            {
                                string versionText = match.Value.Substring(match.Value.IndexOf("v", StringComparison.CurrentCultureIgnoreCase) + 1);
                                Version version = new Version(versionText);

                                App.Instance.Dispatcher.Invoke(new Action(() =>
                                {
                                    Wallace.IDE.Properties.Settings.Default.LatestVersion = versionText;
                                    Wallace.IDE.Properties.Settings.Default.Save();
                                }));

                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore errors
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Open the walli homepage in a browser.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (App.Wait("Opening home page..."))
                    ClientBrowser.GetDefaultBrowser().OpenUrl("https://walli.codeplex.com/");
            }
            catch
            {
            }
        }

        #endregion
    }
}
