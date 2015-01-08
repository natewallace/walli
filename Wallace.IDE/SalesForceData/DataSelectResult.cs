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
using System.Data;

namespace SalesForceData
{
    /// <summary>
    /// The result from a SalesForceClient.DataSelect call.
    /// </summary>
    public class DataSelectResult
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="isMore">IsMore.</param>
        /// <param name="queryLocator">QueryLocator.</param>
        /// <param name="size">Size.</param>
        /// <param name="index">Index.</param>
        public DataSelectResult(DataTable data, bool isMore, string queryLocator, int size, int index)
        {
            if (data == null)
                throw new ArgumentException("data");

            Data = data;
            IsMore = isMore;
            QueryLocator = queryLocator;
            Size = size;
            Index = index;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The data that resulted from the query.
        /// </summary>
        public DataTable Data { get; private set; }

        /// <summary>
        /// Flag that indicates if there is more data that has yet to be returned.
        /// </summary>
        public bool IsMore { get; private set; }

        /// <summary>
        /// Locator used to resume query if the IsMore flag is set to true.
        /// </summary>
        public string QueryLocator { get; private set; }

        /// <summary>
        /// The number of records for the query.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// The current index.
        /// </summary>
        public int Index { get; private set; }

        #endregion
    }
}
