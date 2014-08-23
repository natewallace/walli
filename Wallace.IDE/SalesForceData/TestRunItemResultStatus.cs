using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// The possible status for a test run item result.
    /// </summary>
    public enum TestRunItemResultStatus
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Pass.
        /// </summary>
        Pass,

        /// <summary>
        /// Fail.
        /// </summary>
        Fail,

        /// <summary>
        /// CompileFail.
        /// </summary>
        CompileFail
    }
}
