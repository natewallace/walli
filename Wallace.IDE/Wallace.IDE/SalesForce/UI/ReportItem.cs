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
using System.Text;
using System.Threading.Tasks;
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// A single report item in a report.
    /// </summary>
    public class ReportItem
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">File.</param>
        public ReportItem(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            File = file;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The file that is the report item.
        /// </summary>
        public SourceFile File { get; private set; }

        /// <summary>
        /// Is set to true when selected by the user.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// The file type.
        /// </summary>
        public string FileType
        {
            get { return File.FileType.ToString(); }
        }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name
        {
            get { return File.Name; }
        }

        /// <summary>
        /// The name of the user who last changed the file.
        /// </summary>
        public string ChangedByName
        {
            get { return File.ChangedByName; }
        }

        /// <summary>
        /// The date the file was last changed.
        /// </summary>
        public string ChangedOn
        {
            get { return File.ChangedOn.ToString(); }
        }
        
        /// <summary>
        /// The name of the user who created the file.
        /// </summary>
        public string CreatedByName
        {
            get { return File.CreatedByName; }
        }

        /// <summary>
        /// The date the file was created.
        /// </summary>
        public string CreatedOn
        {
            get { return File.CreatedOn.ToString(); }
        }
                
        /// <summary>
        /// The name of the file on disk.
        /// </summary>
        public string FileName
        {
            get { return File.FileName; }
        }

        /// <summary>
        /// The current state of the file.
        /// </summary>
        public string State
        {
            get { return File.State.ToString(); }
        }

        #endregion
    }
}
