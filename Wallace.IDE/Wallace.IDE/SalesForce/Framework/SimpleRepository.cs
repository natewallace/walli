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
        /// The name of the local branch to work in.
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// The local branch to workin.
        /// </summary>
        private Branch Branch { get; set; }

        /// <summary>
        /// The username used to login to the remote with.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password used to login to the remote with.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// If true then this repository is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(RemoteUrl) &&
                        !String.IsNullOrWhiteSpace(BranchName) &&
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
            if (String.IsNullOrWhiteSpace(BranchName))
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

            // clean out existing repository if it doesn't match current settings
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

            // clone repository if needed
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

            FetchOptions fetchOptions = null;

            try
            {
                // get latest changes
                fetchOptions = new FetchOptions();
                fetchOptions.CredentialsProvider += ProvideCredentials;
                repo.Fetch("origin", fetchOptions);

                // get branch or create new one if needed
                Branch localBranch = repo.Branches[BranchName];
                if (localBranch == null)
                {
                    Branch remoteBranch = repo.Branches[String.Format("origin/{0}", BranchName)];
                    if (remoteBranch == null)
                        throw new Exception(String.Format("There is no branch named: {0} in the repository.", BranchName));

                    localBranch = repo.CreateBranch(BranchName, remoteBranch.Tip);
                    localBranch = repo.Branches.Update(
                        localBranch,
                        b => b.TrackedBranch = remoteBranch.CanonicalName);
                }

                Branch = localBranch;

                // checkout branch and wipe out any uncommitted changes
                CheckoutOptions checkoutOptions = new CheckoutOptions();
                checkoutOptions.CheckoutModifiers = CheckoutModifiers.Force;
                repo.Checkout(localBranch, checkoutOptions);
            }
            finally
            {
                if (fetchOptions != null)
                    fetchOptions.CredentialsProvider -= ProvideCredentials;
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
                PushOptions pushOptions = null;

                try
                {
                    // extract package to the working directory
                    string[] files = Package.Extract(package, WorkingPath, true, false);

                    // stage, commit, and then push
                    repo.Stage(files);

                    Signature author = new Signature(authorName, authorEmail, DateTime.Now);
                    repo.Commit(comment, author);

                    pushOptions = new PushOptions();
                    pushOptions.CredentialsProvider += ProvideCredentials;
                    repo.Network.Push(Branch, pushOptions);
                }
                finally
                {
                    if (pushOptions != null)
                        pushOptions.CredentialsProvider -= ProvideCredentials;
                }
            }
        }

        /// <summary>
        /// Get the commits that involve changes to the given file.
        /// </summary>
        /// <param name="file">The file to get revisions for.</param>
        /// <returns>The revisions that occured for the given file.</returns>
        public SimpleRepositoryCommit[] GetHistory(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            Validate();

            List<SimpleRepositoryCommit> result = new List<SimpleRepositoryCommit>();
            const int MAX_RESULTS = 50;
            const int MAX_COMMITS = 1000;
            const int MAX_DEPTH = 20;

            using (App.Wait("Getting history."))
            {
                using (Repository repo = Init())
                {
                    Commit commit = repo.Head.Tip;
                    string targetSha = null;

                    HashSet<string> shaSet = new HashSet<string>();
                    Queue<Commit> commitQueue = new Queue<Commit>();                    

                    bool go = true;
                    int commitCount = 0;

                    commitQueue.Enqueue(commit);
                    while (go && commitQueue.Count > 0)
                    {
                        commit = commitQueue.Dequeue();

                        // get target sha
                        TreeEntry target = commit[file.FileName];
                        if (target != null)
                            targetSha = target.Target.Sha;

                        // compare the target sha against parent commits
                        foreach (Commit parent in commit.Parents)
                        {
                            if (!go)
                                break;

                            TreeEntry tree = parent[file.FileName];
                            if (tree == null)
                                continue;

                            if (tree.Target.Sha != targetSha && shaSet.Add(commit.Sha))
                            {
                                result.Add(new SimpleRepositoryCommit(commit));
                                if (result.Count >= MAX_RESULTS)
                                    go = false;
                            }

                            if (commitQueue.Count < MAX_DEPTH)
                                commitQueue.Enqueue(parent);

                            commitCount++;
                            if (commitCount >= MAX_COMMITS)
                                go = false;
                        }
                    }
                }
            }

            return result.OrderByDescending(c => c.Date).ToArray();
        }

        /// <summary>
        /// Get the content of the file for the given commit.
        /// </summary>
        /// <param name="file">The file to get content for.</param>
        /// <param name="commit">The commit to get content from.</param>
        /// <returns>The requested content as of the given commit.</returns>
        public string GetContent(SourceFile file, SimpleRepositoryCommit commit)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (commit == null)
                throw new ArgumentNullException("commit");

            Validate();

            using (Repository repo = Init())
            {
                CheckoutOptions checkoutOptions = new CheckoutOptions();
                checkoutOptions.CheckoutModifiers = CheckoutModifiers.Force;

                // get older version text
                repo.CheckoutPaths(
                    commit.Sha,
                    new string[] { file.FileName },
                    checkoutOptions);

                return File.ReadAllText(Path.Combine(WorkingPath, file.FileName));
            }
        }

        /// <summary>
        /// Get the difference between the two versions of the same file.
        /// </summary>
        /// <param name="file">The file to get differences for.</param>
        /// <param name="older">The older version of the file.</param>
        /// <param name="newer">The newer version of the file.</param>
        /// <returns>A patch file that describes the differences.</returns>
        public string Diff(SourceFile file, SimpleRepositoryCommit older, SimpleRepositoryCommit newer)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (older == null)
                throw new ArgumentNullException("older");
            if (newer == null)
                throw new ArgumentNullException("newer");

            Validate();

            using (Repository repo = Init())
            {
                CheckoutOptions checkoutOptions = new CheckoutOptions();
                checkoutOptions.CheckoutModifiers = CheckoutModifiers.Force;

                // get older version text
                repo.CheckoutPaths(
                    older.Sha,
                    new string[] { file.FileName },
                    checkoutOptions);

                string olderText = File.ReadAllText(Path.Combine(WorkingPath, file.FileName));

                // get newer version text
                repo.CheckoutPaths(
                    newer.Sha,
                    new string[] { file.FileName },
                    checkoutOptions);

                string newerText = File.ReadAllText(Path.Combine(WorkingPath, file.FileName));

                // do diff
                //TODO:
            }

            return null;   
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
            BranchName = reader["branch"];
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
            writer.WriteAttributeString("branch", BranchName);
            writer.WriteAttributeString("username", Username);
            writer.WriteAttributeString("password", Password);
        }

        #endregion
    }
}
