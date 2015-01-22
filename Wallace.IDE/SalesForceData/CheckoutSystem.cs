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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// The checkout system.
    /// </summary>
    public class CheckoutSystem
    {
        #region Fields

        /// <summary>
        /// The client for the checktout table.
        /// </summary>
        private SalesForceClient _client;

        /// <summary>
        /// Used to cache the last call to IsCheckoutEnabled.
        /// </summary>
        private bool? _isCheckoutEnabled = null;

        /// <summary>
        /// Supports the TableName property.
        /// </summary>
        private string _tableName = null;

        /// <summary>
        /// Supports the ColumnEntityId property.
        /// </summary>
        private string _columnEntityId = null;

        /// <summary>
        /// Supports the ColumnEntityName property.
        /// </summary>
        private string _columnEntityName = null;

        /// <summary>
        /// Supports the ColumnEntityTypeName property.
        /// </summary>
        private string _columnEntityTypeName = null;

        /// <summary>
        /// Supports the ColumnUserId property.
        /// </summary>
        private string _columnUserId = null;

        /// <summary>
        /// Supports the ColumnUserName property.
        /// </summary>
        private string _columnUserName = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The client to use for checkouts.</param>
        internal CheckoutSystem(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _client = client;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The table name.
        /// </summary>
        private string TableName
        {
            get
            {
                if (_tableName == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _tableName = String.Format("{0}__Walli_Lock_Table__c", _client.Namespace);
                    else
                        _tableName = "Walli_Lock_Table__c";
                }

                return _tableName;
            }
        }

        /// <summary>
        /// The column entity id.
        /// </summary>
        private string ColumnEntityId
        {
            get
            {
                if (_columnEntityId == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _columnEntityId = String.Format("{0}__Entity_Id__c", _client.Namespace);
                    else
                        _columnEntityId = "Entity_Id__c";
                }

                return _columnEntityId;
            }
        }

        /// <summary>
        /// The column entity name.
        /// </summary>
        private string ColumnEntityName
        {
            get
            {
                if (_columnEntityName == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _columnEntityName = String.Format("{0}__Entity_Name__c", _client.Namespace);
                    else
                        _columnEntityName = "Entity_Name__c";
                }

                return _columnEntityName;
            }
        }

        /// <summary>
        /// The column entity type name.
        /// </summary>
        private string ColumnEntityTypeName
        {
            get
            {
                if (_columnEntityTypeName == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _columnEntityTypeName = String.Format("{0}__Entity_Type_Name__c", _client.Namespace);
                    else
                        _columnEntityTypeName = "Entity_Type_Name__c";
                }

                return _columnEntityTypeName;
            }
        }

        /// <summary>
        /// The column user id.
        /// </summary>
        private string ColumnUserId
        {
            get
            {
                if (_columnUserId == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _columnUserId = String.Format("{0}__User_Id__c", _client.Namespace);
                    else
                        _columnUserId = "User_Id__c";
                }

                return _columnUserId;
            }
        }

        /// <summary>
        /// The column user name.
        /// </summary>
        private string ColumnUserName
        {
            get
            {
                if (_columnUserName == null)
                {
                    if (!String.IsNullOrWhiteSpace(_client.Namespace))
                        _columnUserName = String.Format("{0}__User_Name__c", _client.Namespace);
                    else
                        _columnUserName = "User_Name__c";
                }

                return _columnUserName;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check to see if the checkout system is enabled.
        /// </summary>
        /// <param name="refresh">If true the value is refreshed from the server, otherwise a cached value is returned.</param>
        /// <returns>true if checkouts are enabled, false if they are not.</returns>
        public bool IsEnabled(bool refresh)
        {
            if (refresh || !_isCheckoutEnabled.HasValue)
            {
                _isCheckoutEnabled = false;

                foreach (SObjectTypePartial obj in _client.DataDescribeGlobal())
                {
                    if (obj.Name != null && obj.Name.EndsWith("Walli_Lock_Table__c"))
                    {
                        _isCheckoutEnabled = true;
                        break;
                    }
                }
            }

            return _isCheckoutEnabled.Value;
        }

        /// <summary>
        /// Get the most recent value for IsCheckoutEnabled.
        /// </summary>
        /// <returns>The most recent value for IsCheckoutEnabled.</returns>
        public bool IsEnabled()
        {
            return IsEnabled(false);
        }

        /// <summary>
        /// Enable checkouts for the entire system.
        /// </summary>
        /// <param name="value">true to enable checkouts, false to disable them.</param>
        public void Enable(bool value)
        {
            byte[] package = null;
            if (value)
            {
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SalesForceData.Resources.WalliLockTableCreate.zip"))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    package = reader.ReadBytes((int)stream.Length);
                }
            }
            else
            {
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SalesForceData.Resources.WalliLockTableDelete.zip"))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    package = reader.ReadBytes((int)stream.Length);
                }
            }

            string id = _client.DeployPackage(package, false, false);
            bool complete = false;
            while (!complete)
            {
                PackageDeployResult result = _client.CheckPackageDeploy(id);
                if (result.Status == PackageDeployResultStatus.Failed)
                    throw new Exception("Could not change checkout system: " + result.ResultMessage);

                complete = result.DeploymentComplete;
            }

            _isCheckoutEnabled = value;
        }

        /// <summary>
        /// Create a new data table for checkouts.
        /// </summary>
        /// <returns>The newly created table.</returns>
        private DataTable CreateTable()
        {
            DataTable table = new DataTable(TableName);
            table.Columns.Add(ColumnEntityId);
            table.Columns.Add(ColumnEntityTypeName);
            table.Columns.Add(ColumnEntityName);
            table.Columns.Add(ColumnUserId);
            table.Columns.Add(ColumnUserName);

            return table;
        }

        /// <summary>
        /// Checkout the given files.
        /// </summary>
        /// <param name="files">The files to checkout.</param>
        public void CheckoutFiles(IEnumerable<SourceFile> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");

            if (!IsEnabled())
                return;

            DataTable table = CreateTable();

            foreach (SourceFile file in files)
            {
                if (String.IsNullOrEmpty(file.Id))
                    throw new ArgumentException("The file doesn't have an id.", "file.Id");

                DataRow row = table.NewRow();
                row[ColumnEntityId] = file.Id;
                row[ColumnEntityName] = file.Name;
                row[ColumnEntityTypeName] = file.FileType.Name;
                row[ColumnUserId] = _client.User.Id;
                row[ColumnUserName] = _client.User.Name;
                table.Rows.Add(row);
            }

            try
            {
                _client.DataInsert(table);
            }
            catch (Exception err)
            {
                throw new Exception("Could not checkout file(s): " + err.Message, err);
            }

            foreach (SourceFile file in files)
                file.CheckedOutBy = _client.User;
        }

        /// <summary>
        /// Checkout the given file.
        /// </summary>
        /// <param name="file">The file to checkout.</param>
        public void CheckoutFile(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            CheckoutFiles(new SourceFile[] { file });
        }

        /// <summary>
        /// Update the checkout status of the given file.
        /// </summary>
        /// <param name="file">The file to update the checkout status for.</param>
        public void RefreshCheckoutStatus(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (String.IsNullOrEmpty(file.Id))
                throw new ArgumentException("The file doesn't have an id.", "file.Id");

            if (!IsEnabled())
                return;

            DataSelectResult lockTable = _client.DataSelectAll(String.Format("SELECT {0}, {1}, {2} FROM {3} WHERE {0} = '{4}'",
                ColumnEntityId,
                ColumnUserId,
                ColumnUserName,
                TableName,
                file.Id));

            if (lockTable.Data.Rows.Count == 1)
                file.CheckedOutBy = new User(
                    lockTable.Data.Rows[0][ColumnUserId] as string,
                    lockTable.Data.Rows[0][ColumnUserName] as string);
            else
                file.CheckedOutBy = null;
        }

        /// <summary>
        /// Update an existing checkout on a file.
        /// </summary>
        /// <param name="file">The file to update the checkout on.</param>
        public void UpdateCheckout(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (String.IsNullOrEmpty(file.Id))
                throw new ArgumentException("The file doesn't have an id.", "file.Id");

            if (!IsEnabled())
                return;

            DataSelectResult lockTable = _client.DataSelectAll(String.Format("SELECT Id, {0}, {1}, {2}, {3}, {4} FROM {5} WHERE {0} = '{6}' AND {3} = '{7}'",
                ColumnEntityId,
                ColumnEntityName,
                ColumnEntityTypeName,
                ColumnUserId,
                ColumnUserName,
                TableName,
                file.Id,
                _client.User.Id));

            if (lockTable.Data.Rows.Count != 1)
                return; // file isn't checked out to the current user

            lockTable.Data.Rows[0][ColumnEntityTypeName] = file.FileType.Name;
            lockTable.Data.Rows[0][ColumnEntityName] = file.Name;
            lockTable.Data.Rows[0][ColumnUserName] = _client.User.Name;

            try
            {
                _client.DataUpdate(lockTable.Data);
            }
            catch (Exception err)
            {
                throw new Exception("Could not update checkout file: " + err.Message, err);
            }
        }

        /// <summary>
        /// Check in the given files.
        /// </summary>
        /// <param name="files">The files to checkin.</param>
        public void CheckinFiles(IEnumerable<SourceFile> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");

            if (!IsEnabled())
                return;

            List<string> ids = new List<string>();
            foreach (SourceFile file in files)
                ids.Add(file.Id);

            DataSelectResult result = _client.DataSelectAll(String.Format("SELECT Id FROM {0} WHERE {1} IN ('{2}')",
                TableName,
                ColumnEntityId,
                String.Join("','", ids)));

            try
            {
                _client.DataDelete(result.Data);

                foreach (SourceFile file in files)
                    file.CheckedOutBy = null;
            }
            catch (Exception err)
            {
                throw new Exception("Could not checkin file: " + err.Message, err);
            }
        }

        /// <summary>
        /// Check in the given file.
        /// </summary>
        /// <param name="file">The file to checkin.</param>
        public void CheckinFile(SourceFile file)
        {
            CheckinFiles(new SourceFile[] { file });
        }

        /// <summary>
        /// Get the checkout table entries.
        /// </summary>
        /// <returns>The checkout table entries with the file id as a key.</returns>
        public IDictionary<string, SourceFile> GetCheckouts()
        {
            Dictionary<string, SourceFile> result = new Dictionary<string, SourceFile>();
            if (!IsEnabled())
                return result;

            DataSelectResult lockTable = _client.DataSelectAll(String.Format("SELECT {0}, {1}, {2}, {3}, {4} FROM {5}",
                ColumnEntityId,
                ColumnEntityName,
                ColumnEntityTypeName,
                ColumnUserId,
                ColumnUserName,
                TableName));
            foreach (DataRow row in lockTable.Data.Rows)
            {
                SourceFile file = new SourceFile(
                    row[ColumnEntityId] as string,
                    row[ColumnEntityTypeName] as string,
                    row[ColumnEntityName] as string,
                    new User(
                        row[ColumnUserId] as string,
                        row[ColumnUserName] as string));

                result.Add(file.Id, file);
            }

            return result;
        } 

        #endregion
    }
}
