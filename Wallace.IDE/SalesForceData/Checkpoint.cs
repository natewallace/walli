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
    /// A checkpoint in salesforce.
    /// </summary>
    public class Checkpoint
    {
        #region Fields

        /// <summary>
        /// The underlying salesforce object.
        /// </summary>
        SalesForceAPI.Tooling.ApexExecutionOverlayAction _action;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">The action to build this object from.</param>
        /// <param name="fileName">The name of the file that the checkpoint is in.</param>
        internal Checkpoint(SalesForceAPI.Tooling.ApexExecutionOverlayAction action, string fileName)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
            FileName = fileName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Id of the checkpoint.
        /// </summary>
        public string Id
        {
            get { return _action.Id; }
        }

        /// <summary>
        /// The name of the file the checkpoint is in.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// A script that is executed when the checkpoint is hit.
        /// </summary>
        public string Script
        {
            get { return _action.ActionScript; }
            set { _action.ActionScript = value; }
        }

        /// <summary>
        /// The script type.
        /// </summary>
        public CheckpointScriptType ScriptType
        {
            get { return (CheckpointScriptType)Enum.Parse(typeof(CheckpointScriptType), _action.ActionScriptType); }
            set { _action.ActionScriptType = value.ToString(); }
        }

        /// <summary>
        /// The expiration date of the checkpoint.
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return _action.ExpirationDate.Value; }
            set { _action.ExpirationDate = value; }
        }

        /// <summary>
        /// The number of times before the checkpoint is processed.
        /// </summary>
        public int Iteration
        {
            get { return _action.Iteration.Value; }
            set { _action.Iteration = value; }
        }

        /// <summary>
        /// The line number of the checkpoint.
        /// </summary>
        public int LineNumber
        {
            get { return _action.Line.Value; }
            set { _action.Line = value; }
        }

        /// <summary>
        /// The id of the user who owns the checkpoint.
        /// </summary>
        public string UserId
        {
            get { return _action.ScopeId; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the underlying salesforce object.
        /// </summary>
        /// <returns>The underlying salesforce object.</returns>
        internal SalesForceAPI.Tooling.ApexExecutionOverlayAction ToAction()
        {
            return _action;
        }

        #endregion
    }
}
