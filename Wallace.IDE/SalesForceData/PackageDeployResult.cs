using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// The result of a package that has been deployed or is in the process of being deployed.
    /// </summary>
    public class PackageDeployResult
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">The result info to build this object from.</param>
        internal PackageDeployResult(SalesForceAPI.Metadata.DeployResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            Id = result.id;
            DeploymentComplete = result.done;
            ComponentsTotalCount = result.numberComponentsTotal;
            ComponentsFailCount = result.numberComponentErrors;
            ComponentsPassCount = result.numberComponentsDeployed;
            TestsTotalCount = result.numberTestsTotal;
            TestsPassCount = result.numberTestsCompleted;
            TestsFailCount = result.numberTestErrors;

            switch (result.status)
            {
                case SalesForceAPI.Metadata.DeployStatus.Canceled:
                    Status = PackageDeployResultStatus.Canceled;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.Canceling:
                    Status = PackageDeployResultStatus.Canceling;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.Failed:
                    Status = PackageDeployResultStatus.Failed;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.InProgress:
                    Status = PackageDeployResultStatus.InProgress;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.Pending:
                    Status = PackageDeployResultStatus.Pending;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.Succeeded:
                    Status = PackageDeployResultStatus.Succeeded;
                    break;

                case SalesForceAPI.Metadata.DeployStatus.SucceededPartial:
                    Status = PackageDeployResultStatus.SucceededPartial;
                    break;

                default:
                    Status = PackageDeployResultStatus.Unknown;
                    break;
            }

            if (result.done)
            {
                StringBuilder sb = new StringBuilder();
                if (!result.success)
                {
                    sb.AppendLine("Deployment failed.");
                    if (!String.IsNullOrWhiteSpace(result.errorMessage))
                    {
                        sb.AppendFormat("ERROR: {0} ", result.errorMessage);
                        if (result.errorStatusCodeSpecified)
                            sb.AppendFormat("({0})", result.errorStatusCode);
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.AppendLine("Deployment succeeded.");
                }

                sb.AppendLine();
                sb.AppendFormat("{0} of {1} components deployed with {2} failure(s).", result.numberComponentsDeployed, result.numberComponentsTotal, result.numberComponentErrors);
                sb.AppendLine();
                if (result.details != null && result.details.componentFailures != null)
                {
                    sb.AppendLine("----------------------------------------");
                    foreach (SalesForceAPI.Metadata.DeployMessage message in result.details.componentFailures)
                    {
                        sb.AppendLine();
                        if (message.problemTypeSpecified)
                        {
                            sb.AppendFormat("  Problem: {0} - {1}", message.problemType, message.problem);
                            sb.AppendLine();
                        }
                        else
                        {
                            sb.AppendFormat("  Problem: {0}", message.problem);
                            sb.AppendLine();
                        }
                        sb.AppendFormat("Component: {0}", message.fullName);
                        sb.AppendLine();
                        sb.AppendFormat("     File: {0}", message.fileName);
                        sb.AppendLine();
                        if (message.lineNumberSpecified && message.columnNumberSpecified)
                        {
                            sb.AppendFormat(" Position: Line {0} Column {1}", message.lineNumber, message.columnNumber);
                            sb.AppendLine();
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine("----------------------------------------");
                    sb.AppendLine();
                }

                sb.AppendLine();

                sb.AppendFormat("{0} of {1} tests passed with {2} failures.", result.numberTestsCompleted, result.numberTestsTotal, result.numberTestErrors);
                sb.AppendLine();
                if (result.details != null && result.details.runTestResult != null && result.details.runTestResult.failures != null)
                {
                    sb.AppendLine("----------------------------------------");
                    foreach (SalesForceAPI.Metadata.RunTestFailure failure in result.details.runTestResult.failures)
                    {
                        sb.AppendLine();
                        sb.AppendFormat("      Error: {0}", failure.message);
                        sb.AppendLine();
                        sb.AppendFormat("     Method: {0}.{1}", failure.name, failure.methodName);
                        sb.AppendLine();
                        sb.AppendFormat("Stack trace: {0}", (failure.stackTrace != null) ? failure.stackTrace.Replace("\n", Environment.NewLine + "             ") : String.Empty);
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    sb.AppendLine("----------------------------------------");
                    sb.AppendLine();
                }

                sb.AppendLine();

                ResultMessage = sb.ToString();
            }
            else
            {
                ResultMessage = "Pending";
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id that identifies the deployment.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The status of the deployment.
        /// </summary>
        public PackageDeployResultStatus Status { get; private set; }

        /// <summary>
        /// Flag that indicates if the deployment is complete.
        /// </summary>
        public bool DeploymentComplete { get; private set; }

        /// <summary>
        /// The total number of components that are in the deployment.
        /// </summary>
        public int ComponentsTotalCount { get; private set; }

        /// <summary>
        /// The number of components that failed to deploy.
        /// </summary>
        public int ComponentsFailCount { get; private set; }

        /// <summary>
        /// The number of components that were deployed successfully.
        /// </summary>
        public int ComponentsPassCount { get; private set; }

        /// <summary>
        /// The total number of tests.
        /// </summary>
        public int TestsTotalCount { get; private set; }

        /// <summary>
        /// The number of tests that failed.
        /// </summary>
        public int TestsFailCount { get; private set; }

        /// <summary>
        /// The number of tests that have been run.
        /// </summary>
        public int TestsPassCount { get; private set; }

        /// <summary>
        /// The result message to display.
        /// </summary>
        public string ResultMessage { get; private set; }

        #endregion

    }
}
