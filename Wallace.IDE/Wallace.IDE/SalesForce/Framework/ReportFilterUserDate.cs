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

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// A report filter for files that have been changed on or after a given date by the given user.
    /// </summary>
    public class ReportFilterUserDate : IReportFilter
    {
        #region Fields

        /// <summary>
        /// The id of the user to use for filter.
        /// </summary>
        private string _userId;

        /// <summary>
        /// The date to use for filter.
        /// </summary>
        private DateTime _date;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userId">The id of the user to filter with.</param>
        /// <param name="date">The date to filter with.</param>
        public ReportFilterUserDate(string userId, DateTime date)
        {
            _userId = userId;
            _date = date;
        }

        #endregion

        #region IReportFilter Members

        /// <summary>
        /// Filter out files.
        /// </summary>
        /// <param name="files">The files to filter.</param>
        /// <returns>The files that were not filtered out.</returns>
        public SourceFile[] Filter(IEnumerable<SourceFile> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");

            List<SourceFile> result = new List<SourceFile>();

            foreach (SourceFile file in files)
            {
                if ((_userId == "*" || file.ChangedById == _userId) && file.ChangedOn >= _date)
                    result.Add(file);

                foreach (SourceFile child in file.Children)
                    if (child.ChangedById == _userId && child.ChangedOn >= _date)
                        result.Add(child);
            }

            return result.ToArray();
        }

        #endregion
    }
}
