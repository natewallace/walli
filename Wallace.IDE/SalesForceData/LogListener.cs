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

namespace SalesForceData
{
    /// <summary>
    /// Parameters for what to log.
    /// </summary>
    public class LogListener
    {
        #region Constructors

        /// <summary>
        /// Constructor.
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
        public LogListener(
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
            Id = String.Empty;
            TracedEntityId = traceEntityId;
            TracedEntityName = tracedEntityName;
            ScopeId = scopeId;
            ScopeName = scopeName;
            ExpirationDate = expirationDate;
            CodeLevel = codeLevel;
            VisualForceLevel = visualForceLevel;
            ProfilingLevel = profilingLevel;
            CalloutLevel = calloutLevel;
            DatabaseLevel = databaseLevel;
            SystemLevel = systemLevel;
            ValidationLevel = validationLevel;
            WorkflowLevel = workflowLevel;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="traceFlag">The TraceFlag to build this object from.</param>
        /// <param name="scopeName">ScopeName.</param>
        /// <param name="tracedEntityName">TracedEntityName.</param>        
        internal LogListener(
            SalesForceData.SalesForceAPI.Tooling.TraceFlag traceFlag, 
            string scopeName, 
            string tracedEntityName)
        {
            if (traceFlag == null)
                throw new ArgumentNullException("traceFlag");

            Id = traceFlag.Id;
            TracedEntityId = traceFlag.TracedEntityId;
            TracedEntityName = tracedEntityName;
            ScopeId = traceFlag.ScopeId;
            ScopeName = scopeName;
            ExpirationDate = traceFlag.ExpirationDate.HasValue ? traceFlag.ExpirationDate.Value.ToLocalTime() : DateTime.MinValue;
            CodeLevel = ConvertLogLevel(traceFlag.ApexCode);
            ProfilingLevel = ConvertLogLevel(traceFlag.ApexProfiling);
            CalloutLevel = ConvertLogLevel(traceFlag.Callout);
            DatabaseLevel = ConvertLogLevel(traceFlag.Database);
            SystemLevel = ConvertLogLevel(traceFlag.System);
            ValidationLevel = ConvertLogLevel(traceFlag.Validation);
            VisualForceLevel = ConvertLogLevel(traceFlag.Visualforce);
            WorkflowLevel = ConvertLogLevel(traceFlag.Workflow);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id for this object.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the entity that logging will be done for.  Can be a user id, apex class id, or a trigger id.
        /// </summary>
        public string TracedEntityId { get; set; }

        /// <summary>
        /// The name of the entity that logging will be done for.
        /// </summary>
        public string TracedEntityName { get; set; }

        /// <summary>
        /// The scope of the logging.  This will be either null or the id of a user.
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// The user name for the scope.
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// The date and time the logging will expire.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The log level for apex code.
        /// </summary>
        public LogLevel CodeLevel { get; set; }

        /// <summary>
        /// The log level for visual force.
        /// </summary>
        public LogLevel VisualForceLevel { get; set; }

        /// <summary>
        /// The log level for profiling.
        /// </summary>
        public LogLevel ProfilingLevel { get; set; }

        /// <summary>
        /// The log level for callouts.
        /// </summary>
        public LogLevel CalloutLevel { get; set; }

        /// <summary>
        /// The log level for database calls.
        /// </summary>
        public LogLevel DatabaseLevel { get; set; }

        /// <summary>
        /// The log level for system calls.
        /// </summary>
        public LogLevel SystemLevel { get; set; }

        /// <summary>
        /// The log level for validations.
        /// </summary>
        public LogLevel ValidationLevel { get; set; }

        /// <summary>
        /// The log level for workflows.
        /// </summary>
        public LogLevel WorkflowLevel { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the string value to a LogLevel.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>The converted log level.</returns>
        private LogLevel ConvertLogLevel(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return LogLevel.Error;

            switch (value.ToUpper())
            {
                case "FINEST":
                    return LogLevel.Finest;

                case "FINER":
                    return LogLevel.Finer;

                case "FINE":
                    return LogLevel.Fine;

                case "DEBUG":
                    return LogLevel.Debug;

                case "INFO":
                    return LogLevel.Info;

                case "WARN":
                    return LogLevel.Warn;

                case "ERROR":
                    return LogLevel.Error;

                default:
                    return LogLevel.Error;
            }
        }

        /// <summary>
        /// Convert the given log level to the corresponding string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        private string ConvertLogLevel(LogLevel value)
        {
            return value.ToString().ToUpper();
        }

        /// <summary>
        /// Convert this object into a TraceFlag object.
        /// </summary>
        /// <returns>The converted object.</returns>
        internal SalesForceData.SalesForceAPI.Tooling.TraceFlag ToTraceFlag()
        {
            SalesForceData.SalesForceAPI.Tooling.TraceFlag traceFlag = new SalesForceAPI.Tooling.TraceFlag();

            traceFlag.Id = Id;
            traceFlag.ExpirationDate = ExpirationDate.ToUniversalTime();
            traceFlag.ExpirationDateSpecified = true;
            traceFlag.ApexCode = ConvertLogLevel(CodeLevel);
            traceFlag.ApexProfiling = ConvertLogLevel(ProfilingLevel);
            traceFlag.Callout = ConvertLogLevel(CalloutLevel);
            traceFlag.Database = ConvertLogLevel(DatabaseLevel);
            traceFlag.System = ConvertLogLevel(SystemLevel);
            traceFlag.Validation = ConvertLogLevel(ValidationLevel);
            traceFlag.Visualforce = ConvertLogLevel(VisualForceLevel);
            traceFlag.Workflow = ConvertLogLevel(WorkflowLevel);

            return traceFlag;
        }

        /// <summary>
        /// Equality is based on the id field.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>true if the given object is logically equal to this one.</returns>
        public override bool Equals(object obj)
        {
            LogListener other = obj as LogListener;
            if (other == null)
                return false;

            return (this.Id == other.Id);
        }

        /// <summary>
        /// Get the hashcode for this object.
        /// </summary>
        /// <returns>The hashcode for this object.</returns>
        public override int GetHashCode()
        {
            return (Id == null) ? 0 : Id.GetHashCode();
        }

        #endregion
    }
}
