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

using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Implements IDocumentPresenter using a TabItem.
    /// </summary>
    public class TabItemDocumentPresenter : HostBase<TabItem>, IDocumentPresenter
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabItemDocumentPresenter()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="host">Host.</param>
        public TabItemDocumentPresenter(TabItem host)
        {
            Host = host;
        }

        #endregion

        #region IDocumentPresenter Members

        /// <summary>
        /// The header displayed for the document.
        /// </summary>
        public object Header
        {
            get
            {
                EnsureHost();
                return Host.Header;
            }
            set
            {
                EnsureHost();
                Host.Header = value;
            }
        }

        /// <summary>
        /// The tooltip displayed for the document.
        /// </summary>
        public object ToolTip
        {
            get
            {
                EnsureHost();
                if (Host.Header is FrameworkElement)
                    return (Host.Header as FrameworkElement).ToolTip;
                else
                    return null;
            }
            set
            {
                EnsureHost();
                if (Host.Header is FrameworkElement)
                    (Host.Header as FrameworkElement).ToolTip = value;
            }
        }

        /// <summary>
        /// The content displayed for the document.
        /// </summary>
        public object Content
        {
            get
            {
                EnsureHost();
                return Host.Content;
            }
            set
            {
                EnsureHost();
                Host.Content = value;
            }
        }

        #endregion
    }
}
