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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// File related helper methods.
    /// </summary>
    public class FileUtility
    {
        #region Methods

        /// <summary>
        /// Check to see if the given folder is empty.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the folder is empty, false if it is not.</returns>
        public static bool IsFolderEmpty(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);
            return (files.Length == 0 && folders.Length == 0);
        }

        /// <summary>
        /// Delete the folder and all of it's contents.
        /// </summary>
        /// <param name="path">The path to the folder to delete.</param>
        public static void DeleteFolder(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string folder in folders)
                DeleteFolder(folder);

            Directory.Delete(path);
        }

        /// <summary>
        /// Delete the contents of the folder but not the folder itself.
        /// </summary>
        /// <param name="path">The path to the folder to delete contents from.</param>
        public static void DeleteFolderContents(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string folder in folders)
                DeleteFolder(folder);
        }

        #endregion
    }
}
