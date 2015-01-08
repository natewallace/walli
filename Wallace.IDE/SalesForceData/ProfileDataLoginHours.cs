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
        [DisplayName("Sunday Start")]
        public string SundayStart
        {
            get { return Data.sundayStart; }
            set 
            { 
                Data.sundayStart = value;
                OnPropertyChanged("SundayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Sunday End")]
        public string SundayEnd
        {
            get { return Data.sundayEnd; }
            set 
            {
                Data.sundayEnd = value;
                OnPropertyChanged("SundayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Monday Start")]
        public string MondayStart
        {
            get { return Data.mondayStart; }
            set 
            { 
                Data.mondayStart = value;
                OnPropertyChanged("MondayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Monday End")]
        public string MondayEnd
        {
            get { return Data.mondayEnd; }
            set 
            { 
                Data.mondayEnd = value;
                OnPropertyChanged("MondayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Tuesday Start")]
        public string TuesdayStart
        {
            get { return Data.tuesdayStart; }
            set 
            { 
                Data.tuesdayStart = value;
                OnPropertyChanged("TuesdayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Tuesday End")]
        public string TuesdayEnd
        {
            get { return Data.tuesdayEnd; }
            set 
            { 
                Data.tuesdayEnd = value;
                OnPropertyChanged("TuesdayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Wednesday Start")]
        public string WednesdayStart
        {
            get { return Data.wednesdayStart; }
            set 
            { 
                Data.wednesdayStart = value;
                OnPropertyChanged("WednesdayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Wednesday End")]
        public string WednesdayEnd
        {
            get { return Data.wednesdayEnd; }
            set 
            { 
                Data.wednesdayEnd = value;
                OnPropertyChanged("WednesdayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Thursday Start")]
        public string ThursdayStart
        {
            get { return Data.thursdayStart; }
            set 
            { 
                Data.thursdayStart = value;
                OnPropertyChanged("ThursdayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Thursday End")]
        public string ThursdayEnd
        {
            get { return Data.thursdayEnd; }
            set 
            { 
                Data.thursdayEnd = value;
                OnPropertyChanged("ThursdayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Friday Start")]
        public string FridayStart
        {
            get { return Data.fridayStart; }
            set 
            { 
                Data.fridayStart = value;
                OnPropertyChanged("FridayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Friday End")]
        public string FridayEnd
        {
            get { return Data.fridayEnd; }
            set 
            { 
                Data.fridayEnd = value;
                OnPropertyChanged("FridayEnd");
            }
        }

        /// <summary>
        /// Start time.
        /// </summary>
        [DisplayName("Saturday Start")]
        public string SaturdayStart
        {
            get { return Data.saturdayStart; }
            set 
            { 
                Data.saturdayStart = value;
                OnPropertyChanged("SaturdayStart");
            }
        }

        /// <summary>
        /// End time.
        /// </summary>
        [DisplayName("Saturday End")]
        public string SaturdayEnd
        {
            get { return Data.saturdayEnd; }
            set 
            { 
                Data.saturdayEnd = value;
                OnPropertyChanged("SaturdayEnd");
            }
        }

        #endregion
    }
}
