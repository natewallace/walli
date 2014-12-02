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

namespace SalesForceData
{
    /// <summary>
    /// A log created by SalesForce.
    /// </summary>
    public class Log
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="log">The log to build from this one.</param>
        internal Log(SalesForceAPI.Tooling.ApexLog log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            Id = log.Id;
            StartTime = log.StartTime.HasValue ? log.StartTime.Value.ToLocalTime() : DateTime.MinValue;
            Duration = log.DurationMilliseconds.HasValue ? log.DurationMilliseconds.Value : 0;
            Operation = log.Operation;
            Status = log.Status;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id of the log.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The time the log was started.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// The duration of the log from start to finish in milliseconds.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// The operation the log is for.
        /// </summary>
        public string Operation { get; private set; }

        /// <summary>
        /// The result of the log.
        /// </summary>
        public string Status { get; private set; }

        #endregion
    }
}
