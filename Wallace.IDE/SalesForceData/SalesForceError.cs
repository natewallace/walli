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
using System.Text;
using Newtonsoft.Json.Linq;

namespace SalesForceData
{
    /// <summary>
    /// A salesforce error that has occured.
    /// </summary>
    public class SalesForceError
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">StatusCode.</param>
        /// <param name="message">Message.</param>
        /// <param name="fields">Fields.</param>
        public SalesForceError(string statusCode, string message, IEnumerable<string> fields)
        {
            StatusCode = statusCode;
            Message = message;
            Fields = (fields == null) ? new List<string>() : new List<string>(fields);

            // parse json text
            if (message != null)
            {
                JArray array = null;
                if (message.StartsWith("["))
                {
                    try
                    {
                        array = JArray.Parse(message);

                        if (array != null && array.Count > 0)
                        {
                            string problemText = null;
                            JToken problemToken = array[0]["problem"];
                            if (problemToken != null)
                            {
                                if (problemToken.HasValues)
                                    problemText = problemToken.First.Value<string>();
                                else
                                    problemText = problemToken.Value<string>();
                            }

                            string lineText = null;
                            JToken lineToken = array[0]["line"];
                            if (lineToken != null)
                            {
                                if (lineToken.HasValues)
                                    lineText = lineToken.First.Value<string>();
                                else
                                    lineText = lineToken.Value<string>();
                            }

                            Message = String.Format("{0}.  line: {1}", System.Web.HttpUtility.HtmlDecode(problemText), lineText);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The status code for the error.
        /// </summary>
        public string StatusCode { get; private set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Fields that are part of the error.
        /// </summary>
        public IReadOnlyList<string> Fields { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a human readable string for this object.
        /// </summary>
        /// <returns>A human readable string for this object.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}", StatusCode, Message);
            //sb.AppendLine();
            //foreach (string field in Fields)
            //    sb.AppendLine(field);


            return sb.ToString();
        }

        #endregion
    }
}
