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

namespace SalesForceData
{
    /// <summary>
    /// Content for a source file.
    /// </summary>
    public class SourceFileContent
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contentType">ContentType.</param>
        /// <param name="contentValue">ContentValue.</param>
        /// <param name="lastModifiedTimeStamp">LastModifiedTimeStamp.</param>
        public SourceFileContent(string contentType, string contentValue, string lastModifiedTimeStamp)
            : this(contentType, contentValue, lastModifiedTimeStamp, null, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contentType">ContentType.</param>
        /// <param name="contentValue">ContentValue.</param>
        /// <param name="lastModifiedTimeStamp">LastModifiedTimeStamp.</param>
        /// <param name="metadataValue">MetadataValue.</param>
        /// <param name="isReadOnly">IsReadOnly.</param>
        public SourceFileContent(string contentType, string contentValue, string lastModifiedTimeStamp, string metadataValue, bool isReadOnly)
        {
            ContentType = contentType;
            ContentValue = contentValue;
            LastModifiedTimeStamp = lastModifiedTimeStamp;
            MetadataValue = metadataValue;
            IsReadOnly = isReadOnly;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of content.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// The value for the content.
        /// </summary>
        public string ContentValue { get; private set; }

        /// <summary>
        /// The date the content was last modifed.
        /// </summary>
        public string LastModifiedTimeStamp { get; private set; }

        /// <summary>
        /// The metadata that describes the content.
        /// </summary>
        public string MetadataValue { get; private set; }

        /// <summary>
        /// Indicates if the content is read only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        #endregion
    }
}
