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
    /// Login hours.
    /// </summary>
    public class ProfileDataLoginHours : SourceFileDataElement<SalesForceAPI.Metadata.ProfileLoginHours>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ProfileDataLoginHours()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The object to build this object from.</param>
        internal ProfileDataLoginHours(SalesForceAPI.Metadata.ProfileLoginHours data)
            : base(data)
        {
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Start time.
        /// </summary>
        public string SundayStart
        {
            get { return Data.sundayStart; }
            set { Data.sundayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string SundayEnd
        {
            get { return Data.sundayEnd; }
            set { Data.sundayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string MondayStart
        {
            get { return Data.mondayStart; }
            set { Data.mondayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string MondayEnd
        {
            get { return Data.mondayEnd; }
            set { Data.mondayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string TuesdayStart
        {
            get { return Data.tuesdayStart; }
            set { Data.tuesdayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string TuesdayEnd
        {
            get { return Data.tuesdayEnd; }
            set { Data.tuesdayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string WednesdayStart
        {
            get { return Data.wednesdayStart; }
            set { Data.wednesdayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string WednesdayEnd
        {
            get { return Data.wednesdayEnd; }
            set { Data.wednesdayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string ThursdayStart
        {
            get { return Data.thursdayStart; }
            set { Data.thursdayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string ThursdayEnd
        {
            get { return Data.thursdayEnd; }
            set { Data.thursdayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string FridayStart
        {
            get { return Data.fridayStart; }
            set { Data.fridayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string FridayEnd
        {
            get { return Data.fridayEnd; }
            set { Data.fridayEnd = value; }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        public string SaturdayStart
        {
            get { return Data.saturdayStart; }
            set { Data.saturdayStart = value; }
        }

        /// <summary>
        /// End time.
        /// </summary>
        public string SaturdayEnd
        {
            get { return Data.saturdayEnd; }
            set { Data.saturdayEnd = value; }
        }

        #endregion
    }
}
