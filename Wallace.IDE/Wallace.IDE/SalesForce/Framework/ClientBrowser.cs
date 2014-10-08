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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// A locally installed browser.
    /// </summary>
    public class ClientBrowser
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="path">Path.</param>
        /// <param name="image">Image.</param>
        public ClientBrowser(string name, string path, Icon image)
        {
            Name = name;
            Path = path;
            Image = image;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the browser.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The path to the executable.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The icon for the browser.
        /// </summary>
        public Icon Image { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Execute the browser with the given url.
        /// </summary>
        /// <param name="url">The url to open in the browser.</param>
        public void OpenUrl(string url)
        {
            if (Path == null)
                System.Diagnostics.Process.Start(url);
            else
                System.Diagnostics.Process.Start(Path, String.Format("\"{0}\"", url));
        }

        /// <summary>
        /// Get the browsers that are installed on the machine.
        /// </summary>
        /// <returns>The locally installed browsers.</returns>
        public static ClientBrowser[] GetInstalledBrowsers()
        {
            List<ClientBrowser> result = new List<ClientBrowser>();

            using (RegistryKey clients = Registry.LocalMachine.OpenSubKey(@"Software\Clients\StartMenuInternet"))
            {
                if (clients != null)
                {
                    string[] names = clients.GetSubKeyNames();
                    foreach (string name in names)
                    {
                        string browserPath = null;
                        Icon browserIcon = null;

                        using (RegistryKey browser = clients.OpenSubKey(name))
                        {
                            using (RegistryKey path = browser.OpenSubKey(@"shell\open\command"))
                                browserPath = path.GetValue(null) as string;

                            using (RegistryKey icon = browser.OpenSubKey("DefaultIcon"))
                            {
                                string iconPath = icon.GetValue(null) as string;
                                if (iconPath != null)
                                {
                                    string[] iconPathParts = iconPath.Split(new char[] { ',' });
                                    if (iconPathParts.Length > 0)
                                        browserIcon = Icon.ExtractAssociatedIcon(iconPathParts[0]);
                                }
                            }
                        }

                        if (browserPath != null)
                            result.Add(new ClientBrowser(name, browserPath, browserIcon));
                    }
                }
            }

            if (result.Count == 0)
                result.Add(new ClientBrowser("Default", null, null));

            return result.ToArray();
        }

        #endregion
    }
}
