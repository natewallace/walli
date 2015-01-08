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

namespace SalesForceData
{
    /// <summary>
    /// Base class for source file data classes.
    /// </summary>
    public abstract class SourceFileData
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        internal SourceFileData(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the SourceFileData.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the underlying salesforce object.
        /// </summary>
        /// <returns>The underlying salesforce object.</returns>
        internal abstract SalesForceAPI.Metadata.Metadata GetMetadata();

        /// <summary>
        /// Create the corresponding source file data object given the data.
        /// </summary>
        /// <param name="file">The file whose data was retrieved.</param>
        /// <param name="data">The data for the given file.</param>
        /// <returns>The newly created source file data object.</returns>
        internal static SourceFileData Create(SourceFile file, SalesForceAPI.Metadata.Metadata data)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (data == null)
                throw new ArgumentNullException("data");

            switch (file.FileType.Name)
            {
                case "Profile":
                    if (!(data is SalesForceAPI.Metadata.Profile))
                        throw new Exception(String.Format(
                            "The type specified({0}) doesn't match the data({1}) passed in.",
                            file.FileType.Name,
                            data.GetType().FullName));
                    return new ProfileData(data as SalesForceAPI.Metadata.Profile);

                default:
                    throw new Exception("Unsupported type: " + file.FileType.Name);
            }
        }

        /// <summary>
        /// Write the data out to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public abstract void WriteToStream(Stream stream);

        /// <summary>
        /// Read the data in from the given stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public abstract void ReadFromStream(Stream stream);

        #endregion
    }
}
