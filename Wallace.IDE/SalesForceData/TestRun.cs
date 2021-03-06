﻿/*
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

namespace SalesForceData
{
    /// <summary>
    /// A test run.
    /// </summary>
    public class TestRun
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jobId">JobId.</param>
        /// <param name="items">Items.</param>
        internal TestRun(string jobId, TestRunItem[] items)
        {
            if (String.IsNullOrWhiteSpace(jobId))
                throw new ArgumentException("jobId is null or whitespace");

            JobId = jobId;
            Items = items ?? new TestRunItem[0];
            Started = DateTime.Now;
            Finished = DateTime.MinValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id of the job that is running the test.
        /// </summary>
        public string JobId { get; private set; }

        /// <summary>
        /// The items in the test run.
        /// </summary>
        public TestRunItem[] Items { get; private set; }

        /// <summary>
        /// The date and time this test was started.
        /// </summary>
        public DateTime Started { get; internal set; }

        /// <summary>
        /// The date and time this test was finished.
        /// </summary>
        public DateTime Finished { get; internal set; }

        /// <summary>
        /// Returns true if all of the Items in this test run are done.
        /// </summary>
        public bool IsDone
        {
            get
            {
                foreach (TestRunItem item in Items)
                    if (!item.IsDone)
                        return false;

                return true;
            }
        }

        #endregion
    }
}
