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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// IP range.
    /// </summary>
    public class ProfileDataLoginIPRange : SourceFileDataElement<SalesForceAPI.Metadata.ProfileLoginIpRange>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The object to build this object from.</param>
        internal ProfileDataLoginIPRange(SalesForceAPI.Metadata.ProfileLoginIpRange data)
            : base(data)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get { return Data.description; }
            set 
            { 
                Data.description = value;
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Start address.
        /// </summary>
        [DisplayName("Start Address")]
        public string StartAddress
        {
            get { return Data.startAddress; }
            set 
            { 
                Data.startAddress = value;
                OnPropertyChanged("StartAddress");
            }
        }

        /// <summary>
        /// End address.
        /// </summary>
        [DisplayName("End Address")]
        public string EndAddress
        {
            get { return Data.endAddress; }
            set 
            { 
                Data.endAddress = value;
                OnPropertyChanged("EndAddress");
            }
        }

        #endregion
    }
}
