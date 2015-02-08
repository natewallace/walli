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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// The diagnostic system.
    /// </summary>
    public class DiagnosticSystem
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
        internal DiagnosticSystem(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _client = client;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all of the log listeners that currently exist.
        /// </summary>
        /// <returns>The currently existing log listeners.</returns>
        public LogListener[] GetLogListeners()
        {
            SalesForceAPI.Tooling.queryResponse response = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                String.Format("SELECT Id, ApexCode, ApexProfiling, Callout, Database, ExpirationDate, ScopeId, System, TracedEntityId, Validation, Visualforce, Workflow FROM TraceFlag WHERE ExpirationDate > {0}", DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"))));

            List<LogListener> result = new List<LogListener>();
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

                DataSelectResult nameData = _client.Data.SelectAll(String.Format(
                    "SELECT Id, Name FROM User WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (user)", row["Name"]);

                nameData = _client.Data.SelectAll(String.Format(
                    "SELECT Id, Name FROM ApexClass WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (class)", row["Name"]);

                nameData = _client.Data.SelectAll(String.Format(
                    "SELECT Id, Name FROM ApexTrigger WHERE Id IN ('{0}')",
                    String.Join("','", names.Keys)));
                foreach (DataRow row in nameData.Data.Rows)
                    names[row["Id"] as string] = String.Format("{0} (trigger)", row["Name"]);


                // create log listener
                foreach (SalesForceAPI.Tooling.TraceFlag traceFlag in response.result.records)
                    result.Add(new LogListener(
                        traceFlag,
                        (traceFlag.ScopeId != null) ? names[traceFlag.ScopeId] : String.Empty,
                        (traceFlag.TracedEntityId != null) ? names[traceFlag.TracedEntityId] : String.Empty));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Delete the given log parameters.
        /// </summary>
        /// <param name="logListener">The log listener to delete.</param>
        public void DeleteLogListener(LogListener logListener)
        {
            if (logListener == null)
                throw new ArgumentNullException("logParameters");

            if (String.IsNullOrWhiteSpace(logListener.Id))
                return;

            SalesForceAPI.Tooling.deleteResponse response = _client.ToolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new string[] { logListener.Id }));

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
        /// Create new log listener.
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
        /// <returns>The newly created log listener.</returns>
        public LogListener CreateLogListener(
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

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
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

            return new LogListener(traceFlag, scopeName, tracedEntityName);
        }

        /// <summary>
        /// Update the given log listener.
        /// </summary>
        /// <param name="logListener">The log listener to update.</param>
        public void UpdateLogListener(LogListener logListener)
        {
            if (logListener == null)
                throw new ArgumentNullException("logParameters");

            SalesForceAPI.Tooling.updateResponse response = _client.ToolingClient.update(new SalesForceAPI.Tooling.updateRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                new SalesForceAPI.Tooling.sObject[] 
                {
                    logListener.ToTraceFlag()
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
            SalesForceAPI.Tooling.queryResponse response = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
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

            return _client.ExecuteRestCall(String.Format("/sobjects/ApexLog/{0}/Body/", log.Id));
        }

        /// <summary>
        /// Delete the given logs.
        /// </summary>
        /// <param name="logs">The logs to delete.</param>
        public void DeleteLogs(IEnumerable<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException("logs");

            List<string> logIds = new List<string>();
            foreach (Log log in logs)
                logIds.Add(log.Id);

            SalesForceAPI.Tooling.deleteResponse response = _client.ToolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
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
            SalesForceAPI.Tooling.queryResponse response = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                String.Format("SELECT Id, ActionScript, ActionScriptType, ExecutableEntityId, ExpirationDate, IsDumpingHeap, Iteration, Line, ScopeId FROM ApexExecutionOverlayAction WHERE ScopeId = '{0}'", _client.User.Id)));

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

                DataSelectResult names = _client.Data.Select(String.Format(
                    "SELECT Id, Name FROM ApexClass WHERE Id IN ('{0}')",
                    String.Join("','", idToFileNameMap.Keys)));
                foreach (DataRow row in names.Data.Rows)
                {
                    idToFileNameMap.Remove(row["Id"] as string);
                    idToFileNameMap.Add(row["Id"] as string, row["Name"] as string);
                }

                names = _client.Data.Select(String.Format(
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

            // get record id
            string entityId = null;
            switch (file.FileType.Name)
            {
                case "ApexClass":
                case "ApexTrigger":
                    DataSelectResult objectQueryResult = _client.Data.Select(String.Format("SELECT id FROM {0} WHERE Name = '{1}'", file.FileType.Name, file.Name));
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
            action.ScopeId = _client.User.Id;

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
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
        /// Delete a checkpoint.
        /// </summary>
        /// <param name="checkpoint">The checkpoint to delete.</param>
        public void DeleteCheckpoint(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                throw new ArgumentNullException("checkpoint");

            SalesForceAPI.Tooling.deleteResponse response = _client.ToolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
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
        /// Execute the given snippet of code on the server.
        /// </summary>
        /// <param name="code">The code to be executed.</param>
        /// <returns>null of the operation succeeds or a SalesForceError if it fails.</returns>
        public SalesForceError ExecuteSnippet(string code)
        {
            string path = String.Format(
                "/tooling/executeAnonymous/?anonymousBody={0}", 
                System.Net.WebUtility.UrlEncode(code));

            string response = _client.ExecuteRestCall(path);

            if (response == null)
                return new SalesForceError(null, "Invalid response", null);

            JObject json = JObject.Parse(response);
            if (json == null)
                return new SalesForceError(null, "Invalid JSON response", null);

            if (String.Compare(json["success"].Value<string>(), "True", true) != 0)
            {
                StringBuilder sb = new StringBuilder();

                string compiled = json["compiled"].Value<string>();
                string compileProblem = json["compileProblem"].Value<string>();
                string exceptionMessage = json["exceptionMessage"].Value<string>();
                string exceptionStackTrace = json["exceptionStackTrace"].Value<string>();
                string line = json["line"].Value<string>();
                string column = json["column"].Value<string>();

                if (String.Compare(compiled, "True", true) != 0)
                {
                    sb.AppendLine(compileProblem);
                    sb.AppendFormat("Line: {0} Column: {1}", line, column);
                    return new SalesForceError("COMPILE FAILURE", sb.ToString(), null);
                }
                else
                {
                    sb.AppendLine(exceptionMessage);
                    sb.AppendLine();
                    sb.Append(exceptionStackTrace);
                    return new SalesForceError("EXCEPTION", sb.ToString(), null);
                }
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
