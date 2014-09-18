﻿/*
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
    /// A salesforce data element.
    /// </summary>
    public abstract class SourceFileDataElement<TType> where TType : class, new()
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        protected SourceFileDataElement()
        {
            Data = Activator.CreateInstance<TType>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Data.</param>
        protected SourceFileDataElement(TType data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The underlying salesforce object.
        /// </summary>
        internal TType Data { get; private set; }

        #endregion
    }
}
