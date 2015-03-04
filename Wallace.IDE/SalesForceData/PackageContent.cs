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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// Class used to read SourceFile content from a package.
    /// </summary>
    public class PackageContent : IDisposable
    {
        #region Fields

        /// <summary>
        /// The zip archive that is the package.
        /// </summary>
        private ZipArchive _zip;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="package">The package bits.</param>
        public PackageContent(byte[] package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            MemoryStream ms = new MemoryStream(package);
            _zip = new ZipArchive(ms, ZipArchiveMode.Read);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the content for the given file.
        /// </summary>
        /// <param name="file">The file to get the content for.</param>
        /// <returns>The requested content or null if the file can't be found in the package.</returns>
        public string GetContent(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (String.IsNullOrWhiteSpace(file.FileName))
                return null;

            ZipArchiveEntry entry = _zip.GetEntry(file.FileName);
            if (entry == null)
                return null;

            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
        {
            if (_zip != null)
                _zip.Dispose();
        }

        #endregion
    }
}
