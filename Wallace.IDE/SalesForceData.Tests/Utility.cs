using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace SalesForceData.Tests
{
    /// <summary>
    /// Helper class for tests.
    /// </summary>
    public class Utility
    {
        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Utility()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            SalesForceDataConfiguration = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap() { ExeConfigFilename = "SalesForceData.dll.config" },
                ConfigurationUserLevel.None);

            #region Sensitive

            TestCredential = new SalesForceCredential(
                SalesForceDomain.Sandbox,
                "nwallace@unum.com.poc", 
                "kinghipp02",
                "AyCKupIcFkRis7E7ymxhrle2");

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration for the SalesForceData assembly.
        /// </summary>
        public static Configuration SalesForceDataConfiguration { get; private set; }

        /// <summary>
        /// The credential to perform tests with.
        /// </summary>
        public static SalesForceCredential TestCredential { get; private set; }

        #endregion
    }
}
