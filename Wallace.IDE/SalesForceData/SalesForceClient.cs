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

            Data = new DataSystem(this);
            Meta = new MetaSystem(this);
            Checkout = new CheckoutSystem(this);
            Test = new TestSystem(this);
            Diagnostic = new DiagnosticSystem(this);
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

            Data = new DataSystem(this);
            Meta = new MetaSystem(this);
            Checkout = new CheckoutSystem(this);
            Test = new TestSystem(this);
            Diagnostic = new DiagnosticSystem(this);
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

            Data = new DataSystem(this);
            Meta = new MetaSystem(this);
            Checkout = new CheckoutSystem(this);
            Test = new TestSystem(this);
            Diagnostic = new DiagnosticSystem(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The session.
        /// </summary>
        internal string SessionId
        {
            get
            {
                return _session.Id;
            }
        }

        /// <summary>
        /// The partner client.
        /// </summary>
        internal SalesForceAPI.Partner.Soap PartnerClient
        {
            get 
            {
                InitClients();
                return _partnerClient; 
            }
        }

        /// <summary>
        /// The metadata client.
        /// </summary>
        internal SalesForceAPI.Metadata.MetadataPortType MetadataClient
        {
            get
            {
                InitClients();
                return _metadataClient;
            }
        }

        /// <summary>
        /// The apex client.
        /// </summary>
        internal SalesForceAPI.Apex.ApexPortType ApexClient
        {
            get
            {
                InitClients();
                return _apexClient;
            }
        }

        /// <summary>
        /// The tooling client.
        /// </summary>
        internal SalesForceAPI.Tooling.SforceServicePortType ToolingClient
        {
            get
            {
                InitClients();
                return _toolingClient;
            }
        }

        /// <summary>
        /// The currently logged on user.
        /// </summary>
        public User User
        {
            get 
            {
                InitClients();
                return _session.User; 
            }
        }

        /// <summary>
        /// The user email.
        /// </summary>
        public string UserEmail
        {
            get 
            {
                InitClients();
                return _session.UserEmail; 
            }
        }

        /// <summary>
        /// The namespace for the organization.
        /// </summary>
        public string Namespace
        {
            get { return GetOrgInfo().organizationNamespace; }
        }

        /// <summary>
        /// The data system.
        /// </summary>
        public DataSystem Data { get; private set; }

        /// <summary>
        /// The meta system.
        /// </summary>
        public MetaSystem Meta { get; private set; }

        /// <summary>
        /// The checkout system.
        /// </summary>
        public CheckoutSystem Checkout { get; private set; }

        /// <summary>
        /// The test system.
        /// </summary>
        public TestSystem Test { get; private set; }

        /// <summary>
        /// The diagnostic system.
        /// </summary>
        public DiagnosticSystem Diagnostic { get; private set; }

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
        /// Execute a REST call.
        /// </summary>
        /// <param name="path">The relative path for the call.</param>
        /// <returns>The result from the call.</returns>
        internal string ExecuteRestCall(string path)
        {
            return _session.ExecuteRestCall(path);
        }

        /// <summary>
        /// Get the org info for this client.
        /// </summary>
        /// <returns>The org info for this client.</returns>
        internal SalesForceAPI.Metadata.DescribeMetadataResult GetOrgInfo()
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
        /// Search for users.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <returns>The users that match the search query.</returns>
        public User[] SearchUsers(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                throw new ArgumentException("query is null or whitespace", "query");

            InitClients();

            SalesForceAPI.Partner.searchRequest request = new SalesForceAPI.Partner.searchRequest(
                new SalesForceAPI.Partner.SessionHeader() { sessionId = _session.Id },
                null,
                null,
                String.Format("FIND {{{0}}} IN ALL FIELDS RETURNING User (Id, Name)", query));

            SalesForceAPI.Partner.searchResponse response = _partnerClient.search(request);

            if (response == null || response.result == null)
                throw new Exception("Invalid response received.");

            List<User> results = new List<User>();
            if (response.result.searchRecords != null)
            {
                foreach (SalesForceAPI.Partner.SearchRecord searchRecord in response.result.searchRecords)
                {
                    if (searchRecord.record != null && searchRecord.record.Any != null)
                    {
                        string id = null;
                        string name = null;

                        foreach (System.Xml.XmlElement e in searchRecord.record.Any)
                        {
                            string value = null;
                            if (searchRecord.record.fieldsToNull != null && searchRecord.record.fieldsToNull.Contains(e.LocalName))
                                value = null;
                            else
                                value = e.InnerText;

                            if (String.Compare("Id", e.LocalName, true) == 0)
                                id = value;
                            else if (String.Compare("Name", e.LocalName, true) == 0)
                                name = value;
                        }

                        if (id != null && name != null)
                            results.Add(new User(id, name));
                    }
                }
            }

            return results.ToArray();
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
