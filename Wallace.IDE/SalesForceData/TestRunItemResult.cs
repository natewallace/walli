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

namespace SalesForceData
{
    /// <summary>
    /// The result from a test run item.
    /// </summary>
    public class TestRunItemResult
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="methodName">MethodName.</param>
        /// <param name="status">Status.</param>
        /// <param name="stackTrace">StackTrace.</param>
        public TestRunItemResult(string message, string methodName, TestRunItemResultStatus status, string stackTrace)
        {
            Message = message;
            MethodName = methodName;
            Status = status;
            StackTrace = stackTrace;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The message to display.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The name of the method the result is for.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// The status for the result.
        /// </summary>
        public TestRunItemResultStatus Status { get; private set; }

        /// <summary>
        /// The stack trace for the result.
        /// </summary>
        public string StackTrace { get; private set; }

        #endregion
    }
}
