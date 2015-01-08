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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Wallace.IDE.Framework;

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// A locally installed browser.
    /// </summary>
    public class ClientBrowser
    {
        #region Fields

        /// <summary>
        /// Supports the GetInstalledBrowsers method.
        /// </summary>
        private static ClientBrowser[] _browsers = null;

        /// <summary>
        /// Supports the GetDefaultBrowser method.
        /// </summary>
        private static ClientBrowser _defaultBrowser = null;

        #endregion

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
        /// Get the browser with the given name.
        /// </summary>
        /// <param name="name">The name of the browser to get.</param>
        /// <returns>The requested browser or null if it doesn't exist.</returns>
        public static ClientBrowser GetBrowser(string name)
        {
            foreach (ClientBrowser b in GetInstalledBrowsers())
                if (b.Name == name)
                    return b;

            return null;
        }

        /// <summary>
        /// Gets the default browser.
        /// </summary>
        /// <returns>The default browser.</returns>
        public static ClientBrowser GetDefaultBrowser()
        {
            if (_defaultBrowser == null)
            {
                string name = null;

                using (RegistryKey userChoice = Registry.CurrentUser.OpenSubKey(@"Software\Clients\StartMenuInternet"))
                    if (userChoice != null)
                        name = userChoice.GetValue(null) as string;

                if (name == null)
                {
                    using (RegistryKey userChoice = Registry.LocalMachine.OpenSubKey(@"Software\Clients\StartMenuInternet"))
                        if (userChoice != null)
                            name = userChoice.GetValue(null) as string;
                }

                foreach (ClientBrowser browser in GetInstalledBrowsers())
                {
                    if (browser.Name == name)
                    {
                        _defaultBrowser = browser;
                        break;
                    }
                }

                if (_defaultBrowser == null)
                {
                    _defaultBrowser = new ClientBrowser("Default Browser", null, null);
                }
            }

            return _defaultBrowser;
        }

        /// <summary>
        /// Get the browsers that are installed on the machine.
        /// </summary>
        /// <returns>The locally installed browsers.</returns>
        public static ClientBrowser[] GetInstalledBrowsers()
        {
            if (_browsers == null)
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

                _browsers = result.ToArray();
            }

            return _browsers;
        }

        #endregion
    }
}
