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
using System.Runtime.InteropServices;
using System.Security;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// Adds extension methods to SecureString.
    /// </summary>
    public static class SecureStringUtility
    {
        /// <summary>
        /// Gets the plain text from a SecureString instance.
        /// </summary>
        /// <param name="secureString">The secure string to get the plain text from.</param>
        /// <returns>The plain text from the secure string.</returns>
        public static string GetPlainText(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            IntPtr bstr = Marshal.SecureStringToBSTR(secureString);
            string text = Marshal.PtrToStringBSTR(bstr);
            Marshal.FreeBSTR(bstr);

            return text;
        }

        /// <summary>
        /// Create a new secure string.
        /// </summary>
        /// <param name="text">The text to create a secure string with.</param>
        /// <returns>A new secure string.</returns>
        public static SecureString CreateSecureString(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            SecureString secureString = new SecureString();
            foreach (char c in text)
                secureString.AppendChar(c);

            return secureString;
        }
    }
}
