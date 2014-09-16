using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// Possible status for a package deployment.
    /// </summary>
    public enum PackageDeployResultStatus
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Pending.
        /// </summary>
        Pending,

        /// <summary>
        /// In progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Succeeded partial.
        /// </summary>
        SucceededPartial,

        /// <summary>
        /// Failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Canceling.
        /// </summary>
        Canceling,

        /// <summary>
        /// Canceled.
        /// </summary>
        Canceled
    }
}
