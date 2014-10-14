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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SalesForceData
{
    /// <summary>
    /// Data for a profile.
    /// </summary>
    public class ProfileData : SourceFileData
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The profile to build this object from.</param>
        internal ProfileData(SalesForceAPI.Metadata.Profile profile)
            : base(profile.fullName)
        {
            Data = profile;
            InitData();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The underlying salesforce object.
        /// </summary>
        internal SalesForceAPI.Metadata.Profile Data { get; private set; }

        /// <summary>
        /// Indicates if the profile is custom.
        /// </summary>
        public bool IsCustom
        {
            get { return Data.custom; }
            set
            {
                Data.custom = value;
                Data.customSpecified = true;
            }
        }

        /// <summary>
        /// The profile description.
        /// </summary>
        public string Description
        {
            get { return Data.description; }
            set { Data.description = value; }
        }

        /// <summary>
        /// The user license for the profile.
        /// </summary>
        public string UserLicense
        {
            get { return Data.userLicense; }
            set { Data.userLicense = value; }
        }

        /// <summary>
        /// The login hours.
        /// </summary>
        public ProfileDataLoginHours LoginHours { get; private set; }

        /// <summary>
        /// The login ip ranges.
        /// </summary>
        public IList<ProfileDataLoginIPRange> LoginIPRanges { get; private set; }

        /// <summary>
        /// Application visibilities.
        /// </summary>
        public IList<ProfileDataAppVisibility> ApplicationVisibilities { get; private set; }

        /// <summary>
        /// Class access.
        /// </summary>
        public IList<ProfileDataClassAccess> ClassAccess { get; private set; }

        /// <summary>
        /// External data source permissions.
        /// </summary>
        public IList<ProfileDataExternalDataSourcePermission> ExternalDataSourcePermissions { get; private set; }

        /// <summary>
        /// Field permissions.
        /// </summary>
        public IList<ProfileDataFieldPermission> FieldPermissions { get; private set; }

        /// <summary>
        /// Layout assignments.
        /// </summary>
        public IList<ProfileDataLayoutAssignment> LayoutAssignments { get; private set; }

        /// <summary>
        /// Object permissions.
        /// </summary>
        public IList<ProfileDataObjectPermission> ObjectPermissions { get; private set; }

        /// <summary>
        /// Page access.
        /// </summary>
        public IList<ProfileDataPageAccess> PageAccess { get; private set; }

        /// <summary>
        /// Profile permissions.
        /// </summary>
        public IList<ProfileDataCustomPermission> ProfilePermissions { get; private set; }

        /// <summary>
        /// Record type visibilities.
        /// </summary>
        public IList<ProfileDataRecordTypeVisibility> RecordTypeVisibilities { get; private set; }

        /// <summary>
        /// Tab visibilities.
        /// </summary>
        public IList<ProfileDataTabVisibility> TabVisibilities { get; private set; }

        /// <summary>
        /// User permissions.
        /// </summary>
        public IList<ProfileDataUserPermission> UserPermissions { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize this object with the current Data.
        /// </summary>
        private void InitData()
        {
            if (Data == null)
                throw new Exception("Data is not set.");

            LoginHours = (Data.loginHours == null) ? new ProfileDataLoginHours() : new ProfileDataLoginHours(Data.loginHours);

            LoginIPRanges = new List<ProfileDataLoginIPRange>();
            if (Data.loginIpRanges != null)
                foreach (SalesForceAPI.Metadata.ProfileLoginIpRange plir in Data.loginIpRanges)
                    LoginIPRanges.Add(new ProfileDataLoginIPRange(plir));

            ApplicationVisibilities = new List<ProfileDataAppVisibility>();
            if (Data.applicationVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileApplicationVisibility pav in Data.applicationVisibilities)
                    ApplicationVisibilities.Add(new ProfileDataAppVisibility(pav));

            ClassAccess = new List<ProfileDataClassAccess>();
            if (Data.classAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileApexClassAccess paca in Data.classAccesses)
                    ClassAccess.Add(new ProfileDataClassAccess(paca));

            ExternalDataSourcePermissions = new List<ProfileDataExternalDataSourcePermission>();
            if (Data.externalDataSourceAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileExternalDataSourceAccess pedsa in Data.externalDataSourceAccesses)
                    ExternalDataSourcePermissions.Add(new ProfileDataExternalDataSourcePermission(pedsa));

            FieldPermissions = new List<ProfileDataFieldPermission>();
            if (Data.fieldPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileFieldLevelSecurity pfls in Data.fieldPermissions)
                    FieldPermissions.Add(new ProfileDataFieldPermission(pfls));

            LayoutAssignments = new List<ProfileDataLayoutAssignment>();
            if (Data.layoutAssignments != null)
                foreach (SalesForceAPI.Metadata.ProfileLayoutAssignment pla in Data.layoutAssignments)
                    LayoutAssignments.Add(new ProfileDataLayoutAssignment(pla));

            ObjectPermissions = new List<ProfileDataObjectPermission>();
            if (Data.objectPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileObjectPermissions pop in Data.objectPermissions)
                    ObjectPermissions.Add(new ProfileDataObjectPermission(pop));

            PageAccess = new List<ProfileDataPageAccess>();
            if (Data.pageAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileApexPageAccess papa in Data.pageAccesses)
                    PageAccess.Add(new ProfileDataPageAccess(papa));

            ProfilePermissions = new List<ProfileDataCustomPermission>();
            if (Data.customPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileCustomPermissions pcp in Data.customPermissions)
                    ProfilePermissions.Add(new ProfileDataCustomPermission(pcp));

            RecordTypeVisibilities = new List<ProfileDataRecordTypeVisibility>();
            if (Data.recordTypeVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileRecordTypeVisibility prtv in Data.recordTypeVisibilities)
                    RecordTypeVisibilities.Add(new ProfileDataRecordTypeVisibility(prtv));

            TabVisibilities = new List<ProfileDataTabVisibility>();
            if (Data.tabVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileTabVisibility ptv in Data.tabVisibilities)
                    TabVisibilities.Add(new ProfileDataTabVisibility(ptv));

            UserPermissions = new List<ProfileDataUserPermission>();
            if (Data.userPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileUserPermission pup in Data.userPermissions)
                    UserPermissions.Add(new ProfileDataUserPermission(pup));
        }

        /// <summary>
        /// Gets the underlying salesforce object.
        /// </summary>
        /// <returns>The underlying salesforce object.</returns>
        internal override SalesForceAPI.Metadata.Metadata GetMetadata()
        {
            return Data;
        }

        /// <summary>
        /// Write the data out to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public override void WriteToStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            XmlSerializer ser = new XmlSerializer(typeof(SalesForceAPI.Metadata.Profile));
            ser.Serialize(stream, Data);
        }

        /// <summary>
        /// Read the data in from the given stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public override void ReadFromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            XmlSerializer ser = new XmlSerializer(typeof(SalesForceAPI.Metadata.Profile));
            Data = ser.Deserialize(stream) as SalesForceAPI.Metadata.Profile;
            if (Data == null)
                throw new Exception("Could not deserialize stream.");

            InitData();
            Data.fullName = Name;
        }

        #endregion
    }
}
