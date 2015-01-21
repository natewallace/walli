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

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// A commit that has been made to a simple repository.
    /// </summary>
    public class SimpleRepositoryCommit
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="commit">The underlying commit to build this object from.</param>
        internal SimpleRepositoryCommit(Commit commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");

            Sha = commit.Sha;
            Comment = commit.MessageShort;
            if (commit.Author != null)
            {
                AuthorName = commit.Author.Name;
                AuthorEmail = commit.Author.Email;
                Date = commit.Author.When.LocalDateTime;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The sha that identifies this commit.
        /// </summary>
        public string Sha { get; private set; }

        /// <summary>
        /// A shortened representation of the Sha property.
        /// </summary>
        public string ShaShort
        {
            get
            {
                return (Sha == null || Sha.Length < 7) ? String.Empty : Sha.Substring(0, 7);
            }
        }

        /// <summary>
        /// The comment for the commit.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// The autors name.
        /// </summary>
        public string AuthorName { get; private set; }

        /// <summary>
        /// The authors email.
        /// </summary>
        public string AuthorEmail { get; private set; }

        /// <summary>
        /// The date of the commit.
        /// </summary>
        public DateTime Date { get; private set; }

        #endregion
    }
}
