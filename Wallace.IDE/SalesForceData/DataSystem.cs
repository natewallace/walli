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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// The data system.
    /// </summary>
    public class DataSystem
    {
        #region Fields

        /// <summary>
        /// The client for the checkout table.
        /// </summary>
        private SalesForceClient _client;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The client to use.</param>
        internal DataSystem(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _client = client;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get global description of objects.
        /// </summary>
        /// <returns>The SObject types in the org.</returns>
        public SObjectTypePartial[] DescribeGlobal()
        {
            SalesForceAPI.Partner.describeGlobalResponse response = _client.PartnerClient.describeGlobal(new SalesForceAPI.Partner.describeGlobalRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                null,
                null));

            List<SObjectTypePartial> result = new List<SObjectTypePartial>();
            if (response != null && response.result != null)
                foreach (SalesForceAPI.Partner.DescribeGlobalSObjectResult r in response.result.sobjects)
                    result.Add(new SObjectTypePartial(r));

            return result.ToArray();
        }

        /// <summary>
        /// Get more detailed description of the object type.
        /// </summary>
        /// <param name="objectType">The object type to get a more detailed description of.</param>
        /// <returns>The full details for the given object.</returns>
        public SObjectType DescribeObjectType(SObjectTypePartial objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            return DescribeObjectType(objectType.Name);
        }

        /// <summary>
        /// Get more detailed description of the object type.
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to get a more detailed description of.</param>
        /// <returns>The full details for the given object.</returns>
        public SObjectType DescribeObjectType(string objectTypeName)
        {
            if (objectTypeName == null)
                throw new ArgumentNullException("objectTypeName");

            SalesForceAPI.Partner.describeSObjectResponse response = _client.PartnerClient.describeSObject(new SalesForceAPI.Partner.describeSObjectRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                null,
                null,
                null,
                objectTypeName));

            if (response != null && response.result != null)
                return new SObjectType(response.result);

            throw new Exception("Couldn't get details for the given object: " + objectTypeName);
        }  

        /// <summary>
        /// Execute a select query on the server.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult Select(string query)
        {
            return Select(query, false);
        }

        /// <summary>
        /// Execute a select query on the server.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult Select(string query, bool includeStructure)
        {
            SalesForceAPI.Partner.queryResponse response = _client.PartnerClient.query(new SalesForceAPI.Partner.queryRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                null,
                null,
                null,
                null,
                query));

            return new DataSelectResult(
                Convert(response.result.records, includeStructure),
                !response.result.done,
                response.result.queryLocator,
                response.result.size,
                0);
        }

        /// <summary>
        /// Execute a select query on the server to get the next batch of data results.
        /// </summary>
        /// <param name="previousResult">The last batch of data results that were received.</param>
        /// <returns>The next batch of data results that follow the given previous data results.</returns>
        public DataSelectResult Select(DataSelectResult previousResult)
        {
            return Select(previousResult, false);
        }

        /// <summary>
        /// Execute a select query on the server to get the next batch of data results.
        /// </summary>
        /// <param name="previousResult">The last batch of data results that were received.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The next batch of data results that follow the given previous data results.</returns>
        public DataSelectResult Select(DataSelectResult previousResult, bool includeStructure)
        {
            SalesForceAPI.Partner.queryMoreResponse response = _client.PartnerClient.queryMore(new SalesForceAPI.Partner.queryMoreRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                null,
                null,
                previousResult.QueryLocator));

            return new DataSelectResult(
                Convert(response.result.records, includeStructure),
                !response.result.done,
                response.result.queryLocator,
                previousResult.Size,
                previousResult.Index + previousResult.Data.Rows.Count);
        }

        /// <summary>
        /// Execute a select query on the server.  This method returns all records instead of 
        /// chunks of 200 records.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult SelectAll(string query)
        {
            return SelectAll(query, false);
        }

        /// <summary>
        /// Execute a select query on the server.  This method returns all records instead of 
        /// chunks of 200 records.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult SelectAll(string query, bool includeStructure)
        {
            DataSelectResult result = Select(query, includeStructure);
            DataTable data = result.Data;

            while (result.IsMore)
            {
                result = Select(result);
                foreach (DataRow row in result.Data.Rows)
                    data.ImportRow(row);
            }

            data.AcceptChanges();

            return new DataSelectResult(data, false, null, data.Rows.Count, 0);
        }

        /// <summary>
        /// Delete the given records.
        /// </summary>
        /// <param name="data">The records to delete.</param>
        public void Delete(DataTable data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            ValidateDataHasId(data);

            List<string> ids = new List<string>();
            foreach (DataRow row in data.Rows)
                ids.Add(row["Id", DataRowVersion.Original] as string);

            SalesForceAPI.Partner.deleteResponse response = _client.PartnerClient.delete(new SalesForceAPI.Partner.deleteRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                ids.ToArray()));

            if (response.result != null)
            {
                foreach (SalesForceAPI.Partner.DeleteResult entry in response.result)
                {
                    if (!entry.success)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Failed to delete " + entry.id);
                        if (entry.errors != null)
                            foreach (SalesForceAPI.Partner.Error err in entry.errors)
                                sb.AppendLine(err.message);

                        throw new Exception(sb.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Update the given records.
        /// </summary>
        /// <param name="data">The data to update.</param>
        public void Update(DataTable data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            ValidateDataHasId(data);

            SalesForceAPI.Partner.sObject[] records = Convert(data, true);
            for (int i = 0; i < records.Length; i += 200)
            {
                SalesForceAPI.Partner.sObject[] buf = new SalesForceAPI.Partner.sObject[Math.Min(200, records.Length - i)];
                Array.Copy(records, i, buf, 0, buf.Length);

                SalesForceAPI.Partner.updateResponse response = _client.PartnerClient.update(new SalesForceAPI.Partner.updateRequest(
                    new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    buf));

                if (response.result != null)
                {
                    foreach (SalesForceAPI.Partner.SaveResult entry in response.result)
                    {
                        if (!entry.success)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Failed to save " + entry.id);
                            if (entry.errors != null)
                                foreach (SalesForceAPI.Partner.Error err in entry.errors)
                                    sb.AppendLine(err.message);

                            throw new Exception(sb.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Insert the given records.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <remarks>The data passed in will be updated with the id of the newly inserted records.</remarks>
        public void Insert(DataTable data)
        {
            SalesForceAPI.Partner.sObject[] records = Convert(data, true);
            for (int i = 0; i < records.Length; i += 200)
            {
                SalesForceAPI.Partner.sObject[] buf = new SalesForceAPI.Partner.sObject[Math.Min(200, records.Length - i)];
                Array.Copy(records, i, buf, 0, buf.Length);

                SalesForceAPI.Partner.createResponse response = _client.PartnerClient.create(new SalesForceAPI.Partner.createRequest(
                    new SalesForceAPI.Partner.SessionHeader() { sessionId = _client.SessionId },
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    buf));

                if (!data.Columns.Contains("Id"))
                    data.Columns.Add("Id");

                data.Columns["Id"].ReadOnly = false;

                try
                {
                    if (response.result != null && response.result.Length == data.Rows.Count)
                    {
                        for (int j = 0; j < response.result.Length; j++)
                        {
                            if (!response.result[j].success)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Failed to save " + response.result[j].id);
                                if (response.result[j].errors != null)
                                    foreach (SalesForceAPI.Partner.Error err in response.result[j].errors)
                                        sb.AppendLine(err.message);

                                throw new Exception(sb.ToString());
                            }
                            else
                            {
                                data.Rows[j + i]["Id"] = response.result[j].id;
                            }
                        }
                    }
                }
                finally
                {
                    data.Columns["Id"].ReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Validate that the data has an Id column.  If not an exception is thrown.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        private void ValidateDataHasId(DataTable data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            foreach (DataColumn column in data.Columns)
                if (String.Compare(column.ColumnName, "Id", true) == 0)
                    return;

            throw new Exception("There is no Id column in the data.");
        }

        /// <summary>
        /// Converts the collection of SObjects into a data table.
        /// </summary>
        /// <param name="records">The records to convert.  All of them need to be of the same type.</param>
        /// <returns>The data table which holds all of the records.</returns>
        private DataTable Convert(IEnumerable<SalesForceAPI.Partner.sObject> records)
        {
            return Convert(records, false);
        }

        /// <summary>
        /// Converts the collection of SObjects into a data table.
        /// </summary>
        /// <param name="records">The records to convert.  All of them need to be of the same type.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The data table which holds all of the records.</returns>
        private DataTable Convert(IEnumerable<SalesForceAPI.Partner.sObject> records, bool includeStructure)
        {
            if (records == null || records.Count() == 0)
                return new DataTable();

            // setup the table schema
            SalesForceAPI.Partner.sObject first = records.First();

            SObjectType objectType = null;
            if (includeStructure)
                objectType = DescribeObjectType(first.type);

            DataTable dt = new DataTable(first.type);
            if (first.Any != null)
            {
                foreach (System.Xml.XmlElement e in first.Any)
                {
                    DataColumn column = dt.Columns.Add(e.LocalName);
                    if (objectType != null)
                    {
                        SObjectFieldType field = objectType.Fields
                            .Where(f => String.Compare(f.Name, column.ColumnName, true) == 0)
                            .FirstOrDefault();

                        if (field != null)
                        {
                            column.ReadOnly = !field.Updateable;
                        }
                    }
                }
            }

            // convert the rows
            foreach (SalesForceAPI.Partner.sObject record in records)
            {
                if (record == null)
                    throw new ArgumentException("records contains a null entry.", "records");

                DataRow row = dt.NewRow();
                if (record.Any != null)
                {
                    foreach (System.Xml.XmlElement e in record.Any)
                    {
                        if (record.fieldsToNull != null && record.fieldsToNull.Contains(e.LocalName))
                            row[e.LocalName] = null;
                        else
                            row[e.LocalName] = e.InnerText;
                    }
                }

                dt.Rows.Add(row);
            }

            dt.AcceptChanges();

            return dt;
        }

        /// <summary>
        /// Convert the given data into a collection of SObjects.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>The SObjects built from the data table.</returns>
        private SalesForceAPI.Partner.sObject[] Convert(DataTable data)
        {
            return Convert(data, false);
        }

        /// <summary>
        /// Convert the given data into a collection of SObjects.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="excludeReadOnlyFields">
        /// When true, any fields that are readonly will not be included in the converted
        /// results with the exception of the id field.
        /// </param>
        /// <returns>The SObjects built from the data table.</returns>
        private SalesForceAPI.Partner.sObject[] Convert(DataTable data, bool excludeReadOnlyFields)
        {
            List<SalesForceAPI.Partner.sObject> result = new List<SalesForceAPI.Partner.sObject>();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            if (data == null)
                return result.ToArray();

            foreach (DataRow row in data.Rows)
            {
                SalesForceAPI.Partner.sObject record = new SalesForceAPI.Partner.sObject();
                record.type = data.TableName;

                List<System.Xml.XmlElement> any = new List<System.Xml.XmlElement>();
                List<string> nullFields = new List<string>();

                foreach (DataColumn column in data.Columns)
                {
                    if (excludeReadOnlyFields &&
                        column.ReadOnly &&
                        String.Compare(column.ColumnName, "Id", 0) != 0)
                        continue;

                    System.Xml.XmlElement e = doc.CreateElement(column.ColumnName);

                    if (row[column] != null && row[column].GetType() == typeof(DateTime))
                        e.InnerText = ((DateTime)row[column]).ToString("s");
                    else
                        e.InnerText = System.Convert.ToString(row[column]);

                    // don't put Id field in null list
                    if (String.Compare(column.ColumnName, "Id", 0) == 0 &&
                        String.IsNullOrWhiteSpace(e.InnerText))
                        continue;

                    if (e.InnerText == null || e.InnerText == String.Empty)
                        nullFields.Add(column.ColumnName);
                    else
                        any.Add(e);
                }

                record.Any = any.ToArray();
                record.fieldsToNull = nullFields.ToArray();

                result.Add(record);
            }

            return result.ToArray();
        }

        #endregion
    }
}
