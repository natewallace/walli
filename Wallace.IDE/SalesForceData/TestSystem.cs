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
    /// The test system.
    /// </summary>
    public class TestSystem
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
        internal TestSystem(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _client = client;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the code coverage for the entire org.
        /// </summary>
        /// <returns>The code coverage for the entire org.</returns>
        public CodeCoverage[] GetCodeCoverage()
        {
            // get all apex classes and triggers
            SourceFile[] files = _client.Meta.GetSourceFiles(
                new SourceFileType[] { new SourceFileType("ApexClass", null), new SourceFileType("ApexTrigger", null) },
                false);
            Dictionary<string, SourceFile> fileMap = new Dictionary<string, SourceFile>();
            foreach (SourceFile file in files)
                if (!String.IsNullOrWhiteSpace(file.Id))
                    fileMap.Add(file.Id, file);

            // get the code coverage
            SalesForceAPI.Tooling.queryResponse response = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                "SELECT ApexClassorTriggerId, Coverage FROM ApexCodeCoverageAggregate"));

            List<CodeCoverage> result = new List<CodeCoverage>();
            if (response != null && response.result != null && response.result.records != null)
            {
                foreach (SalesForceAPI.Tooling.sObject obj in response.result.records)
                {
                    SalesForceAPI.Tooling.ApexCodeCoverageAggregate coverage = obj as SalesForceAPI.Tooling.ApexCodeCoverageAggregate;
                    if (coverage != null)
                    {
                        if (!String.IsNullOrWhiteSpace(coverage.ApexClassOrTriggerId) &&
                            fileMap.ContainsKey(coverage.ApexClassOrTriggerId))
                        {
                            CodeCoverage cc = new CodeCoverage(fileMap[coverage.ApexClassOrTriggerId], coverage);
                            if (cc.LinesUncovered.Length != 0 || cc.LinesCovered.Length != 0)
                                result.Add(cc);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get the overall code coverage percent.
        /// </summary>
        /// <returns>The overall code coverage percent.</returns>
        public int GetOverallCodeCoveragePercent()
        {
            // get the code coverage
            SalesForceAPI.Tooling.queryResponse response = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                "SELECT PercentCovered FROM ApexOrgWideCoverage"));

            if (response != null && 
                response.result != null && 
                response.result.records != null &&
                response.result.records.Length == 1)
            {
                SalesForceAPI.Tooling.ApexOrgWideCoverage coverage = response.result.records[0] as SalesForceAPI.Tooling.ApexOrgWideCoverage;
                if (coverage != null && coverage.PercentCovered.HasValue)
                    return coverage.PercentCovered.Value;
            }

            return 0;
        }

        /// <summary>
        /// Start tests for a given class.
        /// </summary>
        /// <param name="names">The names of the classes to start tests for.</param>
        /// <returns>The started test run.</returns>
        public TestRun StartTests(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException("names");

            // get class ids
            StringBuilder fileNameBuilder = new StringBuilder();
            foreach (string name in names)
                fileNameBuilder.AppendFormat("'{0}',", name);
            fileNameBuilder.Length--;

            DataSelectResult classIdData = _client.Data.SelectAll(
                String.Format("SELECT Id, Name FROM ApexClass WHERE Name IN ({0})", fileNameBuilder.ToString()));

            // submit test run items
            Dictionary<string, string> apexClassNameMap = new Dictionary<string, string>();
            DataTable dt = new DataTable("ApexTestQueueItem");
            dt.Columns.Add("ApexClassId");

            foreach (DataRow row in classIdData.Data.Rows)
            {
                DataRow testRunRow = dt.NewRow();
                testRunRow["ApexClassId"] = row["Id"];
                dt.Rows.Add(testRunRow);

                apexClassNameMap.Add(row["Id"] as string, row["Name"] as string);
            }

            _client.Data.Insert(dt);

            // create the test run object
            List<TestRunItem> items = new List<TestRunItem>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new TestRunItem(
                    apexClassNameMap[row["ApexClassId"] as string],
                    row["ApexClassId"] as string));
            }

            DataSelectResult jobIdData = _client.Data.Select(
                String.Format("SELECT ParentJobId FROM ApexTestQueueItem WHERE Id = '{0}'", dt.Rows[0]["Id"]));

            return new TestRun(jobIdData.Data.Rows[0]["ParentJobId"] as string, items.ToArray());
        }

        /// <summary>
        /// Update the test run with the most recent status.
        /// </summary>
        /// <param name="testRun">The test run to update.</param>
        public void UpdateTests(TestRun testRun)
        {
            // filter out completed test runs
            Dictionary<string, TestRunItem> itemsMap = new Dictionary<string, TestRunItem>();
            StringBuilder classIdBuilder = new StringBuilder();
            foreach (TestRunItem item in testRun.Items)
            {
                if (!item.IsDone)
                {
                    itemsMap.Add(item.ApexClassId, item);
                    classIdBuilder.AppendFormat("'{0}',", item.ApexClassId);
                }
            }

            // get updated status
            StringBuilder completedItemsBuilder = new StringBuilder();
            if (itemsMap.Count > 0)
            {
                classIdBuilder.Length--;
                DataSelectResult testRunData = _client.Data.SelectAll(String.Format(
                    "SELECT ApexClassId, Status, ExtendedStatus FROM ApexTestQueueItem WHERE ParentJobId = '{0}' AND ApexClassId IN ({1})",
                    testRun.JobId,
                    classIdBuilder.ToString()));

                foreach (DataRow row in testRunData.Data.Rows)
                {
                    TestRunItem item = itemsMap[row["ApexClassId"] as string];
                    item.Status = (TestRunItemStatus)Enum.Parse(typeof(TestRunItemStatus), row["Status"] as string);
                    item.ExtendedStatus = row["ExtendedStatus"] as string;

                    if (item.IsDone)
                        completedItemsBuilder.AppendFormat("'{0}',", item.ApexClassId);
                }
            }

            // get details for items that were completed
            if (completedItemsBuilder.Length > 0)
            {
                completedItemsBuilder.Length--;

                DataSelectResult completedData = _client.Data.SelectAll(
                    String.Format("SELECT ApexClassId, ApexLogId, Message, MethodName, Outcome, StackTrace FROM ApexTestResult WHERE AsyncApexJobId = '{0}' AND ApexClassId IN ({1})",
                                  testRun.JobId,
                                  completedItemsBuilder.ToString()));

                Dictionary<string, List<TestRunItemResult>> resultsMap = new Dictionary<string, List<TestRunItemResult>>();
                foreach (DataRow row in completedData.Data.Rows)
                {
                    List<TestRunItemResult> resultList = null;
                    string classId = row["ApexClassId"] as string;
                    if (resultsMap.ContainsKey(classId))
                    {
                        resultList = resultsMap[classId];
                    }
                    else
                    {
                        resultList = new List<TestRunItemResult>();
                        resultsMap.Add(classId, resultList);
                    }

                    resultList.Add(new TestRunItemResult(
                        row["Message"] as string,
                        row["MethodName"] as string,
                        (TestRunItemResultStatus)Enum.Parse(typeof(TestRunItemResultStatus), row["Outcome"] as string),
                        row["StackTrace"] as string));
                }

                foreach (KeyValuePair<string, List<TestRunItemResult>> kvp in resultsMap)
                    itemsMap[kvp.Key].Results = kvp.Value.OrderBy(t => t.MethodName).ToArray();
            }

            if (testRun.IsDone)
                testRun.Finished = DateTime.Now;
        }

        #endregion
    }
}
