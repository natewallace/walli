/*
 * Copyright (c) 2014 Nathaniel Wallace
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
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace SalesForceData
{
    /// <summary>
    /// This object initates and maintains a SalesForce session.
    /// </summary>
    public class SalesForceSession : IDisposable
    {
        #region Fields

        /// <summary>
        /// Credential used to login.
        /// </summary>
        private SalesForceCredential _credential;

        /// <summary>
        /// Holds the underlying session information.
        /// </summary>
        private SalesForceAPI.Partner.LoginResult _session;

        /// <summary>
        /// Records the last time the session was used.
        /// </summary>
        private DateTime _lastUsed;

        /// <summary>
        /// Holds the configuration for services.
        /// </summary>
        private Configuration _configuration;

        /// <summary>
        /// Supports PartnerClientFactory property.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Partner.Soap> _partnerClientFactory;

        /// <summary>
        /// Supports MetadataClientFactory property.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Metadata.MetadataPortType> _metadataClientFactory;

        /// <summary>
        /// Supports ApexClientFactory property.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Apex.ApexPortType> _apexClientFactory;

        /// <summary>
        /// Supports ToolingClientFactory.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Tooling.SforceServicePortType> _toolingClientFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credential">The credential to login with.</param>
        /// <param name="configuration">Used to configure connections.</param>
        public SalesForceSession(SalesForceCredential credential, Configuration configuration)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _credential = credential;
            _configuration = configuration;
            _session = null;
            _lastUsed = DateTime.Now;

            _partnerClientFactory = null;
            _metadataClientFactory = null;
            _apexClientFactory = null;
            _toolingClientFactory = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the id for this session.
        /// </summary>
        public string Id
        {
            get
            {
                if (_session == null || DateTime.Now - _lastUsed > TimeSpan.FromMinutes(10))
                    Login();

                _lastUsed = DateTime.Now;
                return _session.sessionId;
            }
        }

        /// <summary>
        /// The id of the user that is logged in.
        /// </summary>
        public string UserId
        {
            get
            {
                return _session.userId;
            }
        }

        /// <summary>
        /// The display name of the user that is logged in.
        /// </summary>
        public string UserName
        {
            get
            {
                return _session.userInfo.userFullName;
            }
        }

        /// <summary>
        /// A uri that can be used to login to the salesforce website using this session.
        /// </summary>
        public string WebsiteAutoLoginUri
        {
            get
            {
                Uri uri = new Uri(_session.serverUrl);
                return String.Format("https://{0}/secur/frontdoor.jsp?sid={1}", uri.Host, _session.sessionId);
            }
        }

        /// <summary>
        /// The base url for rest calls.
        /// </summary>
        private string RestBaseUrl { get; set; }

        /// <summary>
        /// Used to create Partner client channels.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Partner.Soap> PartnerClientFactory
        {
            get
            {
                if (_partnerClientFactory == null)
                {
                    _partnerClientFactory = new ConfigurationChannelFactory<SalesForceAPI.Partner.Soap>(
                        String.Format("SalseForceAPI.Partner.{0}", _credential.Domain),
                        _configuration,
                        null);
                }

                return _partnerClientFactory;
            }
        }

        /// <summary>
        /// Used to create Metadata client channels.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Metadata.MetadataPortType> MetadataClientFactory
        {
            get
            {
                if (_metadataClientFactory == null)
                {
                    _metadataClientFactory = new ConfigurationChannelFactory<SalesForceAPI.Metadata.MetadataPortType>(
                        "SalesForceAPI.Metadata",
                        _configuration,
                        null);
                }

                return _metadataClientFactory;
            }
        }

        /// <summary>
        /// Used to create Apex client channels.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Apex.ApexPortType> ApexClientFactory
        {
            get
            {
                if (_apexClientFactory == null)
                {
                    _apexClientFactory = new ConfigurationChannelFactory<SalesForceAPI.Apex.ApexPortType>(
                        "SalesForceAPI.Apex",
                        _configuration,
                        null);
                }

                return _apexClientFactory;
            }
        }

        /// <summary>
        /// Used to create Tooling client channels.
        /// </summary>
        private ConfigurationChannelFactory<SalesForceAPI.Tooling.SforceServicePortType> ToolingClientFactory
        {
            get
            {
                if (_toolingClientFactory == null)
                {
                    _toolingClientFactory = new ConfigurationChannelFactory<SalesForceAPI.Tooling.SforceServicePortType>(
                        "SalesForceAPI.Tooling",
                        _configuration,
                        null);
                }

                return _toolingClientFactory;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Login to SalesForce.
        /// </summary>
        private void Login()
        {
            SalesForceAPI.Partner.Soap loginClient = PartnerClientFactory.CreateChannel();
            SalesForceAPI.Partner.loginResponse response = loginClient.login(new SalesForceAPI.Partner.loginRequest(
                null, 
                null, 
                _credential.Username, 
                _credential.Password + _credential.Token));
            _session = response.result;
            DisposeClient(loginClient);

            PartnerClientFactory.Endpoint.Address = new System.ServiceModel.EndpointAddress(_session.serverUrl);
            MetadataClientFactory.Endpoint.Address = new System.ServiceModel.EndpointAddress(_session.metadataServerUrl);
            ApexClientFactory.Endpoint.Address = new System.ServiceModel.EndpointAddress(_session.serverUrl.Replace("/u/", "/s/"));
            ToolingClientFactory.Endpoint.Address = new System.ServiceModel.EndpointAddress(_session.serverUrl.Replace("/u/", "/T/"));

            RestBaseUrl = String.Format("https://{0}/services/data/v32.0", new Uri(_session.serverUrl).Host);
        }

        /// <summary>
        /// Create a client to call Partner methods with.
        /// </summary>
        /// <returns>A client to call Partner methods with</returns>
        public SalesForceAPI.Partner.Soap CreatePartnerClient()
        {
            if (_session == null)
                Login();

            return PartnerClientFactory.CreateChannel();
        }

        /// <summary>
        /// Create a client to call Metadata methods with.
        /// </summary>
        /// <returns>A client to call Metadata methods with</returns>
        public SalesForceAPI.Metadata.MetadataPortType CreateMetadataClient()
        {
            if (_session == null)
                Login();

            return MetadataClientFactory.CreateChannel();
        }

        /// <summary>
        /// Create a client to call Apex methods with.
        /// </summary>
        /// <returns>A client to call Apex methods with</returns>
        public SalesForceAPI.Apex.ApexPortType CreateApexClient()
        {
            if (_session == null)
                Login();

            return ApexClientFactory.CreateChannel();
        }

        /// <summary>
        /// Create a client to call Tooling methods with.
        /// </summary>
        /// <returns>A client to call Tooling methods with</returns>
        public SalesForceAPI.Tooling.SforceServicePortType CreateToolingClient()
        {
            if (_session == null)
                Login();

            return ToolingClientFactory.CreateChannel();
        }

        /// <summary>
        /// Execute a rest call.
        /// </summary>
        /// <param name="path">The relative path for the rest call.</param>
        /// <returns>The result from the call.</returns>
        public string ExecuteRestCall(string path)
        {
            if (path == null)
                path = String.Empty;
            else
                path = path.Trim();

            string url = (!path.StartsWith("/") && !path.StartsWith("\\")) ?
                String.Format("{0}/{1}", RestBaseUrl, path) :
                String.Format("{0}{1}", RestBaseUrl, path);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Authorization", String.Format("Bearer {0}", Id));

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response == null)
                throw new Exception("Response was not of type HttpWebResponse");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(String.Format("REST call failed: {0} {1}", response.StatusCode, response.StatusDescription));

            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Dispose of the given client.
        /// </summary>
        /// <param name="client">The client to dispose of.  Must implement ICommunicationObject.</param>
        public void DisposeClient(object client)
        {
            if (client == null)
                return;

            ICommunicationObject clientComm = client as ICommunicationObject;
            if (clientComm == null)
                throw new ArgumentException("The given client isn't an ICommunicationObject type.", "client");

            if (clientComm.State == CommunicationState.Faulted)
                clientComm.Abort();
            else
                clientComm.Close();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Close all the open factories.
        /// </summary>
        public void Dispose()
        {
            if (_partnerClientFactory != null)
                DisposeClient(_partnerClientFactory);

            if (_metadataClientFactory != null)
                DisposeClient(_metadataClientFactory);

            if (_apexClientFactory != null)
                DisposeClient(_apexClientFactory);

            if (_toolingClientFactory != null)
                DisposeClient(_toolingClientFactory);
        }

        #endregion
    }
}
