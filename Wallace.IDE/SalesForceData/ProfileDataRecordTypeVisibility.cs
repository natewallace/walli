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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// Record type visibility.
    /// </summary>
    public class ProfileDataRecordTypeVisibility : SourceFileDataElement<SalesForceAPI.Metadata.ProfileRecordTypeVisibility>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The object to build this object from.</param>
        internal ProfileDataRecordTypeVisibility(SalesForceAPI.Metadata.ProfileRecordTypeVisibility data)
            : base(data)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Record type name.
        /// </summary>
        public string RecordTypeName
        {
            get { return Data.recordType; }
            set { Data.recordType = value; }
        }

        /// <summary>
        /// Person account default.
        /// </summary>
        public bool PersonAccountDefaultVisible
        {
            get { return Data.personAccountDefault; }
            set
            {
                Data.personAccountDefault = value;
                Data.personAccountDefaultSpecified = true;
            }
        }

        /// <summary>
        /// Default visibility.
        /// </summary>
        [DisplayName("Default")]
        public bool DefaultVisible
        {
            get { return Data.@default; }
            set 
            { 
                Data.@default = value;
                OnPropertyChanged("DefaultVisible");
            }
        }

        /// <summary>
        /// Visibility.
        /// </summary>
        public bool Visible
        {
            get { return Data.visible; }
            set 
            { 
                Data.visible = value;
                OnPropertyChanged("Visible");
            }
        }

        #endregion
    }
}