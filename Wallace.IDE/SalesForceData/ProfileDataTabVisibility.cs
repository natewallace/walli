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
    /// Tab visibility.
    /// </summary>
    public class ProfileDataTabVisibility : SourceFileDataElement<SalesForceAPI.Metadata.ProfileTabVisibility>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The object to build this object from.</param>
        internal ProfileDataTabVisibility(SalesForceAPI.Metadata.ProfileTabVisibility data)
            : base(data)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The tab name.
        /// </summary>
        public string TabName
        {
            get { return Data.tab; }
            set { Data.tab = value; }
        }

        /// <summary>
        /// The visibility of the tab.
        /// </summary>
        public ProfileDataTabVisibilityValue Visibility
        {
            get 
            {
                switch (Data.visibility)
                {
                    case SalesForceAPI.Metadata.TabVisibility.DefaultOn:
                        return ProfileDataTabVisibilityValue.DefaultOn;

                    case SalesForceAPI.Metadata.TabVisibility.Hidden:
                        return ProfileDataTabVisibilityValue.Hidden;

                    default:
                        return ProfileDataTabVisibilityValue.DefaultOff;
                }
            }
            set 
            {
                switch (value)
                {
                    case ProfileDataTabVisibilityValue.DefaultOn:
                        Data.visibility = SalesForceAPI.Metadata.TabVisibility.DefaultOn;
                        break;

                    case ProfileDataTabVisibilityValue.Hidden:
                        Data.visibility = SalesForceAPI.Metadata.TabVisibility.Hidden;
                        break;

                    default:
                        Data.visibility = SalesForceAPI.Metadata.TabVisibility.DefaultOff;
                        break;
                }
            }
        }

        #endregion
    }
}
