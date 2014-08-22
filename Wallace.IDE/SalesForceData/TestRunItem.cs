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

namespace SalesForceData
{
    /// <summary>
    /// An individual test run item.
    /// </summary>
    public class TestRunItem
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="apexClassId">ApexClassId.</param>
        internal TestRunItem(string name, string apexClassId)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (String.IsNullOrWhiteSpace(apexClassId))
                throw new ArgumentException("apexClassId is null or whitespace.");

            Name = name;
            ApexClassId = apexClassId;
            Status = TestRunItemStatus.Preparing;
            ExtendedStatus = String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the test class that is running.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The id of the test class that is running.
        /// </summary>
        internal string ApexClassId { get; private set; }

        /// <summary>
        /// The status of the test run item.
        /// </summary>
        public TestRunItemStatus Status { get; internal set; }

        /// <summary>
        /// The extended status of the test run item.
        /// </summary>
        public string ExtendedStatus { get; internal set; }

        #endregion
    }
}
