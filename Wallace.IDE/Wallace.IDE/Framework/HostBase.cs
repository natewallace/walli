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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Base for classes that host another control.
    /// </summary>
    /// <typeparam name="TType">The type of Host.</typeparam>
    public abstract class HostBase<TType> where TType : class
    {
        #region Fields

        /// <summary>
        /// Supports the Host property.
        /// </summary>
        private TType _host;

        #endregion

        #region Properties

        /// <summary>
        /// The host for the class.
        /// </summary>
        public virtual TType Host
        {
            get
            {
                return _host;
            }
            set
            {
                BeforeHostSet();
                _host = value;
                AfterHostSet();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called immediately before the Host property has been set.
        /// </summary>
        protected virtual void BeforeHostSet()
        {
        }

        /// <summary>
        /// Called immediately after the Host property has been set.
        /// </summary>
        protected virtual void AfterHostSet()
        {
        }

        /// <summary>
        /// Checks to see if Host is null and throws an exception if it is.
        /// </summary>
        protected virtual void EnsureHost()
        {
            if (Host == null)
                throw new Exception("Host has not been set.");
        }

        #endregion
    }
}
