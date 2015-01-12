﻿/*
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
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// Client used to communicate with SalesForce servers.
    /// </summary>
    public class SalesForceClient : IDisposable
    {
        #region Fields

        /// <summary>
        /// Supports the Session property.
        /// </summary>
        private SalesForceSession _session;

        /// <summary>
        /// If the session was created by this client this will be set to true.
        /// </summary>
        private bool _isSessionOwned;

        /// <summary>
        /// The partner client.
        /// </summary>
        private SalesForceAPI.Partner.Soap _partnerClient;

        /// <summary>
        /// The metadata client.
        /// </summary>
        private SalesForceAPI.Metadata.MetadataPortType _metadataClient;

        /// <summary>
        /// The apex client.
        /// </summary>
        private SalesForceAPI.Apex.ApexPortType _apexClient;

        /// <summary>
        /// The tooling client.
        /// </summary>
        private SalesForceAPI.Tooling.SforceServicePortType _toolingClient;

        /// <summary>
        /// Holds the org info.
        /// </summary>
        private SalesForceAPI.Metadata.DescribeMetadataResult _orgInfo;

        /// <summary>
        /// The minimum version number of metadata retrieved.
        /// </summary>
        public static readonly double METADATA_VERSION = 31.0;

        /// <summary>
        /// The amount of time to wait on a save operation before throwing an exception.
        /// </summary>
        private static readonly TimeSpan SAVE_TIMEOUT = new TimeSpan(0, 1, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="session">The session for this client.</param>
        public SalesForceClient(SalesForceSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _session = session;
            _isSessionOwned = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credential">The credential to use for the client.</param>
        public SalesForceClient(SalesForceCredential credential)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");

            _session = new SalesForceSession(credential, GetDefaultConfiguration());
            _isSessionOwned = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credential">The credential to use for the client.</param>
        /// <param name="configuration">The configuration for the connections.</param>
        public SalesForceClient(SalesForceCredential credential, Configuration configuration)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _session = new SalesForceSession(credential, configuration);
            _isSessionOwned = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a default configuration for the client.
        /// </summary>
        /// <returns>The default configuration for the client.</returns>
        private static Configuration GetDefaultConfiguration()
        {
            string configFile = String.Format("{0}.config", System.Reflection.Assembly.GetExecutingAssembly().Location);
            return ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap() { ExeConfigFilename = configFile },
                ConfigurationUserLevel.None);
        }

        /// <summary>
        /// Open and immediately close a salesforce connection using the credential.  If a failure occurs
        /// an exception is thrown.
        /// </summary>
        /// <param name="credential">The credential to test with.</param>
        public static void TestLogin(SalesForceCredential credential)
        {
            TestLogin(credential, null);
        }

        /// <summary>
        /// Get the user id of the currently logged on user.
        /// </summary>
        /// <returns>The id of the user that is currently logged on.</returns>
        public string GetUserId()
        {
            return _session.UserId;
        }

        /// <summary>
        /// Get the user name of the currently logged on user.
        /// </summary>
        /// <returns>The name of the user that is currently logged on.</returns>
        public string GetUserName()
        {
            return _session.UserName;
        }

        /// <summary>
        /// Open and immediately close a salesforce connection using the credential.  If a failure occurs
        /// an exception is thrown.
        /// </summary>
        /// <param name="credential">The credential to test with.</param>
        /// <param name="configuration">Configuration for the connections.</param>
        public static void TestLogin(SalesForceCredential credential, Configuration configuration)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");

            if (configuration == null)
                configuration = GetDefaultConfiguration();

            SalesForceAPI.Partner.Soap client = null;
            SalesForceSession session = new SalesForceSession(credential, configuration);
            try
            {
                client = session.CreatePartnerClient();
                client.logout(new SalesForceAPI.Partner.logoutRequest(
                    new SalesForceAPI.Partner.SessionHeader() { sessionId = session.Id },
                    null));
                session.DisposeClient(client);
            }
            finally
            {
                if (client != null)
                    session.DisposeClient(client);
            }
        }

        /// <summary>
        /// Initialize all of the clients.
        /// </summary>
        private void InitClients()
        {
            if (_partnerClient == null)
            {
                _partnerClient = _session.CreatePartnerClient();
                _metadataClient = _session.CreateMetadataClient();
                _apexClient = _session.CreateApexClient();
                _toolingClient = _session.CreateToolingClient();
            }
        }

        /// <summary>
        /// Get a uri that a user can use to login to salesforce through their browser using the current session id.
        /// </summary>
        /// <returns>A uri that a user can use to login to salesforce through their browser using the current session id.</returns>
        public string GetWebsiteAutoLoginUri()
        {
            InitClients();
            return _session.WebsiteAutoLoginUri;
        }

        /// <summary>
        /// Get the org info for this client.
        /// </summary>
        /// <returns></returns>
        private SalesForceAPI.Metadata.DescribeMetadataResult GetOrgInfo()
        {
            InitClients();

            if (_orgInfo == null)
            {
                SalesForceAPI.Metadata.describeMetadataRequest request = new SalesForceAPI.Metadata.describeMetadataRequest(
                    new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                    null,
                    METADATA_VERSION);

                SalesForceAPI.Metadata.describeMetadataResponse response = _metadataClient.describeMetadata(request);
                if (response == null || response.result == null)
                    throw new Exception("Could not get organization info.");

                _orgInfo = response.result;
            }

            return _orgInfo;
        }

        /// <summary>
        /// Delete a checkpoint.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to delete.</param>
        public void DeleteCheckpoint(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                throw new ArgumentNullException("checkpoint");

            InitClients();

            SalesForceAPI.Tooling.deleteResponse response = _toolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                new string[] { checkpoint.Id }));

            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    StringBuilder sb = new StringBuilder();
                    if (response.result[0].errors != null)
                        foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                            sb.AppendLine(err.message);

                    throw new Exception("Couldn't delete checkpoint: \r\n" + sb.ToString());
                }
            }
            else
            {
                throw new Exception("Couldn't delete checkpoint: Invalid response received.");
            }
        }

        /// <summary>
        /// Get all of the log parameters that currently exist.
        /// </summary>
        /// <returns>The currently existing log parameters.</returns>
        public LogParameters[] GetLogParameters()
        {
            InitClients();

            SalesForceAPI.Tooling.queryResponse response = _toolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                String.Format("SELECT Id, ApexCode, ApexProfiling, Callout, Database, ExpirationDate, ScopeId, System, TracedEntityId, Validation, Visualforce, Workflow FROM TraceFlag WHERE ExpirationDate > {0}", DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"))));

            List<LogParameters> result = new List<LogParameters>();
            if (response.result.records != null)
            {
                // get entity names
                Dictionary<string, string> names = new Dictionary<string, string>();
                foreach (SalesForceAPI.Tooling.TraceFlag traceFlag in response.result.records)
                {
                    if (traceFlag.ScopeId != null && !names.ContainsKey(traceFlag.ScopeId))
                        names.Add(traceFlag.ScopeId, String.Empty);
                    if (traceFlag.TracedEntityId != null && !names.ContainsKey(traceFlag.TracedEntityId))
                        names.Add(traceFlag.TracedEntityId, String.Empty);
                }

                DataSelectResult nameData = DataSelectAll(String.Format(
                    "SELECT Id, Name FROM User WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (user)", row["Name"]);

                nameData = DataSelectAll(String.Format(
                    "SELECT Id, Name FROM ApexClass WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (class)", row["Name"]);

                nameData = DataSelectAll(String.Format(
                    "SELECT Id, Name FROM ApexTrigger WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (trigger)", row["Name"]);


                // create log parameters
                foreach (SalesForceAPI.Tooling.TraceFlag traceFlag in response.result.records)
                    result.Add(new LogParameters(
                        traceFlag,
                        (traceFlag.ScopeId != null) ? names[traceFlag.ScopeId] : String.Empty,
                        (traceFlag.TracedEntityId != null) ? names[traceFlag.TracedEntityId] : String.Empty));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Delete the given log parameters.
        /// </summary>
        /// <param name="logParameters">The log parameters to delete.</param>
        public void DeleteLogParameters(LogParameters logParameters)
        {
            if (logParameters == null)
                throw new ArgumentNullException("logParameters");

            if (String.IsNullOrWhiteSpace(logParameters.Id))
                return;

            InitClients();

            SalesForceAPI.Tooling.deleteResponse response = _toolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                            new string[] { logParameters.Id }));

            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    StringBuilder sb = new StringBuilder();
                    if (response.result[0].errors != null)
                        foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                            sb.AppendLine(err.message);

                    throw new Exception("Couldn't delete checkpoint: \r\n" + sb.ToString());
                }
            }
            else
            {
                throw new Exception("Couldn't delete checkpoint: Invalid response received.");
            }
        }

        /// <summary>
        /// Create new log parameters.
        /// </summary>
        /// <param name="traceEntityId">TraceEntityId.</param>
        /// <param name="tracedEntityName">TracedEntityName.</param>
        /// <param name="scopeId">ScopeId.</param>
        /// <param name="scopeName">ScopeName.</param>
        /// <param name="expirationDate">ExpirationDate.</param>
        /// <param name="codeLevel">CodeLevel.</param>
        /// <param name="visualForceLevel">VisualForceLevel.</param>
        /// <param name="profilingLevel">ProfilingLevel.</param>
        /// <param name="calloutLevel">CalloutLevel.</param>
        /// <param name="databaseLevel">DatabaseLevel.</param>
        /// <param name="systemLevel">SystemLevel.</param>
        /// <param name="validationLevel">ValidationLevel.</param>
        /// <param name="workflowLevel">WorkflowLevel.</param>
        /// <returns>The newly created log parameters.</returns>
        public LogParameters CreateLogParameters(
            string traceEntityId,
            string tracedEntityName,
            string scopeId,
            string scopeName,
            DateTime expirationDate,
            LogLevel codeLevel,
            LogLevel visualForceLevel,
            LogLevel profilingLevel,
            LogLevel calloutLevel,
            LogLevel databaseLevel,
            LogLevel systemLevel,
            LogLevel validationLevel,
            LogLevel workflowLevel)
        {
            if (traceEntityId == null)
                throw new ArgumentNullException("traceEntityId");

            InitClients();

            SalesForceAPI.Tooling.TraceFlag traceFlag = new SalesForceAPI.Tooling.TraceFlag();
            traceFlag.ApexCode = codeLevel.ToString().ToUpper();
            traceFlag.ApexProfiling = profilingLevel.ToString().ToUpper();
            traceFlag.Callout = calloutLevel.ToString().ToUpper();
            traceFlag.Database = databaseLevel.ToString().ToUpper();
            traceFlag.ExpirationDate = expirationDate.ToUniversalTime();
            traceFlag.ExpirationDateSpecified = true;
            traceFlag.ScopeId = scopeId;
            traceFlag.System = systemLevel.ToString().ToUpper();
            traceFlag.TracedEntityId = traceEntityId;
            traceFlag.Validation = validationLevel.ToString().ToUpper();
            traceFlag.Visualforce = visualForceLevel.ToString().ToUpper();
            traceFlag.Workflow = workflowLevel.ToString().ToUpper();

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                new SalesForceAPI.Tooling.sObject[] 
                {
                    traceFlag
                }));

            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    StringBuilder sb = new StringBuilder();
                    if (response.result[0].errors != null)
                        foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                            sb.AppendLine(err.message);

                    throw new Exception("Couldn't create log parameters: \r\n" + sb.ToString());
                }
            }
            else
            {
                throw new Exception("Couldn't create log parameters: Invalid response received.");
            }

            traceFlag.Id = response.result[0].id;

            return new LogParameters(traceFlag, scopeName, tracedEntityName);
        }

        /// <summary>
        /// Update the given log parameters.
        /// </summary>
        /// <param name="logParameters">The parameters to update.</param>
        public void UpdateLogParameters(LogParameters logParameters)
        {
            if (logParameters == null)
                throw new ArgumentNullException("logParameters");

            InitClients();

            SalesForceAPI.Tooling.updateResponse response = _toolingClient.update(new SalesForceAPI.Tooling.updateRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                new SalesForceAPI.Tooling.sObject[] 
                {
                    logParameters.ToTraceFlag()
                }));

            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    StringBuilder sb = new StringBuilder();
                    if (response.result[0].errors != null)
                        foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                            sb.AppendLine(err.message);

                    throw new Exception("Couldn't update log parameters: \r\n" + sb.ToString());
                }
            }
            else
            {
                throw new Exception("Couldn't update log parameters: Invalid response received.");
            }
        }

        /// <summary>
        /// Get the logs for the given user.
        /// </summary>
        /// <param name="userId">The id of the user to get logs for.</param>
        /// <returns>The log for the given user.</returns>
        public Log[] GetLogs(string userId)
        {
            InitClients();

            SalesForceAPI.Tooling.queryResponse response = _toolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                String.Format("SELECT Id, StartTime, DurationMilliseconds, Operation, Status FROM ApexLog WHERE LogUserId = '{0}' ORDER BY StartTime DESC", userId)));

            List<Log> result = new List<Log>();
            if (response.result.records != null)
            {
                // create logs
                foreach (SalesForceAPI.Tooling.ApexLog log in response.result.records)
                    result.Add(new Log(log));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get the content for the given log.
        /// </summary>
        /// <param name="log">The log to get the content for.</param>
        /// <returns>The log content.</returns>
        public string GetLogContent(Log log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            return _session.ExecuteRestCall(String.Format("/sobjects/ApexLog/{0}/Body/", log.Id));
        }

        /// <summary>
        /// Delete the given logs.
        /// </summary>
        /// <param name="logs">The logs to delete.</param>
        public void DeleteLogs(IEnumerable<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException("logs");

            InitClients();

            List<string> logIds = new List<string>();
            foreach (Log log in logs)
                logIds.Add(log.Id);

            SalesForceAPI.Tooling.deleteResponse response = _toolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                            logIds.ToArray()));

            if (response != null && response.result != null)
            {
                foreach (SalesForceAPI.Tooling.DeleteResult result in response.result)
                {
                    if (!result.success)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (response.result[0].errors != null)
                            foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                                sb.AppendLine(err.message);

                        throw new Exception("Couldn't delete all logs: \r\n" + sb.ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Couldn't delete logs: Invalid response received.");
            }
        }

        /// <summary>
        /// Get checkpoints that have been created.
        /// </summary>
        /// <returns>Existing checkpoints.</returns>
        public Checkpoint[] GetCheckpoints()
        {
            InitClients();

            SalesForceAPI.Tooling.queryResponse response = _toolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                String.Format("SELECT Id, ActionScript, ActionScriptType, ExecutableEntityId, ExpirationDate, IsDumpingHeap, Iteration, Line, ScopeId FROM ApexExecutionOverlayAction WHERE ScopeId = '{0}'", _session.UserId)));

            List<Checkpoint> result = new List<Checkpoint>();
            if (response.result.records != null)
            {
                // get file names
                Dictionary<string, string> idToFileNameMap = new Dictionary<string, string>();
                foreach (SalesForceAPI.Tooling.sObject record in response.result.records)
                    if (!idToFileNameMap.ContainsKey((record as SalesForceAPI.Tooling.ApexExecutionOverlayAction).ExecutableEntityId))
                        idToFileNameMap.Add(
                            (record as SalesForceAPI.Tooling.ApexExecutionOverlayAction).ExecutableEntityId,
                            String.Empty);

                DataSelectResult names = DataSelect(String.Format(
                    "SELECT Id, Name FROM ApexClass WHERE Id IN ('{0}')", 
                    String.Join("','", idToFileNameMap.Keys)));
                foreach (DataRow row in names.Data.Rows)
                {
                    idToFileNameMap.Remove(row["Id"] as string);
                    idToFileNameMap.Add(row["Id"] as string, row["Name"] as string);
                }

                names = DataSelect(String.Format(
                    "SELECT Id, Name FROM ApexTrigger WHERE Id IN ('{0}')",
                    String.Join("','", idToFileNameMap.Keys)));
                foreach (DataRow row in names.Data.Rows)
                {
                    idToFileNameMap.Remove(row["Id"] as string);
                    idToFileNameMap.Add(row["Id"] as string, row["Name"] as string);
                }

                // create checkpoints
                foreach (SalesForceAPI.Tooling.sObject record in response.result.records)
                    result.Add(new Checkpoint(
                        record as SalesForceAPI.Tooling.ApexExecutionOverlayAction,
                        idToFileNameMap[(record as SalesForceAPI.Tooling.ApexExecutionOverlayAction).ExecutableEntityId]));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Create a new checkpoint.
        /// </summary>
        /// <param name="file">The file to create a checkpoint in.</param>
        /// <param name="lineNumber">The line number in the file to create the checkpoint for.</param>
        /// <param name="iteration">The number of iterations before the checkpoint is processed.</param>
        /// <param name="isHeapDump">If true a heap dump will be collected with the checkpoint.</param>
        /// <param name="script">An optional script to run with the checkpoint.</param>
        /// <param name="scriptType">The type of script specified.</param>
        /// <returns>The newly created checkpoint.</returns>
        public Checkpoint CreateCheckpoint(
            SourceFile file,
            int lineNumber,
            int iteration,
            bool isHeapDump,
            string script,
            CheckpointScriptType scriptType)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            InitClients();

            // get record id
            string entityId = null;
            switch (file.FileType.Name)
            {
                case "ApexClass":
                case "ApexTrigger":
                    DataSelectResult objectQueryResult = DataSelect(String.Format("SELECT id FROM {0} WHERE Name = '{1}'", file.FileType.Name, file.Name));
                    if (objectQueryResult.Data.Rows.Count > 0)
                        entityId = objectQueryResult.Data.Rows[0]["id"] as string;
                    break;

                default:
                    throw new Exception("Unsupported type: " + file.FileType.Name);
            }

            if (entityId == null)
                throw new Exception("Couldn't get id for: " + file.Name);

            return CreateCheckpoint(
                entityId,
                file.Name,
                lineNumber,
                iteration,
                isHeapDump,
                script,
                scriptType);
        }

        /// <summary>
        /// Create a new checkpoint.
        /// </summary>
        /// <param name="entityId">The id of the file to create a checkpoint in.</param>
        /// <param name="fileName">The name of the file to create a checkpoint in.</param>
        /// <param name="lineNumber">The line number in the file to create the checkpoint for.</param>
        /// <param name="iteration">The number of iterations before the checkpoint is processed.</param>
        /// <param name="isHeapDump">If true a heap dump will be collected with the checkpoint.</param>
        /// <param name="script">An optional script to run with the checkpoint.</param>
        /// <param name="scriptType">The type of script specified.</param>
        /// <returns>The newly created checkpoint.</returns>
        private Checkpoint CreateCheckpoint(
            string entityId,
            string fileName,
            int lineNumber, 
            int iteration, 
            bool isHeapDump, 
            string script, 
            CheckpointScriptType scriptType)
        {
            if (entityId == null)
                throw new ArgumentNullException("entityId");

            InitClients();

            SalesForceAPI.Tooling.ApexExecutionOverlayAction action = new SalesForceAPI.Tooling.ApexExecutionOverlayAction();
            action.ActionScript = script;
            action.ActionScriptType = scriptType.ToString();
            action.ExecutableEntityId = entityId;
            action.ExpirationDate = DateTime.Now.AddDays(1);
            action.ExpirationDateSpecified = true;
            action.IsDumpingHeap = isHeapDump;
            action.IsDumpingHeapSpecified = true;
            action.Iteration = iteration;
            action.IterationSpecified = true;
            action.Line = lineNumber;
            action.LineSpecified = true;
            action.ScopeId = _session.UserId;

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                new SalesForceAPI.Tooling.sObject[] 
                {
                    action
                }));

            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    StringBuilder sb = new StringBuilder();
                    if (response.result[0].errors != null)
                        foreach (SalesForceAPI.Tooling.Error err in response.result[0].errors)
                            sb.AppendLine(err.message);

                    throw new Exception("Couldn't create checkpoint: \r\n" + sb.ToString());
                }
            }
            else
            {
                throw new Exception("Couldn't create checkpoint: Invalid response received.");
            }

            action.Id = response.result[0].id;

            return new Checkpoint(action, fileName);
        }

        /// <summary>
        /// Save the changes made to the checkpoint.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to update.</param>
        public void SaveCheckpoint(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                throw new ArgumentNullException("checkpoint");

            DeleteCheckpoint(checkpoint);
            Checkpoint c = CreateCheckpoint(
                checkpoint.EntityId,
                checkpoint.FileName,
                checkpoint.LineNumber,
                checkpoint.Iteration,
                checkpoint.HeapDump,
                checkpoint.Script,
                checkpoint.ScriptType);

            checkpoint.Update(c.ToAction());
        }

        /// <summary>
        /// Deploy a package.
        /// </summary>
        /// <param name="package">The package to deploy.</param>
        /// <param name="checkOnly">When true only a deployment check is performed.</param>
        /// <param name="runAllTests">When true all tests are run.</param>
        /// <returns>The id of the deployment that was started.</returns>
        public string DeployPackage(Package package, bool checkOnly, bool runAllTests)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            return DeployPackage(package.ToByteArray(), checkOnly, runAllTests);
        }

        /// <summary>
        /// Deploy a package.
        /// </summary>
        /// <param name="package">The package to deploy.</param>
        /// <param name="checkOnly">When true only a deployment check is performed.</param>
        /// <param name="runAllTests">When true all tests are run.</param>
        /// <returns>The id of the deployment that was started.</returns>
        public string DeployPackage(byte[] package, bool checkOnly, bool runAllTests)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            InitClients();

            SalesForceAPI.Metadata.deployRequest deployRequest = new SalesForceAPI.Metadata.deployRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                null,
                package,
                new SalesForceAPI.Metadata.DeployOptions()
                {
                    checkOnly = checkOnly,
                    runAllTests = runAllTests,
                    singlePackage = true,
                    rollbackOnError = true
                });

            return _metadataClient.deploy(deployRequest).result.id;
        }

        /// <summary>
        /// Check the status of a package deployment.
        /// </summary>
        /// <param name="id">The id of the package deployment that was returned in a call to DeployPackage.</param>
        /// <returns>The current package deployment status.</returns>
        public PackageDeployResult CheckPackageDeploy(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null or whitespace.", "id");

            InitClients();

            SalesForceAPI.Metadata.checkDeployStatusRequest checkRequest = new SalesForceAPI.Metadata.checkDeployStatusRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                id,
                true);

            return new PackageDeployResult(_metadataClient.checkDeployStatus(checkRequest).result);  
        }

        /// <summary>
        /// Cancel the deployment of a package.
        /// </summary>
        /// <param name="id">The id of the deployment to cancel.</param>
        public void CancelPackageDeploy(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null or whitespace.", "id");

            InitClients();

            _metadataClient.cancelDeploy(new SalesForceAPI.Metadata.cancelDeployRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                id));
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

            DataSelectResult classIdData = DataSelectAll(
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

            DataInsert(dt);

            // create the test run object
            List<TestRunItem> items = new List<TestRunItem>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new TestRunItem(
                    apexClassNameMap[row["ApexClassId"] as string],
                    row["ApexClassId"] as string));
            }

            DataSelectResult jobIdData = DataSelect(
                String.Format("SELECT ParentJobId FROM ApexTestQueueItem WHERE Id = '{0}'", dt.Rows[0]["Id"]));

            return new TestRun(jobIdData.Data.Rows[0]["ParentJobId"] as string, items.ToArray());
        }

        /// <summary>
        /// Update the test run with the most recent status.
        /// </summary>
        /// <param name="testRun">The test run to update.</param>
        public void UpdateTests(TestRun testRun)
        {
            InitClients();

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
                DataSelectResult testRunData = DataSelectAll(String.Format(
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

                DataSelectResult completedData = DataSelectAll(
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

        /// <summary>
        /// Delete the given file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        public void DeleteSourceFile(SourceFile file)
        {
            InitClients();

            if (file == null)
                throw new ArgumentNullException("file");

            switch (file.FileType.Name)
            {
                case "ApexClass":
                case "ApexTrigger":
                case "ApexPage":
                case "ApexComponent":

                    // parse name for apex items
                    string itemName = null;
                    string namespacePrefix = null;

                    string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                    if (classNameParts.Length == 1)
                    {
                        itemName = classNameParts[0];
                        namespacePrefix = GetOrgInfo().organizationNamespace;
                    }
                    else if (classNameParts.Length == 2)
                    {
                        itemName = classNameParts[1];
                        namespacePrefix = classNameParts[0];
                    }
                    else
                    {
                        itemName = file.Name;
                        namespacePrefix = GetOrgInfo().organizationNamespace;
                    }

                    // get record
                    DataSelectResult objectQueryResult = DataSelect(String.Format("SELECT id FROM {0} WHERE Name = '{1}' AND NamespacePrefix = '{2}'", file.FileType.Name, itemName, namespacePrefix));
                    string objectId = null;
                    if (objectQueryResult.Data.Rows.Count > 0)
                        objectId = objectQueryResult.Data.Rows[0]["id"] as string;
                    else
                        throw new Exception("Couldn't find id of object.");

                    // delete record
                    SalesForceAPI.Tooling.deleteResponse response = _toolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                        new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                        new string[] { objectId }));

                    // process any error messages
                    if (response == null || response.result == null || response.result.Length != 1)
                        throw new Exception("Invalid response received.");

                    if (response.result[0].errors != null && response.result[0].errors.Length > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                            sb.AppendLine(error.message);

                        throw new Exception(sb.ToString());
                    }

                    break;

                default:
                    throw new Exception("The given file type is not supported: " + file.FileType.Name);
            }
        }

        /// <summary>
        /// Get the data for the given file.
        /// </summary>
        /// <param name="file">The file to get data for.</param>
        /// <returns>The requested data or null if it isn't supported.</returns>
        public SourceFileData GetSourceFileData(SourceFile file)
        {
            InitClients();

            if (file == null)
                throw new ArgumentNullException("file");

            SalesForceAPI.Metadata.readMetadataRequest request = new SalesForceAPI.Metadata.readMetadataRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                file.FileType.Name,
                new string[] { file.Name });

            SalesForceAPI.Metadata.readMetadataResponse response = _metadataClient.readMetadata(request);
            if (response == null || response.result.Length != 1)
                return null;

            return SourceFileData.Create(file, response.result[0]);
        }

        /// <summary>
        /// Save the source file data.
        /// </summary>
        /// <param name="data">The data to save.</param>
        public void SaveSourceFileData(SourceFileData data)
        {
            InitClients();

            if (data == null)
                throw new ArgumentNullException("data");

            SalesForceAPI.Metadata.updateMetadataRequest request = new SalesForceAPI.Metadata.updateMetadataRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                new SalesForceAPI.Metadata.Metadata[] { data.GetMetadata() });

            SalesForceAPI.Metadata.updateMetadataResponse response = _metadataClient.updateMetadata(request);
            if (response != null && response.result != null && response.result.Length == 1)
            {
                if (!response.result[0].success)
                {
                    if (response.result[0].errors != null && response.result[0].errors.Length > 0)
                    {
                        StringBuilder msg = new StringBuilder();
                        foreach (SalesForceAPI.Metadata.Error err in response.result[0].errors)
                            msg.AppendLine(String.Format("{0}:{1}", err.statusCode, err.message));

                        throw new Exception(msg.ToString());
                    }
                    else
                    {
                        throw new Exception("An unknown exception occured when trying to update MetaData.");
                    }
                }
            }
        }

        /// <summary>
        /// Get the content for the given source file.
        /// </summary>
        /// <param name="file">The file to get content for.</param>
        /// <returns>The requested content.</returns>
        public SourceFileContent GetSourceFileContent(SourceFile file)
        {
            InitClients();

            if (file == null)
                throw new ArgumentNullException("file");

            // parse name for apex items
            string itemName = null;
            string namespacePrefix = null;

            string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            if (classNameParts.Length == 1)
            {
                itemName = classNameParts[0];
                namespacePrefix = GetOrgInfo().organizationNamespace;
            }
            else if (classNameParts.Length == 2)
            {
                itemName = classNameParts[1];
                namespacePrefix = classNameParts[0];
            }
            else
            {
                itemName = file.Name;
                namespacePrefix = GetOrgInfo().organizationNamespace;
            }

            switch (file.FileType.Name)
            {
                case "ApexClass":

                    DataSelectResult classResult = DataSelect(String.Format(
                        "SELECT Id, Body, LastModifiedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (classResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex class named: " + file.Name);

                    string id = classResult.Data.Rows[0]["Id"] as string;
                    string classBody = classResult.Data.Rows[0]["Body"] as string;
                    string lastModifiedDate = classResult.Data.Rows[0]["LastModifiedDate"] as string;
                    bool isClassReadOnly = false;

                    // managed classes don't expose source code so lookup the symbol table
                    if (classBody == "(hidden)")
                    {
                        isClassReadOnly = true;
                        SalesForceAPI.Tooling.retrieveRequest request = new SalesForceAPI.Tooling.retrieveRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                            "SymbolTable",
                            "ApexClass",
                            new string[] { id });

                        SalesForceAPI.Tooling.retrieveResponse response = _toolingClient.retrieve(request);
                        if (response != null && response.result != null && response.result.Length == 1)
                        {
                            SalesForceAPI.Tooling.ApexClass apexClass = response.result[0] as SalesForceAPI.Tooling.ApexClass;
                            if (apexClass != null && apexClass.SymbolTable != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("/**\n * The following apex was generated by the application and represents the SalesForce definition of this class.\n */\n");
                                SymbolTableToText(apexClass.SymbolTable, sb, 0);
                                classBody = sb.ToString();
                            }
                        }
                    }

                    return new SourceFileContent(
                        file.FileType.Name,
                        classBody,
                        lastModifiedDate,
                        null,
                        isClassReadOnly);

                case "ApexTrigger":

                    DataSelectResult triggerResult = DataSelect(String.Format(
                        "SELECT Body, LastModifiedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (triggerResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex trigger named: " + file.Name);

                    string triggerBody = triggerResult.Data.Rows[0]["Body"] as string;
                    bool isTriggerReadOnly = false;

                    // managed classes don't expose source code
                    if (triggerBody == "(hidden)")
                        isTriggerReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        triggerBody,
                        triggerResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isTriggerReadOnly);

                case "ApexPage":

                    DataSelectResult pageResult = DataSelect(String.Format(
                        "SELECT Markup, LastModifiedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (pageResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex page named: " + file.Name);

                    string pageBody = pageResult.Data.Rows[0]["Markup"] as string;
                    bool isPageReadOnly = false;

                    // managed classes don't expose source code
                    if (pageBody == "(hidden)")
                        isPageReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        pageBody,
                        pageResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isPageReadOnly);

                case "ApexComponent":

                    DataSelectResult componentResult = DataSelect(String.Format(
                        "SELECT Markup, LastModifiedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (componentResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex component named: " + file.Name);

                    string componentBody = componentResult.Data.Rows[0]["Markup"] as string;
                    bool isComponentReadOnly = false;

                    // managed classes don't expose source code
                    if (componentBody == "(hidden)")
                        isComponentReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        componentBody,
                        componentResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isComponentReadOnly);

                default:

                    byte[] zipFile = GetSourceFileContentAsPackage(new SourceFile[] { file });
                    using (MemoryStream ms = new MemoryStream(zipFile))
                    {
                        using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
                        {
                            string text = null;
                            ZipArchiveEntry zipEntry = zip.GetEntry(file.FileName);
                            if (zipEntry == null)
                                throw new Exception("Couldn't find file named " + file.FileName + " in the downloaded zip archive.");                            
                            using (StreamReader reader = new StreamReader(zipEntry.Open()))
                                text = reader.ReadToEnd();

                            string metadataText = null;
                            ZipArchiveEntry metadataZipEntry = zip.GetEntry(file.MetadataFileName);
                            if (metadataZipEntry != null)
                            {
                                using (StreamReader metadataReader = new StreamReader(metadataZipEntry.Open()))
                                    metadataText = metadataReader.ReadToEnd();
                            }

                            return new SourceFileContent(
                                file.FileType.Name, 
                                text, 
                                GetSourceFileContentLastModifiedTimeStamp(file),
                                metadataText,
                                false);
                        }
                    }
            }
        }

        /// <summary>
        /// Convert the given symbol table into apex text.
        /// </summary>
        /// <param name="symbolTable">The symbol table to convert.</param>
        /// <param name="sb">The output for the apex text.</param>
        /// <param name="indentCount">The indent count to start text at.</param>
        private void SymbolTableToText(
            SalesForceAPI.Tooling.SymbolTable symbolTable, 
            StringBuilder sb, 
            int indentCount)
        {
            if (symbolTable == null)
                return;

            // start namespace
            if (!String.IsNullOrWhiteSpace(symbolTable.@namespace))
            {
                if (indentCount == 0)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("public class {0} {{\n\n", symbolTable.@namespace);
                    indentCount++;
                }
            }

            // start class definition
            if (symbolTable.tableDeclaration != null)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.AppendFormat("public class {0} {{\n\n", symbolTable.tableDeclaration.name);
                indentCount++;
            }

            // constructors
            if (symbolTable.constructors != null && symbolTable.constructors.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Constructors\n\n");

                foreach (SalesForceAPI.Tooling.Constructor constructor in symbolTable.constructors)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1}(",
                        constructor.visibility.ToString().ToLower(),
                        constructor.name);

                    if (constructor.parameters != null && constructor.parameters.Length > 0)
                    {
                        foreach (SalesForceAPI.Tooling.Parameter param in constructor.parameters)
                            sb.AppendFormat("{0} {1}, ", param.type, param.name);

                        sb.Length = sb.Length - 2;
                    }

                    sb.Append(");\n\n");
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // properties
            if (symbolTable.properties != null && symbolTable.properties.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Properties\n\n");

                foreach (SalesForceAPI.Tooling.VisibilitySymbol prop in symbolTable.properties)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1} {2} {{ get; set; }}\n\n",
                        prop.visibility.ToString().ToLower(),
                        prop.type,
                        prop.name);
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // methods
            if (symbolTable.methods != null && symbolTable.methods.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Methods\n\n");

                foreach (SalesForceAPI.Tooling.Method method in symbolTable.methods)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1} {2}(",
                        method.visibility.ToString().ToLower(),
                        method.returnType,
                        method.name);

                    if (method.parameters != null && method.parameters.Length > 0)
                    {
                        foreach (SalesForceAPI.Tooling.Parameter param in method.parameters)
                            sb.AppendFormat("{0} {1}, ", param.type, param.name);

                        sb.Length = sb.Length - 2;
                    }

                    sb.Append(");\n\n");
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // inner classes
            if (symbolTable.innerClasses != null && symbolTable.innerClasses.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Inner Classes\n\n");

                foreach (SalesForceAPI.Tooling.SymbolTable st in symbolTable.innerClasses)
                    SymbolTableToText(st, sb, indentCount);

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // end class definition
            if (symbolTable.tableDeclaration != null)
            {
                indentCount--;
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("}\n\n");                
            }

            // end namespace
            if (!String.IsNullOrWhiteSpace(symbolTable.@namespace))
            {
                indentCount--;

                if (indentCount == 0)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.Append("}\n\n");
                }
            }
        }

        /// <summary>
        /// Gets the timestamp for when the source file content was last updated.
        /// </summary>
        /// <param name="file">The file to get the timestamp for.</param>
        /// <returns>The timestamp for the given file's content.</returns>
        public string GetSourceFileContentLastModifiedTimeStamp(SourceFile file)
        {
            InitClients();

            if (file == null)
                throw new ArgumentNullException("file");

            // parse name for apex items
            string itemName = null;
            string namespacePrefix = null;

            string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            if (classNameParts.Length == 1)
            {
                itemName = classNameParts[0];
                namespacePrefix = GetOrgInfo().organizationNamespace;
            }
            else if (classNameParts.Length == 2)
            {
                itemName = classNameParts[1];
                namespacePrefix = classNameParts[0];
            }
            else
            {
                itemName = file.Name;
                namespacePrefix = GetOrgInfo().organizationNamespace;
            }

            switch (file.FileType.Name)
            {
                case "ApexClass":

                    DataSelectResult classResult = DataSelect(String.Format(
                        "SELECT LastModifiedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (classResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex class named: " + file.Name);

                    return classResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexTrigger":

                    DataSelectResult triggerResult = DataSelect(String.Format(
                        "SELECT LastModifiedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (triggerResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex trigger named: " + file.Name);

                    return triggerResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexPage":

                    DataSelectResult pageResult = DataSelect(String.Format(
                        "SELECT LastModifiedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (pageResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex page named: " + file.Name);

                    return pageResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexComponent":

                    DataSelectResult componentResult = DataSelect(String.Format(
                        "SELECT LastModifiedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (componentResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex component named: " + file.Name);

                    return componentResult.Data.Rows[0]["LastModifiedDate"] as string;

                default:
                    SalesForceAPI.Metadata.ListMetadataQuery query = new SalesForceAPI.Metadata.ListMetadataQuery();
                    query.type = file.FileType.Name;
                    SalesForceAPI.Metadata.FileProperties[] fileProperties = _metadataClient.listMetadata(new SalesForceAPI.Metadata.listMetadataRequest(
                        new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                        null,
                        new SalesForceAPI.Metadata.ListMetadataQuery[] { query },
                        METADATA_VERSION)).result;

                    SalesForceAPI.Metadata.FileProperties fileProp = fileProperties.Where(fp => fp.fullName == file.Name).FirstOrDefault();
                    if (fileProp == null)
                        throw new Exception("Couldn't find file named: " + file.Name);

                    return fileProp.lastModifiedDate.ToString();
            }
        }

        /// <summary>
        /// Create a new class.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="header">Optional header text to include at the head of the class.</param>
        /// <returns>The source file for the newly created class.</returns>
        public SourceFile CreateClass(string name, string header)
        {
            InitClients();

            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexClass apexClass = new SalesForceAPI.Tooling.ApexClass();
            if (header != null)
                apexClass.Body = String.Format("{0}{1}public class {2} {{{1}}}", header, Environment.NewLine, name);
            else
                apexClass.Body = String.Format("public class {0} {{{1}}}", name, Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexClass }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = DataSelect(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, GetOrgInfo().organizationNamespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="objectName">The name of the object to create the trigger on.</param>
        /// <param name="triggerEvents">The events for the trigger.</param>
        /// <param name="header">Optional header text to include at the head of the trigger.</param>
        /// <returns>The newly created trigger.</returns>
        public SourceFile CreateTrigger(string triggerName, string objectName, TriggerEvents triggerEvents, string header)
        {
            InitClients();

            if (String.IsNullOrWhiteSpace(triggerName))
                throw new ArgumentException("triggerName", "triggerName is null or whitespace.");
            if (String.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("objectName", "objectName is null or whitespace.");
            if (triggerEvents == TriggerEvents.None)
                throw new ArgumentException("triggerEvents", "triggerEvents can't be None.");

            StringBuilder triggerEventsText = new StringBuilder();
            if (triggerEvents.HasFlag(TriggerEvents.BeforeInsert))
                triggerEventsText.Append("before insert, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterInsert))
                triggerEventsText.Append("after insert, ");
            if (triggerEvents.HasFlag(TriggerEvents.BeforeUpdate))
                triggerEventsText.Append("before update, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterUpdate))
                triggerEventsText.Append("after update, ");
            if (triggerEvents.HasFlag(TriggerEvents.BeforeDelete))
                triggerEventsText.Append("before delete, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterDelete))
                triggerEventsText.Append("after delete, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterUndelete))
                triggerEventsText.Append("after undelete, ");
            triggerEventsText.Length = triggerEventsText.Length - 2;

            SalesForceAPI.Tooling.ApexTrigger apexTrigger = new SalesForceAPI.Tooling.ApexTrigger();
            apexTrigger.Name = triggerName;
            apexTrigger.TableEnumOrId = objectName;

            if (header != null)
                apexTrigger.Body = String.Format("{0}{4}trigger {1} on {2}({3}) {{{4}}}",
                    header,
                    triggerName,
                    objectName,
                    triggerEventsText.ToString(),
                    Environment.NewLine);
            else
                apexTrigger.Body = String.Format("trigger {0} on {1}({2}) {{{3}}}",
                    triggerName,
                    objectName,
                    triggerEventsText.ToString(),
                    Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexTrigger }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = DataSelect(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", triggerName, GetOrgInfo().organizationNamespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new component.
        /// </summary>
        /// <param name="name">The name of the component to create.</param>
        /// <param name="header">Optional header text to include at the head of the page.</param>
        /// <returns>The newly created component.</returns>
        public SourceFile CreateComponent(string name, string header)
        {
            InitClients();

            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexComponent apexComponent = new SalesForceAPI.Tooling.ApexComponent();
            apexComponent.Name = name;
            apexComponent.MasterLabel = name;

            if (header != null)
                apexComponent.Markup = String.Format("{0}{1}<apex:component>{1}</apex:component>", header, Environment.NewLine);
            else
                apexComponent.Markup = String.Format("<apex:component>{0}</apex:component>", Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexComponent }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = DataSelect(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, GetOrgInfo().organizationNamespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new page.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="header">Optional header text to include at the head of the page.</param>
        /// <returns>The source file for the newly created page.</returns>
        public SourceFile CreatePage(string name, string header)
        {
            InitClients();

            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexPage apexPage = new SalesForceAPI.Tooling.ApexPage();
            apexPage.Name = name;
            apexPage.MasterLabel = name;

            if (header != null)
                apexPage.Markup = String.Format("{0}{1}<apex:page>{1}</apex:page>", header, Environment.NewLine);
            else
                apexPage.Markup = String.Format("<apex:page>{0}</apex:page>", Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _toolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexPage }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = DataSelect(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, GetOrgInfo().organizationNamespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Save the source file content.
        /// </summary>
        /// <param name="file">The file to save the content for.</param>
        /// <param name="contentValue">The content to save.</param>
        /// <returns>Any errors that occured.</returns>
        public SalesForceError[] SaveSourceFileContent(SourceFile file, string contentValue)
        {
            return SaveSourceFileContent(file, contentValue, null);
        }

        /// <summary>
        /// Save the source file content.
        /// </summary>
        /// <param name="file">The file to save the content for.</param>
        /// <param name="contentValue">The content to save.</param>
        /// <param name="metadataValue">The metadata value.</param>
        /// <returns>Any errors that occured.</returns>
        public SalesForceError[] SaveSourceFileContent(SourceFile file, string contentValue, string metadataValue)
        {
            InitClients();

            if (file == null)
                throw new ArgumentNullException("file");

            switch (file.FileType.Name)
            {
                case "ApexClass":
                case "ApexPage":
                case "ApexTrigger":
                case "ApexComponent":

                    // parse name for apex items
                    string itemName = null;
                    string namespacePrefix = null;

                    string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                    if (classNameParts.Length == 1)
                    {
                        itemName = classNameParts[0];
                        namespacePrefix = GetOrgInfo().organizationNamespace;
                    }
                    else if (classNameParts.Length == 2)
                    {
                        itemName = classNameParts[1];
                        namespacePrefix = classNameParts[0];
                    }
                    else
                    {
                        itemName = file.Name;
                        namespacePrefix = GetOrgInfo().organizationNamespace;
                    }

                    // get item record
                    DataSelectResult objectQueryResult = DataSelect(String.Format("SELECT id FROM {0} WHERE Name = '{1}' AND NamespacePrefix = '{2}'", file.FileType.Name, itemName, namespacePrefix));
                    string objectId = null;
                    if (objectQueryResult.Data.Rows.Count > 0)
                        objectId = objectQueryResult.Data.Rows[0]["id"] as string;

                    // create metadata container
                    SalesForceAPI.Tooling.MetadataContainer metadataContainer = new SalesForceAPI.Tooling.MetadataContainer();
                    metadataContainer.Name = Guid.NewGuid().ToString("N");
                    SalesForceAPI.Tooling.createResponse containerResponse = _toolingClient.create(new SalesForceAPI.Tooling.createRequest(
                        new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                        new SalesForceAPI.Tooling.sObject[] { metadataContainer }));

                    if (containerResponse == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse returned was null.", null) };
                    if (containerResponse.result == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse.result returned was null.", null) };
                    if (containerResponse.result.Length != 1)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse.result length was an unexpected value: " + containerResponse.result.Length, null) };
                    if (!containerResponse.result[0].success)
                    {
                        List<SalesForceError> errors = new List<SalesForceError>();
                        if (containerResponse.result[0].errors != null)
                            foreach (SalesForceAPI.Tooling.Error err in containerResponse.result[0].errors)
                                errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                        else
                            errors.Add(new SalesForceError("SYSTEM", "containerResponse.result inidcated failure but reported no error details.", null));

                        return errors.ToArray();
                    }

                    // stage object
                    SalesForceAPI.Tooling.sObject stageObject = null;

                    if (file.FileType.Name == "ApexClass")
                    {
                        SalesForceAPI.Tooling.ApexClassMember apexClass = new SalesForceAPI.Tooling.ApexClassMember();
                        apexClass.ContentEntityId = objectId;
                        apexClass.Body = contentValue ?? String.Empty;
                        apexClass.MetadataContainerId = containerResponse.result[0].id;
                        stageObject = apexClass;
                    }
                    else if (file.FileType.Name == "ApexTrigger")
                    {
                        SalesForceAPI.Tooling.ApexTriggerMember apexTrigger = new SalesForceAPI.Tooling.ApexTriggerMember();
                        apexTrigger.ContentEntityId = objectId;
                        apexTrigger.Body = contentValue ?? String.Empty;
                        apexTrigger.MetadataContainerId = containerResponse.result[0].id;
                        stageObject = apexTrigger;
                    }
                    else if (file.FileType.Name == "ApexPage")
                    {
                        SalesForceAPI.Tooling.ApexPageMember apexPage = new SalesForceAPI.Tooling.ApexPageMember();
                        apexPage.ContentEntityId = objectId;
                        apexPage.Body = contentValue ?? String.Empty;
                        apexPage.MetadataContainerId = containerResponse.result[0].id;
                        stageObject = apexPage;
                    }
                    else if (file.FileType.Name == "ApexComponent")
                    {
                        SalesForceAPI.Tooling.ApexComponentMember apexComponent = new SalesForceAPI.Tooling.ApexComponentMember();
                        apexComponent.ContentEntityId = objectId;
                        apexComponent.Body = contentValue ?? String.Empty;
                        apexComponent.MetadataContainerId = containerResponse.result[0].id;
                        stageObject = apexComponent;
                    }
                    

                    SalesForceAPI.Tooling.createResponse stageApexResponse = _toolingClient.create(new SalesForceAPI.Tooling.createRequest(
                        new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                        new SalesForceAPI.Tooling.sObject[] { stageObject }));

                    if (stageApexResponse == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse returned was null.", null) };
                    if (stageApexResponse.result == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse.result returned was null.", null) };
                    if (stageApexResponse.result.Length != 1)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse.result length was an unexpected value: " + stageApexResponse.result.Length, null) };
                    if (!stageApexResponse.result[0].success)
                    {
                        List<SalesForceError> errors = new List<SalesForceError>();
                        if (stageApexResponse.result[0].errors != null)
                            foreach (SalesForceAPI.Tooling.Error err in stageApexResponse.result[0].errors)
                                errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                        else
                            errors.Add(new SalesForceError("SYSTEM", "stageApexResponse.result inidcated failure but reported no error details.", null));

                        return errors.ToArray();
                    }

                    // save apex
                    SalesForceAPI.Tooling.ContainerAsyncRequest apexSaveRequest = new SalesForceAPI.Tooling.ContainerAsyncRequest();
                    apexSaveRequest.IsCheckOnly = false;
                    apexSaveRequest.MetadataContainerId = containerResponse.result[0].id;
                    SalesForceAPI.Tooling.createResponse apexSaveResponse = _toolingClient.create(new SalesForceAPI.Tooling.createRequest(
                        new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                        new SalesForceAPI.Tooling.sObject[] { apexSaveRequest }));

                    if (apexSaveResponse == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse returned was null.", null) };
                    if (apexSaveResponse.result == null)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse.result returned was null.", null) };
                    if (apexSaveResponse.result.Length != 1)
                        return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse.result length was an unexpected value: " + apexSaveResponse.result.Length, null) };
                    if (!apexSaveResponse.result[0].success)
                    {
                        List<SalesForceError> errors = new List<SalesForceError>();
                        if (apexSaveResponse.result[0].errors != null)
                            foreach (SalesForceAPI.Tooling.Error err in apexSaveResponse.result[0].errors)
                                errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                        else
                            errors.Add(new SalesForceError("SYSTEM", "apexSaveResponse.result inidcated failure but reported no error details.", null));

                        return errors.ToArray();
                    }
                    string saveRequestId = apexSaveResponse.result[0].id;

                    // get result        
                    DateTime startTime = DateTime.Now;
                    apexSaveRequest.State = "Queued";
                    while (apexSaveRequest.State == "Queued")
                    {
                        SalesForceAPI.Tooling.queryResponse pollResponse = _toolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                            String.Format("SELECT Id, State, CompilerErrors, ErrorMsg FROM ContainerAsyncRequest where id = '{0}'", saveRequestId)));

                        if (pollResponse != null &&
                            pollResponse.result != null &&
                            pollResponse.result.records.Length == 1)
                        {
                            apexSaveRequest = pollResponse.result.records[0] as SalesForceAPI.Tooling.ContainerAsyncRequest;
                        }

                        if (apexSaveRequest.State == "Queued")
                        {
                            if (DateTime.Now - startTime > SAVE_TIMEOUT)
                                throw new Exception("A client side timeout occured while trying to save a file to SalesForce.");

                            System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                        }
                    }

                    _toolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                        new SalesForceAPI.Tooling.SessionHeader() { sessionId = _session.Id },
                        new string[] { containerResponse.result[0].id }));

                    switch (apexSaveRequest.State)
                    {
                        case "Completed":
                            DataSelectResult objectNameQueryResult = DataSelect(String.Format("SELECT Name FROM {0} WHERE Id = '{1}'", file.FileType.Name, objectId));
                            if (objectNameQueryResult.Data.Rows.Count > 0)
                            {
                                string name = objectNameQueryResult.Data.Rows[0]["Name"] as string;
                                file.UpdateName(name);
                            }

                            return new SalesForceError[0];

                        default:
                            List<SalesForceError> errors = new List<SalesForceError>();
                            errors.Add(new SalesForceError("SYSTEM", "Failed to save apex file.", null));
                            if (!String.IsNullOrWhiteSpace(apexSaveRequest.ErrorMsg))
                                errors.Add(new SalesForceError("ERROR", apexSaveRequest.ErrorMsg, null));
                            if (!String.IsNullOrWhiteSpace(apexSaveRequest.CompilerErrors))
                                errors.Add(new SalesForceError("COMPILE ERROR", apexSaveRequest.CompilerErrors, null));
                            return errors.ToArray();
                    }

                // all other source files
                default:
                    using (MemoryStream msZip = new MemoryStream())
                    {
                        using (ZipArchive zip = new ZipArchive(msZip, ZipArchiveMode.Create))
                        {
                            ZipArchiveEntry fileEntry = zip.CreateEntry(file.FileName);
                            using (StreamWriter fileWriter = new StreamWriter(fileEntry.Open()))
                                fileWriter.Write(contentValue);

                            if (metadataValue != null)
                            {
                                ZipArchiveEntry metadataEntry = zip.CreateEntry(file.MetadataFileName);
                                using (StreamWriter metadataWriter = new StreamWriter(metadataEntry.Open()))
                                    metadataWriter.Write(metadataValue);
                            }

                            Manifest manifest = new Manifest("package");
                            manifest.AddGroup(new ManifestItemGroup(file.FileType.Name));
                            manifest.Groups.ElementAt(0).AddItem(new ManifestItem(file.Name));
                            ZipArchiveEntry manifestEntry = zip.CreateEntry("package.xml");
                            using (Stream manifestWriter = manifestEntry.Open())
                                manifest.Save(manifestWriter);
                        }

                        msZip.Flush();

                        SalesForceAPI.Metadata.AsyncResult result = _metadataClient.deploy(new SalesForceAPI.Metadata.deployRequest(
                            new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                            null,
                            null,
                            msZip.ToArray(),
                            new SalesForceAPI.Metadata.DeployOptions()
                            {
                                singlePackage = true,
                                rollbackOnError = true
                            })).result;

                        int pollCount = 0;

                        SalesForceAPI.Metadata.DeployResult deployResult = new SalesForceAPI.Metadata.DeployResult();
                        deployResult.id = result.id;
                        while (!deployResult.done && String.IsNullOrWhiteSpace(deployResult.errorMessage))
                        {
                            if (pollCount > 100)
                            {
                                return new SalesForceError[] 
                                { 
                                    new SalesForceError(
                                        null, 
                                        "Save timed out waiting for response from the server.  Note that your save might have gone through.",
                                        null)
                                };
                            }

                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.2));

                            deployResult = _metadataClient.checkDeployStatus(new SalesForceAPI.Metadata.checkDeployStatusRequest(
                                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                                null,
                                result.id,
                                true)).result;

                            pollCount++;
                        }

                        if (deployResult.success)
                            return new SalesForceError[0];

                        return new SalesForceError[] 
                        { 
                            new SalesForceError(
                                null, 
                                "Unable to save. " + deployResult.errorMessage,
                                null)
                        };
                    }                    
            }
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="groupedFiles">The files to download grouped by their file type.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        private byte[] GetSourceFileContentAsPackage(Dictionary<string, HashSet<string>> groupedFiles)
        {
            if (groupedFiles == null)
                throw new ArgumentNullException("groupedFiles");

            InitClients();

            // create request
            SalesForceAPI.Metadata.RetrieveRequest request = new SalesForceAPI.Metadata.RetrieveRequest();
            request.apiVersion = METADATA_VERSION;
            request.unpackaged = new SalesForceAPI.Metadata.Package();
            request.unpackaged.version = METADATA_VERSION.ToString("N1");
            request.singlePackage = true;

            // translate files into package request
            List<SalesForceAPI.Metadata.PackageTypeMembers> types = new List<SalesForceAPI.Metadata.PackageTypeMembers>();
            foreach (KeyValuePair<string, HashSet<string>> kvp in groupedFiles)
            {
                SalesForceAPI.Metadata.PackageTypeMembers memberGroup = new SalesForceAPI.Metadata.PackageTypeMembers();
                memberGroup.name = kvp.Key;
                memberGroup.members = kvp.Value.ToArray();
                types.Add(memberGroup);
            }

            request.unpackaged.types = types.ToArray();

            // submit request
            SalesForceAPI.Metadata.retrieveResponse retrieveResponse = _metadataClient.retrieve(new SalesForceAPI.Metadata.retrieveRequest1(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                request));

            if (retrieveResponse == null)
                throw new Exception("There was an error trying to retrieve metadata.  retrieveResponse was null.");
            if (retrieveResponse.result == null)
                throw new Exception("There was an error trying to retrieve metadata.  retrieveResponse.result property was null.");
            if (retrieveResponse.result.state == SalesForceAPI.Metadata.AsyncRequestState.Error)
                throw new Exception(String.Format("There was an error trying to retrieve metadata. ({0}) {1}", retrieveResponse.result.statusCode, retrieveResponse.result.message));

            SalesForceAPI.Metadata.AsyncResult statusResult = retrieveResponse.result;

            // wait for request to be fulfilled
            int loopCount = 0;
            TimeSpan sleepTime = TimeSpan.FromSeconds(1);
            while (!statusResult.done)
            {
                if (loopCount < 5)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                else if (loopCount > 5 && loopCount < 20)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                else if (loopCount > 20)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

                SalesForceAPI.Metadata.checkStatusResponse checkStatusResponse = _metadataClient.checkStatus(new SalesForceAPI.Metadata.checkStatusRequest(
                    new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                    null,
                    new string[] { statusResult.id }));

                if (checkStatusResponse == null)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse was null.");
                if (checkStatusResponse.result == null)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse.result property was null.");
                if (checkStatusResponse.result.Length != 1)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse.result didn't have exactly one result.");

                statusResult = checkStatusResponse.result[0];
                loopCount++;
            }

            // download package
            SalesForceAPI.Metadata.checkRetrieveStatusResponse checkRetrieveStatusResponse = _metadataClient.checkRetrieveStatus(new SalesForceAPI.Metadata.checkRetrieveStatusRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                statusResult.id));

            if (checkRetrieveStatusResponse == null)
                throw new Exception("There was an error trying to check the retrieve status of a metadata retrieve.  checkRetrieveStatusResponse was null.");
            if (checkRetrieveStatusResponse.result == null)
                throw new Exception("There was an error trying to check the retrieve status of a metadata retrieve.  checkRetrieveStatusResponse.result property was null.");

            return checkRetrieveStatusResponse.result.zipFile;
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="files">The files to download.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        public byte[] GetSourceFileContentAsPackage(IEnumerable<SourceFile> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");
            if (files.Count() == 0)
                throw new ArgumentException("files count is zero.", "files");

            // group files by type
            Dictionary<string, HashSet<string>> memberMap = new Dictionary<string, HashSet<string>>();
            foreach (SourceFile file in files)
            {
                HashSet<string> memberSet = null;
                if (memberMap.ContainsKey(file.FileType.Name))
                {
                    memberSet = memberMap[file.FileType.Name];
                }
                else
                {
                    memberSet = new HashSet<string>();
                    memberMap.Add(file.FileType.Name, memberSet);
                }

                memberSet.Add(file.Name);

                // add children that are in a folder
                foreach (SourceFile child in file.Children)
                {
                    if (child.FileName != file.FileName)
                    {
                        HashSet<string> childMemberSet = null;
                        if (memberMap.ContainsKey(child.FileType.Name))
                        {
                            childMemberSet = memberMap[child.FileType.Name];
                        }
                        else
                        {
                            childMemberSet = new HashSet<string>();
                            memberMap.Add(child.FileType.Name, childMemberSet);
                        }

                        childMemberSet.Add(child.Name);
                    }
                }
            }

            return GetSourceFileContentAsPackage(memberMap);
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="manifest">The manifest that lists the files to download.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        public byte[] GetSourceFileContentAsPackage(Manifest manifest)
        {
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            Dictionary<string, HashSet<string>> memberMap = new Dictionary<string, HashSet<string>>();
            foreach (ManifestItemGroup group in manifest.Groups)
            {
                HashSet<string> groupSet = new HashSet<string>();
                foreach (ManifestItem item in group.Items)
                    groupSet.Add(item.Name);
                memberMap.Add(group.Name, groupSet);
            }

            return GetSourceFileContentAsPackage(memberMap);
        }

        /// <summary>
        /// Get all of the source file types.
        /// </summary>
        /// <returns>All source file types.</returns>
        public SourceFileType[] GetSourceFileTypes()
        {
            return GetSourceFileTypes(null);
        }

        /// <summary>
        /// Get the source file types.
        /// </summary>
        /// <param name="excluded">The names of types that should be excluded from the returned list.  If null then no types will be excluded.</param>
        /// <returns>All source file types.</returns>
        public SourceFileType[] GetSourceFileTypes(IEnumerable<string> excluded)
        {
            InitClients();

            HashSet<string> excludedSet = (excluded == null) ? new HashSet<string>() : new HashSet<string>(excluded);

            List<SourceFileType> results = new List<SourceFileType>();
            if (GetOrgInfo().metadataObjects != null)
            {
                foreach (SalesForceAPI.Metadata.DescribeMetadataObject metadataObject in GetOrgInfo().metadataObjects)
                {
                    if (metadataObject != null && !excludedSet.Contains(metadataObject.xmlName))
                        results.Add(new SourceFileType(metadataObject));
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Get the source files for the given types.
        /// </summary>
        /// <param name="fileTypes">The types of source files to get.</param>
        /// <param name="includeChildren">If true any children will be retrieved as well.</param>
        /// <returns>The requested source files.</returns>
        public SourceFile[] GetSourceFiles(SourceFileType[] fileTypes, bool includeChildren)
        {
            return GetSourceFiles(fileTypes, includeChildren, null);
        }

        /// <summary>
        /// Get the source files for the given types.
        /// </summary>
        /// <param name="fileTypes">The types of source files to get.</param>
        /// <param name="includeChildren">If true any children will be retrieved as well.</param>
        /// <param name="stateFilter">Only return files that have one of these states.  If null then no files are filtered out.</param>
        /// <returns>The requested source files.</returns>
        public SourceFile[] GetSourceFiles(SourceFileType[] fileTypes, bool includeChildren, IEnumerable<SourceFileState> stateFilter)
        {
            InitClients();

            HashSet<SourceFileState> stateFilterSet = (stateFilter == null) ? null : new HashSet<SourceFileState>(stateFilter);
            if (stateFilterSet != null && stateFilterSet.Count == 0)
                throw new ArgumentException("stateFilter is empty.  Set to null if you don't want a filter.", "stateFilter");

            // flatten the file types.
            Dictionary<string, SourceFileType> mapFileTypes = new Dictionary<string, SourceFileType>();
            foreach (SourceFileType fileType in fileTypes)
            {
                mapFileTypes.Add(fileType.PackageMemberName, fileType);
                if (includeChildren)
                    foreach (SourceFileType childFileType in fileType.Children)
                        mapFileTypes.Add(childFileType.PackageMemberName, childFileType);
            }

            // make calls to get file properties
            List<Task<SalesForceAPI.Metadata.FileProperties[]>> tasks = new List<Task<SalesForceAPI.Metadata.FileProperties[]>>();
            for (int i = 0; i < mapFileTypes.Values.Count; i += 3)
            {
                List<SourceFileType> buf = new List<SourceFileType>();
                for (int j = i; j < mapFileTypes.Values.Count && j < i + 3; j++)
                    buf.Add(mapFileTypes.Values.ElementAt(j));

                tasks.Add(GetFilePropertiesAsync(buf.ToArray()));
            }

            // collect and sort file properties
            Dictionary<string, List<SalesForceAPI.Metadata.FileProperties>> fileProperties = new Dictionary<string, List<SalesForceAPI.Metadata.FileProperties>>();
            foreach (Task<SalesForceAPI.Metadata.FileProperties[]> task in tasks)
            {
                task.Wait();

                foreach (SalesForceAPI.Metadata.FileProperties fp in task.Result)
                {
                    if (stateFilterSet != null && !stateFilterSet.Contains(SourceFile.GetState(fp)))
                        continue;

                    // for files that appear in folders, format the name so it is that of the folder it's in.
                    string fileName = fp.fileName;
                    int indexOfFirstSlash = fileName.IndexOf('/');
                    int indexOfLastSlash = fileName.LastIndexOf('/');
                    if (indexOfFirstSlash != indexOfLastSlash)
                        fileName = fileName.Substring(0, indexOfLastSlash);

                    // fix for difference in workflow folder names
                    if (fileName.StartsWith("workflows"))
                        fileName = String.Format("Workflow{0}", fileName.Substring(9));

                    List<SalesForceAPI.Metadata.FileProperties> list = null;
                    if (!fileProperties.ContainsKey(fileName))
                    {
                        list = new List<SalesForceAPI.Metadata.FileProperties>();
                        fileProperties.Add(fileName, list);
                    }
                    else
                    {
                        list = fileProperties[fileName];
                    }

                    list.Add(fp);
                }
            }

            // build source files from results 
            List<SourceFile> result = new List<SourceFile>();
            foreach (KeyValuePair<string, List<SalesForceAPI.Metadata.FileProperties>> kvp in fileProperties)
            {
                SalesForceAPI.Metadata.FileProperties parent = null;
                SourceFileType parentType = null;
                List<SourceFile> children = new List<SourceFile>();                

                foreach (SalesForceAPI.Metadata.FileProperties fp in kvp.Value)
                {
                    bool isInFolder = false;

                    // check for types that are in folders
                    string fileTypeName = fp.type;
                    if (!mapFileTypes.ContainsKey(fileTypeName))
                    {
                        fileTypeName = String.Format("{0}Folder", fileTypeName);
                        if (!mapFileTypes.ContainsKey(fileTypeName))
                        {
                            if (!String.IsNullOrWhiteSpace(fp.type))
                                result.Add(new SourceFile(new SourceFileType(fp.type, null), fp));

                            continue;
                        }
                        else
                        {
                            isInFolder = true;
                        }
                    }

                    SourceFileType fileType = mapFileTypes[fileTypeName];
                    if (!fileType.IsChild && !isInFolder)
                    {
                        if (parent != null)
                        {
                            result.Add(new SourceFile(parentType, parent, children));
                            parent = null;
                            children = new List<SourceFile>();
                        }

                        parent = fp;
                        parentType = fileType;
                    }
                    else
                    {
                        children.Add(new SourceFile(fileType, fp));
                    }
                }

                if (parent != null)
                    result.Add(new SourceFile(parentType, parent, children));
                else
                    result.AddRange(children);
            }

            // get user names
            //HashSet<string> distinctUserIds = new HashSet<string>();
            //foreach (SourceFile parent in result)
            //{
            //    if (!String.IsNullOrWhiteSpace(parent.ChangedById))
            //        distinctUserIds.Add(String.Format("'{0}'", parent.ChangedById));

            //    foreach (SourceFile child in parent.Children)
            //        if (!String.IsNullOrWhiteSpace(child.ChangedById))
            //            distinctUserIds.Add(String.Format("'{0}'", child.ChangedById));
            //}

            //IDictionary<string, string> userNames = GetUserNames(distinctUserIds);

            //foreach (SourceFile parent in result)
            //{
            //    if (userNames.ContainsKey(parent.ChangedById))
            //        parent.ChangedByName = userNames[parent.ChangedById];
            //    else
            //        parent.ChangedByName = "Unknown";

            //    foreach (SourceFile child in parent.Children)
            //    {
            //        if (userNames.ContainsKey(child.ChangedById))
            //            child.ChangedByName = userNames[child.ChangedById];
            //        else
            //            child.ChangedByName = "Unknown";
            //    }
            //}

            SourceFile[] files = result.ToArray();
            Array.Sort(files);
            return files;
        }

        /// <summary>
        /// An async call to get file properties for the given types.
        /// </summary>
        /// <param name="sourceFileTypes">The file types to get metadata files for.  A max of 3 types is allowed.</param>
        /// <returns>The running task that is getting the files.</returns>
        private Task<SalesForceAPI.Metadata.FileProperties[]> GetFilePropertiesAsync(SourceFileType[] sourceFileTypes)
        {
            if (sourceFileTypes == null || sourceFileTypes.Length == 0)
                throw new ArgumentException("At least one sourceFileTypes must be provided.", "sourceFileTypes");
            if (sourceFileTypes.Length > 3)
                throw new ArgumentException("Only up to 3 sourceFileTypes are allowed.", "sourceFileTypes");

            return Task<SalesForceAPI.Metadata.FileProperties[]>.Factory.StartNew(() =>
            {
                return GetFileProperties(sourceFileTypes);
            });
        }

        /// <summary>
        /// A call to get file properties for the given types.
        /// </summary>
        /// <param name="sourceFileTypes">The file types to get metadata files for.  A max of 3 types is allowed.</param>
        /// <returns>The resulting files.</returns>
        private SalesForceAPI.Metadata.FileProperties[] GetFileProperties(SourceFileType[] sourceFileTypes)
        {
            if (sourceFileTypes == null || sourceFileTypes.Length == 0)
                throw new ArgumentException("At least one sourceFileTypes must be provided.", "sourceFileTypes");
            if (sourceFileTypes.Length > 3)
                throw new ArgumentException("Only up to 3 sourceFileTypes are allowed.", "sourceFileTypes");

            List<string> sourceFileTypeNames = new List<string>();
            foreach (SourceFileType fileType in sourceFileTypes)
                sourceFileTypeNames.Add(fileType.PackageMemberName);

            List<SalesForceAPI.Metadata.ListMetadataQuery> queries = new List<SalesForceAPI.Metadata.ListMetadataQuery>();
            foreach (string metadataTypeName in sourceFileTypeNames)
            {
                if (String.IsNullOrWhiteSpace(metadataTypeName))
                    throw new ArgumentException("sourceFileTypeNames contains a null or empty type name.", "sourceFileTypeNames");

                queries.Add(new SalesForceAPI.Metadata.ListMetadataQuery() { type = metadataTypeName });
            }

            SalesForceAPI.Metadata.listMetadataRequest request = new SalesForceAPI.Metadata.listMetadataRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _session.Id },
                null,
                queries.ToArray(),
                METADATA_VERSION);

            SalesForceAPI.Metadata.listMetadataResponse response = null;

            try
            {
                response = _metadataClient.listMetadata(request);
            }
            catch (System.ServiceModel.FaultException err)
            {
                if (!err.Message.StartsWith("INVALID_TYPE")) // suppress invalid type errors.  no way to identify them in advance.
                    throw err;

                response = null;

                // if there is more than one file type then break it out into multiple calls for the file types that aren't invalid.
                if (sourceFileTypes.Length > 1)
                {
                    List<SalesForceAPI.Metadata.FileProperties> result = new List<SalesForceAPI.Metadata.FileProperties>();
                    foreach (SourceFileType sft in sourceFileTypes)
                        result.AddRange(GetFileProperties(new SourceFileType[] { sft }));

                    return result.ToArray();
                }
            }

            if (response != null && response.result != null)
            {
                // for files that are folders do further query to get their contents
                queries.Clear();
                foreach (SalesForceAPI.Metadata.FileProperties fp in response.result)
                {
                    if (fp.type != null && fp.type.EndsWith("Folder"))
                        queries.Add(new SalesForceAPI.Metadata.ListMetadataQuery()
                        {
                            folder = fp.fullName,
                            type = fp.type.Substring(0, fp.type.Length - 6)
                        });
                }

                if (queries.Count > 0)
                {
                    List<SalesForceAPI.Metadata.FileProperties> result = new List<SalesForceAPI.Metadata.FileProperties>(response.result);
                    foreach (SalesForceAPI.Metadata.ListMetadataQuery query in queries)
                    {
                        request.queries = new SalesForceAPI.Metadata.ListMetadataQuery[] { query };
                        response = _metadataClient.listMetadata(request);
                        if (response != null & response.result != null)
                            result.AddRange(response.result);
                    }

                    return result.ToArray();
                }
                else
                {
                    return response.result;
                }
            }
            else
            {
                return new SalesForceAPI.Metadata.FileProperties[0];
            }
        }

        /// <summary>
        /// Look up the user name for the given ids.
        /// </summary>
        /// <param name="userIds">The ids to get names for.</param>
        private IDictionary<string, string> GetUserNames(IEnumerable<string> userIds)
        {
            if (userIds == null)
                throw new ArgumentException("userIds is null.", "userIds");

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (userIds.Count() == 0)
                return result;

            DataSelectResult usersResult = DataSelect(String.Format("SELECT Id, Name FROM User WHERE Id IN ({0})", String.Join(",", userIds)));
            do
            {
                foreach (DataRow row in usersResult.Data.Rows)
                    result.Add(row["Id"] as string, row["Name"] as string);

                usersResult = (usersResult.IsMore) ? DataSelect(usersResult) : null;
            }
            while (usersResult != null);

            return result;
        }

        /// <summary>
        /// Get global description of objects.
        /// </summary>
        /// <returns>The SObject types in the org.</returns>
        public SObjectTypePartial[] DataDescribeGlobal()
        {
            InitClients();

            SalesForceAPI.Partner.describeGlobalResponse response = _partnerClient.describeGlobal(new SalesForceAPI.Partner.describeGlobalRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
        public SObjectType DataDescribeObjectType(SObjectTypePartial objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            return DataDescribeObjectType(objectType.Name);
        }

        /// <summary>
        /// Get more detailed description of the object type.
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to get a more detailed description of.</param>
        /// <returns>The full details for the given object.</returns>
        public SObjectType DataDescribeObjectType(string objectTypeName)
        {
            if (objectTypeName == null)
                throw new ArgumentNullException("objectTypeName");

            InitClients();

            SalesForceAPI.Partner.describeSObjectResponse response = _partnerClient.describeSObject(new SalesForceAPI.Partner.describeSObjectRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
                null,
                null,
                null,
                objectTypeName));

            if (response != null && response.result != null)
                return new SObjectType(response.result);

            throw new Exception("Couldn't get details for the given object: " + objectTypeName);
        }

        /// <summary>
        /// Check to see if the checkout system is enabled.
        /// </summary>
        /// <returns>true if checkouts are enabled, false if they are not.</returns>
        public bool IsCheckoutEnabled()
        {
            foreach (SObjectTypePartial obj in DataDescribeGlobal())
                if (obj.Name != null && obj.Name.EndsWith("Walli_Lock_Table__c"))
                    return true;

            return false;
        }

        /// <summary>
        /// Enable checkouts for the entire system.
        /// </summary>
        /// <param name="value">true to enable checkouts, false to disable them.</param>
        public void EnableCheckout(bool value)
        {
            InitClients();

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

            string id = DeployPackage(package, false, false);
            bool complete = false;
            while (!complete)
            {
                PackageDeployResult result = CheckPackageDeploy(id);
                if (result.Status == PackageDeployResultStatus.Failed)
                    throw new Exception("Could not change checkout system: " + result.ResultMessage);

                complete = result.DeploymentComplete;
            }
        }

        /// <summary>
        /// Execute a select query on the server.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult DataSelect(string query)
        {
            return DataSelect(query, false);
        }

        /// <summary>
        /// Execute a select query on the server.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult DataSelect(string query, bool includeStructure)
        {
            InitClients();

            SalesForceAPI.Partner.queryResponse response = _partnerClient.query(new SalesForceAPI.Partner.queryRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
        public DataSelectResult DataSelect(DataSelectResult previousResult)
        {
            return DataSelect(previousResult, false);
        }

        /// <summary>
        /// Execute a select query on the server to get the next batch of data results.
        /// </summary>
        /// <param name="previousResult">The last batch of data results that were received.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The next batch of data results that follow the given previous data results.</returns>
        public DataSelectResult DataSelect(DataSelectResult previousResult, bool includeStructure)
        {
            InitClients();

            SalesForceAPI.Partner.queryMoreResponse response = _partnerClient.queryMore(new SalesForceAPI.Partner.queryMoreRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
        public DataSelectResult DataSelectAll(string query)
        {
            return DataSelectAll(query, false);
        }

        /// <summary>
        /// Execute a select query on the server.  This method returns all records instead of 
        /// chunks of 200 records.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="includeStructure">If true the table structure is set.  i.e. readonly columns are set.</param>
        /// <returns>The result of the query.</returns>
        public DataSelectResult DataSelectAll(string query, bool includeStructure)
        {
            DataSelectResult result = DataSelect(query, includeStructure);
            DataTable data = result.Data;

            while (result.IsMore)
            {
                result = DataSelect(result);
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
        public void DataDelete(DataTable data)
        {
            InitClients();

            if (data == null)
                throw new ArgumentNullException("data");
            ValidateDataHasId(data);

            List<string> ids = new List<string>();
            foreach (DataRow row in data.Rows)
                ids.Add(row["Id", DataRowVersion.Original] as string);

            SalesForceAPI.Partner.deleteResponse response = _partnerClient.delete(new SalesForceAPI.Partner.deleteRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
        public void DataUpdate(DataTable data)
        {
            InitClients();

            if (data == null)
                throw new ArgumentNullException("data");
            ValidateDataHasId(data);

            SalesForceAPI.Partner.sObject[] records = Convert(data, true);
            for (int i = 0; i < records.Length; i += 200)
            {
                SalesForceAPI.Partner.sObject[] buf = new SalesForceAPI.Partner.sObject[Math.Min(200, records.Length - i)];
                Array.Copy(records, i, buf, 0, buf.Length);

                SalesForceAPI.Partner.updateResponse response = _partnerClient.update(new SalesForceAPI.Partner.updateRequest(
                    new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
        public void DataInsert(DataTable data)
        {
            InitClients();

            SalesForceAPI.Partner.sObject[] records = Convert(data, true);
            for (int i = 0; i < records.Length; i += 200)
            {
                SalesForceAPI.Partner.sObject[] buf = new SalesForceAPI.Partner.sObject[Math.Min(200, records.Length - i)];
                Array.Copy(records, i, buf, 0, buf.Length);

                SalesForceAPI.Partner.createResponse response = _partnerClient.create(new SalesForceAPI.Partner.createRequest(
                    new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
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
                objectType = DataDescribeObjectType(first.type);

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

        #region IDisposable Members

        /// <summary>
        /// Cleanup any open connections.
        /// </summary>
        public void Dispose()
        {
            _session.DisposeClient(_partnerClient);
            _session.DisposeClient(_metadataClient);
            _session.DisposeClient(_apexClient);
            _session.DisposeClient(_toolingClient);

            if (_isSessionOwned)
                _session.Dispose();
        }

        #endregion
    }
}
