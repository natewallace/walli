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

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Event arguments for a user search.
    /// </summary>
    public class UserSearchEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Supports the Results property.
        /// </summary>
        private User[] _results;

        #endregion

        #region Constructors

        public UserSearchEventArgs(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                throw new ArgumentException("query is null or whitespace.", "query");

            Query = query;
            Results = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The query for the user search.
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// The results from a search using the given query.
        /// </summary>
        public User[] Results 
        {
            get { return _results; }
            set { _results = value ?? new User[0]; }
        }

        #endregion
    }
}
