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
    /// Object permission settings.
    /// </summary>
    public class ProfileDataObjectPermission : SourceFileDataElement<SalesForceAPI.Metadata.ProfileObjectPermissions>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Data.</param>
        internal ProfileDataObjectPermission(SalesForceAPI.Metadata.ProfileObjectPermissions data)
            : base(data)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the object these permissions are for.
        /// </summary>
        [DisplayName("Object")]
        public string ObjectName
        {
            get { return Data.@object; }
        }

        /// <summary>
        /// Allow creation of records.
        /// </summary>
        [DisplayName("Create")]
        public bool AllowCreate
        {
            get { return Data.allowCreate; }
            set 
            { 
                Data.allowCreate = value;
                Data.allowCreateSpecified = true;
                OnPropertyChanged("AllowCreate");
            }
        }

        /// <summary>
        /// Allow deletion of records.
        /// </summary>
        [DisplayName("Delete")]
        public bool AllowDelete
        {
            get { return Data.allowDelete; }
            set 
            { 
                Data.allowDelete = value;
                Data.allowDeleteSpecified = true;
                OnPropertyChanged("AllowDelete");
            }
        }

        /// <summary>
        /// Allow editing of records.
        /// </summary>
        [DisplayName("Edit")]
        public bool AllowEdit
        {
            get { return Data.allowEdit; }
            set
            {
                Data.allowEdit = value;
                Data.allowEditSpecified = true;
                OnPropertyChanged("AllowEdit");
            }
        }

        /// <summary>
        /// Allow reading records.
        /// </summary>
        [DisplayName("Read")]
        public bool AllowRead
        {
            get { return Data.allowRead; }
            set
            {
                Data.allowRead = value;
                Data.allowReadSpecified = true;
                OnPropertyChanged("AllowRead");
            }
        }

        /// <summary>
        /// Allow viewing of all records.
        /// </summary>
        [DisplayName("View All")]
        public bool AllowViewAllRecords
        {
            get { return Data.viewAllRecords; }
            set
            {
                Data.viewAllRecords = value;
                Data.viewAllRecordsSpecified = true;
                OnPropertyChanged("AllowViewAllRecords");
            }
        }

        /// <summary>
        /// Allow modification to all records.
        /// </summary>
        [DisplayName("Modify All")]
        public bool AllowModifyAllRecords
        {
            get { return Data.modifyAllRecords; }
            set
            {
                Data.modifyAllRecords = value;
                Data.modifyAllRecordsSpecified = true;
                OnPropertyChanged("AllowModifyAllRecords");
            }
        }

        #endregion
    }
}
