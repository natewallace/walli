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

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for ProfileEditorControl.xaml
    /// </summary>
    public partial class ProfileEditorControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Supports the Profile property.
        /// </summary>
        private ProfileData _profile;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProfileEditorControl()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">Profile.</param>
        public ProfileEditorControl(ProfileData profile)
        {
            InitializeComponent();
            Profile = profile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The profile being edited.
        /// </summary>
        public ProfileData Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
                if (_profile != null)
                {
                    dataGridApplication.ItemsSource = _profile.ApplicationVisibilities;
                    dataGridClass.ItemsSource = _profile.ClassAccess;
                    dataGridExternal.ItemsSource = _profile.ExternalDataSourcePermissions;
                    dataGridField.ItemsSource = _profile.FieldPermissions;
                    dataGridLayout.ItemsSource = _profile.LayoutAssignments;
                    //TODO: login
                    dataGridObject.ItemsSource = _profile.ObjectPermissions;
                    dataGridPage.ItemsSource = _profile.PageAccess;
                    dataGridProfile.ItemsSource = _profile.ProfilePermissions;
                    dataGridRecordType.ItemsSource = _profile.RecordTypeVisibilities;
                    dataGridUser.ItemsSource = _profile.UserPermissions;
                    dataGridTab.ItemsSource = _profile.TabVisibilities;
                }
                else
                {
                    dataGridApplication.ItemsSource = null;
                    dataGridClass.ItemsSource = null;
                    dataGridExternal.ItemsSource = null;
                    dataGridField.ItemsSource = null;
                    dataGridLayout.ItemsSource = null;
                    //TODO: login
                    dataGridObject.ItemsSource = null;
                    dataGridPage.ItemsSource = null;
                    dataGridProfile.ItemsSource = null;
                    dataGridRecordType.ItemsSource = null;
                    dataGridUser.ItemsSource = null;
                    dataGridTab.ItemsSource = null;
                }
            }
        }

        #endregion
    }
}
