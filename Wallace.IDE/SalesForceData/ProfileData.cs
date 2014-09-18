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

            LoginHours = (profile.loginHours == null) ? new ProfileDataLoginHours() : new ProfileDataLoginHours(profile.loginHours);

            LoginIPRanges = new List<ProfileDataLoginIPRange>();
            if (profile.loginIpRanges != null)
                foreach (SalesForceAPI.Metadata.ProfileLoginIpRange plir in profile.loginIpRanges)
                    LoginIPRanges.Add(new ProfileDataLoginIPRange(plir));

            ApplicationVisibilities = new List<ProfileDataAppVisibility>();
            if (profile.applicationVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileApplicationVisibility pav in profile.applicationVisibilities)
                    ApplicationVisibilities.Add(new ProfileDataAppVisibility(pav));

            ClassAccess = new List<ProfileDataClassAccess>();
            if (profile.classAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileApexClassAccess paca in profile.classAccesses)
                    ClassAccess.Add(new ProfileDataClassAccess(paca));

            ExternalDataSourcePermissions = new List<ProfileDataExternalDataSourcePermission>();
            if (profile.externalDataSourceAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileExternalDataSourceAccess pedsa in profile.externalDataSourceAccesses)
                    ExternalDataSourcePermissions.Add(new ProfileDataExternalDataSourcePermission(pedsa));

            FieldPermissions = new List<ProfileDataFieldPermission>();
            if (profile.fieldPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileFieldLevelSecurity pfls in profile.fieldPermissions)
                    FieldPermissions.Add(new ProfileDataFieldPermission(pfls));

            LayoutAssignments = new List<ProfileDataLayoutAssignment>();
            if (profile.layoutAssignments != null)
                foreach (SalesForceAPI.Metadata.ProfileLayoutAssignment pla in profile.layoutAssignments)
                    LayoutAssignments.Add(new ProfileDataLayoutAssignment(pla));

            ObjectPermissions = new List<ProfileDataObjectPermission>();
            if (profile.objectPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileObjectPermissions pop in profile.objectPermissions)
                    ObjectPermissions.Add(new ProfileDataObjectPermission(pop));

            PageAccess = new List<ProfileDataPageAccess>();
            if (profile.pageAccesses != null)
                foreach (SalesForceAPI.Metadata.ProfileApexPageAccess papa in profile.pageAccesses)
                    PageAccess.Add(new ProfileDataPageAccess(papa));

            ProfilePermissions = new List<ProfileDataCustomPermission>();
            if (profile.customPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileCustomPermissions pcp in profile.customPermissions)
                    ProfilePermissions.Add(new ProfileDataCustomPermission(pcp));

            RecordTypeVisibilities = new List<ProfileDataRecordTypeVisibility>();
            if (profile.recordTypeVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileRecordTypeVisibility prtv in profile.recordTypeVisibilities)
                    RecordTypeVisibilities.Add(new ProfileDataRecordTypeVisibility(prtv));

            TabVisibilities = new List<ProfileDataTabVisibility>();
            if (profile.tabVisibilities != null)
                foreach (SalesForceAPI.Metadata.ProfileTabVisibility ptv in profile.tabVisibilities)
                    TabVisibilities.Add(new ProfileDataTabVisibility(ptv));

            UserPermissions = new List<ProfileDataUserPermission>();
            if (profile.userPermissions != null)
                foreach (SalesForceAPI.Metadata.ProfileUserPermission pup in profile.userPermissions)
                    UserPermissions.Add(new ProfileDataUserPermission(pup));
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
        /// Gets the underlying salesforce object.
        /// </summary>
        /// <returns>The underlying salesforce object.</returns>
        internal override SalesForceAPI.Metadata.Metadata GetMetadata()
        {
            return Data;
        }

        #endregion
    }
}
