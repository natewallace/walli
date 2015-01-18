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

using LibGit2Sharp;
using SalesForceData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// A very stripped down implementation of a source control repository.
    /// </summary>
    public class SimpleRepository : IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimpleRepository()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// The working path for the repository.
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// The url of the remote repository that changes will be pushed to.
        /// </summary>
        public string RemoteUrl { get; set; }

        /// <summary>
        /// The branch to work in.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// The username used to login to the remote with.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password used to login to the remote with.
        /// </summary>
        public string Password { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// If true then this repository is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(RemoteUrl) &&
                        !String.IsNullOrWhiteSpace(Branch) &&
                        Directory.Exists(WorkingPath));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Throws an exception if this repository isn't valid.
        /// </summary>
        private void Validate()
        {
            if (String.IsNullOrWhiteSpace(RemoteUrl))
                throw new Exception("RemoteUrl is not set.");
            if (String.IsNullOrWhiteSpace(Branch))
                throw new Exception("Branch is not set.");
            if (!Directory.Exists(WorkingPath))
                throw new Exception("WorkingPath doesn't exist: " + WorkingPath);
        }

        /// <summary>
        /// Get repository.
        /// </summary>
        public Repository Init()
        {
            Validate();

            Repository repo = null;

            string[] existingFiles = Directory.GetFiles(WorkingPath);
            string[] existingFolders = Directory.GetDirectories(WorkingPath);

            if ((existingFiles != null && existingFiles.Length > 0) ||
                (existingFolders != null && existingFolders.Length > 0))
            {
                repo = new Repository(WorkingPath);
                Remote remote = repo.Network.Remotes["origin"];
                if (remote == null || remote.Url != RemoteUrl)
                {
                    repo = null;

                    foreach (string file in existingFiles)
                        File.Delete(file);

                    foreach (string directory in existingFolders)
                        Directory.Delete(directory, true);
                }
            }

            if (repo == null)
            {
                CloneOptions options = new CloneOptions();
                options.CredentialsProvider += ProvideCredentials;
                try
                {
                    repo = new Repository(Repository.Clone(RemoteUrl, WorkingPath, options));
                }
                finally
                {
                    options.CredentialsProvider -= ProvideCredentials;
                }
            }

            return repo;
        }

        /// <summary>
        /// Push the contents of the package to the remote.
        /// </summary>
        /// <param name="package">The package to push.</param>
        /// <param name="comment">The comment for the commit.</param>
        /// <param name="authorName">The author of the commit.</param>
        /// <param name="authorEmail">The author's email.</param>
        public void PushPackage(byte[] package, string comment, string authorName, string authorEmail)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            Validate();

            using (Repository repo = Init())
            {
                FetchOptions fetchOptions = null;
                PushOptions pushOptions = null;

                try
                {
                    // get latest changes
                    fetchOptions = new FetchOptions();
                    fetchOptions.CredentialsProvider += ProvideCredentials;
                    repo.Fetch("origin", fetchOptions);

                    // get branch or create new one if needed
                    Branch localBranch = repo.Branches[Branch];
                    if (localBranch == null)
                    {
                        Branch remoteBranch = repo.Branches[String.Format("origin/{0}", Branch)];
                        if (remoteBranch == null)
                            throw new Exception(String.Format("There is no branch named: {0} in the repository.", Branch));

                        localBranch = repo.CreateBranch(Branch, remoteBranch.Tip);
                        localBranch = repo.Branches.Update(
                            localBranch,
                            b => b.TrackedBranch = remoteBranch.CanonicalName);
                    }

                    // checkout branch and wipe out any uncommitted changes
                    CheckoutOptions checkoutOptions = new CheckoutOptions();
                    checkoutOptions.CheckoutModifiers = CheckoutModifiers.Force;
                    repo.Checkout(localBranch, checkoutOptions);

                    // extract package to the working directory
                    string[] files = Package.Extract(package, WorkingPath, true, false);

                    // stage, commit, and then push
                    repo.Stage(files);

                    Signature author = new Signature(authorName, authorEmail, DateTime.Now);
                    repo.Commit(comment, author);

                    pushOptions = new PushOptions();
                    pushOptions.CredentialsProvider += ProvideCredentials;
                    repo.Network.Push(localBranch, pushOptions);
                }
                finally
                {
                    if (fetchOptions != null)
                        fetchOptions.CredentialsProvider -= ProvideCredentials;
                    if (pushOptions != null)
                        pushOptions.CredentialsProvider -= ProvideCredentials;
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Provide the credentials from this object.
        /// </summary>
        /// <param name="url">The url to provide credentials for.</param>
        /// <param name="usernameFromUrl">The user to provide credentials for.</param>
        /// <param name="types">The type of credentials to provide.</param>
        /// <returns>The credentials to use.</returns>
        private Credentials ProvideCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            UsernamePasswordCredentials c = new UsernamePasswordCredentials();
            c.Username = Username;
            c.Password = Password;

            return c;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Not used.
        /// </summary>
        /// <returns>null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read this object in from file.
        /// </summary>
        /// <param name="reader">The strea to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            RemoteUrl = reader["remoteUrl"];
            Branch = reader["branch"];
            Username = reader["username"];
            Password = reader["password"];
        }

        /// <summary>
        /// Write this object to file.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("remoteUrl", RemoteUrl);
            writer.WriteAttributeString("branch", Branch);
            writer.WriteAttributeString("username", Username);
            writer.WriteAttributeString("password", Password);
        }

        #endregion
    }
}
