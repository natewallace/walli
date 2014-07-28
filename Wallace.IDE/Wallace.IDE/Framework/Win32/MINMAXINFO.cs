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

using System.Runtime.InteropServices;

namespace Wallace.IDE.Framework.Win32
{
    /// <summary>
    /// Contains information about a window's maximized size and position and its minimum and maximum tracking size.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        #region Fields

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public POINT ptReserved;

        /// <summary>
        /// The maximized width (x member) and the maximized height (y member) of the window. For top-level windows, 
        /// this value is based on the width of the primary monitor.
        /// </summary>
        public POINT ptMaxSize;

        /// <summary>
        /// The position of the left side of the maximized window (x member) and the position of the top of the 
        /// maximized window (y member). For top-level windows, this value is based on the position of the primary 
        /// monitor.
        /// </summary>
        public POINT ptMaxPosition;

        /// <summary>
        /// The minimum tracking width (x member) and the minimum tracking height (y member) of the window. This value 
        /// can be obtained programmatically from the system metrics SM_CXMINTRACK and SM_CYMINTRACK 
        /// (see the GetSystemMetrics function).
        /// </summary>
        public POINT ptMinTrackSize;

        /// <summary>
        /// The maximum tracking width (x member) and the maximum tracking height (y member) of the window. This value 
        /// is based on the size of the virtual screen and can be obtained programmatically from the system metrics 
        /// SM_CXMAXTRACK and SM_CYMAXTRACK (see the GetSystemMetrics function).
        /// </summary>
        public POINT ptMaxTrackSize;

        #endregion
    }
}
