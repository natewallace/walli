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
        /// A sub folder under the branch where commits are made.
        /// </summary>
        public string SubFolder { get; set; }

        /// <summary>
        /// The username used to login to the remote with.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password used to login to the remote with.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The author name for any repository changes.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// The author email for any repository changes.
        /// </summary>
        public string AuthorEmail { get; set; }

        /// <summary>
        /// If true then this repository is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                string workPath = ParseLocalUrl(RemoteUrl) ?? WorkingPath;
                return (!String.IsNullOrWhiteSpace(RemoteUrl) &&
                        !String.IsNullOrWhiteSpace(BranchName) &&
                        Directory.Exists(workPath));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the url to get the local path.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <returns>The local path if the url is a local url, null if it's not.</returns>
        private string ParseLocalUrl(string url)
        {
            if (url == null)
                return null;

            url = url.Trim();
            if (url.StartsWith("local://", StringComparison.CurrentCultureIgnoreCase))
            {
                int index = url.IndexOf("//");
                return url.Substring(index + 2);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Throws an exception if this repository isn't valid.
        /// </summary>
        private void Validate()
        {
            if (String.IsNullOrWhiteSpace(RemoteUrl))
                throw new Exception("RemoteUrl is not set.");
            if (String.IsNullOrWhiteSpace(BranchName))
                throw new Exception("Branch is not set.");            
            if (String.IsNullOrWhiteSpace(AuthorName))
                throw new Exception("AuthorName is null or whitespace.");
            if (String.IsNullOrWhiteSpace(AuthorEmail))
                throw new Exception("AuthorEmail is null or whitespace.");

            string workPath = ParseLocalUrl(RemoteUrl) ?? WorkingPath;
            if (!Directory.Exists(WorkingPath))
                throw new Exception("WorkingPath doesn't exist: " + WorkingPath);
        }

        /// <summary>
        /// Get repository.
        /// </summary>
        /// <param name="latest">If true, ensure the repository is up to date.</param>
        public Repository Init(bool latest)
        {
            return Init(latest, false);
        }

        /// <summary>
        /// Get repository.
        /// </summary>
        /// <param name="latest">If true, ensure the repository is up to date.</param>
        /// <param name="reset">If true, the repository is cloned from scratch.</param>
        public Repository Init(bool latest, bool reset)
        {
            Validate();

            // special local path logic
            string workPath = ParseLocalUrl(RemoteUrl);
            if (workPath != null)
            {
                Repository local = null;
                if (FileUtility.IsFolderEmpty(workPath))
                {
                    local = new Repository(Repository.Init(workPath));
                }
                else
                {
                    local = new Repository(workPath);
                }
                
                // get latest from remote
                if (latest && local.Network.Remotes["origin"] != null)
                {
                    FetchOptions localFetchOptions = new FetchOptions();
                    localFetchOptions.CredentialsProvider += ProvideCredentials;

                    PullOptions pullOptions = new PullOptions();
                    pullOptions.FetchOptions = localFetchOptions;
                    local.Network.Pull(new Signature(AuthorName, AuthorEmail, DateTime.Now), pullOptions);
                }

                // get branch
                Branch branch = local.Branches[BranchName];
                if (branch == null)
                {
                    Branch remoteBranch = local.Branches[String.Format("origin/{0}", BranchName)];
                    if (remoteBranch != null)
                    {
                        branch = local.CreateBranch(BranchName, remoteBranch.Tip);
                        branch = local.Branches.Update(
                            branch,
                            b => b.TrackedBranch = remoteBranch.CanonicalName);
                    }
                }

                if (branch != null)
                    local.Checkout(branch);

                return local;
            }

            // don't get latest or clone from remote
            if (!latest)
            {
                if (FileUtility.IsFolderEmpty(WorkingPath))
                    throw new Exception("Repository doesn't exist.");

                return new Repository(WorkingPath);
            }

            Repository repo = null;

            // delete existing repository so we will start from scratch
            if (reset)
                FileUtility.DeleteFolderContents(WorkingPath);

            // clean out existing repository if it doesn't match current settings
            if (!FileUtility.IsFolderEmpty(WorkingPath))
            {
                repo = new Repository(WorkingPath);
                Remote remote = repo.Network.Remotes["origin"];
                if (remote == null || remote.Url != RemoteUrl)
                {
                    if (repo != null)
                        repo.Dispose();

                    repo = null;
                    FileUtility.DeleteFolderContents(WorkingPath);
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

                PullOptions pullOptions = new PullOptions();
                pullOptions.FetchOptions = fetchOptions;
                repo.Network.Pull(new Signature(AuthorName, AuthorEmail, DateTime.Now), pullOptions);

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

                // if the local branch is ahead of the remote branch then we need to start from scratch
                if (localBranch.TrackingDetails.AheadBy.HasValue &&
                    localBranch.TrackingDetails.AheadBy.Value > 0)
                {
                    if (!reset)
                    {
                        repo.Dispose();
                        return Init(latest, true);
                    }

                    throw new Exception("It appears the local repository is out of sync with the remote repository.");
                }

                Branch = localBranch;

                // checkout branch and wipe out any uncommitted changes
                repo.Reset(ResetMode.Hard);
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
        public void PushPackage(byte[] package, string comment)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            Validate();

            using (Repository repo = Init(true))
            {
                PushOptions pushOptions = null;

                try
                {
                    string workPath = ParseLocalUrl(RemoteUrl);
                    bool isLocal = true;
                    if (workPath == null)
                    {
                        workPath = WorkingPath;
                        isLocal = false;
                    }

                    // extract package to the working directory
                    string path = (!String.IsNullOrWhiteSpace(SubFolder)) ?
                        Path.Combine(workPath, SubFolder) :
                        workPath;

                    string[] files = Package.Extract(package, path, true, false);

                    // stage, commit, and then push (if not local)
                    repo.Stage(files);

                    Signature author = new Signature(AuthorName, AuthorEmail, DateTime.Now);
                    repo.Commit(comment, author, author);

                    if (!isLocal)
                    {
                        pushOptions = new PushOptions();
                        pushOptions.CredentialsProvider += ProvideCredentials;
                        repo.Network.Push(Branch, pushOptions);
                    }
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

            return GetHistory(file.FileName);
        }

        /// <summary>
        /// Get the commits that involve changes to the given file.
        /// </summary>
        /// <param name="file">The file to get revisions for.</param>
        /// <returns>The revisions that occured for the given file.</returns>
        public SimpleRepositoryCommit[] GetHistory(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is null or whitespace.", "fileName");

            if (!String.IsNullOrWhiteSpace(SubFolder))
                fileName = Path.Combine(SubFolder, fileName);

            Validate();

            List<SimpleRepositoryCommit> result = new List<SimpleRepositoryCommit>();
            const int MAX_RESULTS = 50;
            const int MAX_COMMITS = 1000;

            using (App.Wait("Getting history."))
            {
                using (Repository repo = Init(true))
                {
                    Commit currentCommit = repo.Head.Tip;
                    string currentSha = null;
                    TreeEntry currentEntry = null;

                    Commit lastCommit = null;
                    string lastSha = null;
                    TreeEntry lastEntry = null;

                    int count = 0;
                    while (currentCommit != null && count <= MAX_COMMITS && result.Count < MAX_RESULTS)
                    {
                        count++;

                        // get values
                        currentEntry = currentCommit[fileName];
                        currentSha = (currentEntry != null) ? currentEntry.Target.Sha : null;

                        if (lastCommit != null)
                        {
                            lastEntry = lastCommit[fileName];
                            lastSha = (lastEntry != null) ? lastEntry.Target.Sha : null;
                        }
                        else
                        {
                            lastEntry = null;
                            lastSha = null;
                        }

                        // add commit if it's different
                        if (lastEntry != null && lastSha != currentSha)
                            result.Add(new SimpleRepositoryCommit(lastCommit));
                            
                        // move to next
                        if (currentCommit.Parents == null || currentCommit.Parents.Count() != 1)
                            break;

                        lastCommit = currentCommit;
                        currentCommit = currentCommit.Parents.ElementAt(0);
                    }

                    // check for final entry
                    currentEntry = currentCommit[fileName];
                    currentSha = (currentEntry != null) ? currentEntry.Target.Sha : null;

                    if (lastCommit != null)
                    {
                        lastEntry = lastCommit[fileName];
                        lastSha = (lastEntry != null) ? lastEntry.Target.Sha : null;
                    }
                    else
                    {
                        lastEntry = null;
                        lastSha = null;
                    }

                    if (currentEntry != null && (lastSha != currentSha || result.Count == 0))
                        result.Add(new SimpleRepositoryCommit(currentCommit));
                }
            }

            return result.OrderByDescending(c => c.Date).ToArray();
        }

        /// <summary>
        /// Get the most recent number of commits.
        /// </summary>
        /// <param name="max">The max number of commits to return.</param>
        /// <returns>The recent commits.</returns>
        public SimpleRepositoryCommit[] GetHistory(int max)
        {
            if (max <= 0)
                throw new ArgumentException("max must be greater than zero.", "max");

            List<SimpleRepositoryCommit> result = new List<SimpleRepositoryCommit>();

            using (App.Wait("Getting history."))
            {
                using (Repository repo = Init(true))
                {
                    foreach (Commit commit in repo.Commits)
                    {
                        result.Add(new SimpleRepositoryCommit(commit));
                        if (result.Count >= max)
                            break;
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Compare a file in the given commit with the previous version of the file.
        /// </summary>
        /// <param name="fileName">The file name to get the diff for.</param>
        /// <param name="commit">The commit to compare with.</param>
        /// <returns>The diff for the file.</returns>
        public string GetChangedFileDiff(string fileName, SimpleRepositoryCommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is null or whitespace.", "fileName");

            if (!String.IsNullOrWhiteSpace(SubFolder))
                fileName = Path.Combine(SubFolder, fileName);

            string newerText = String.Empty;
            string olderText = String.Empty;

            Validate();

            using (Repository repo = Init(false))
            {
                Commit c = repo.Lookup(commit.Sha) as Commit;
                if (c != null)
                {
                    TreeEntry newerEntry = c[fileName];
                    if (newerEntry != null && newerEntry.Target is Blob)
                        newerText = (newerEntry.Target as Blob).GetContentText();

                    if (c.Parents != null)
                    {
                        Commit p = c.Parents.FirstOrDefault();
                        if (p != null)
                        {
                            TreeEntry olderEntry = p[fileName];
                            if (olderEntry != null && olderEntry.Target is Blob)
                                olderText = (olderEntry.Target as Blob).GetContentText();
                        }
                    }
                }
            }

            return DiffUtility.Diff(olderText, newerText);
        }

        /// <summary>
        /// Get a list of the files that were changed in the commit.
        /// </summary>
        /// <param name="commit">The commit to get changed files for.</param>
        /// <returns>The changed files.</returns>
        public string[] GetChangedFiles(SimpleRepositoryCommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");

            List<string> result = new List<string>();

            Validate();

            using (Repository repo = Init(false))
            {
                Commit c = repo.Lookup(commit.Sha) as Commit;
                if (c == null)
                    return result.ToArray();

                Commit p = null;
                if (c.Parents != null && c.Parents.Count() == 1)
                    p = c.Parents.ElementAt(0);

                // initial commit
                if (p == null)
                {
                    Stack<TreeEntry> folderStack = new Stack<TreeEntry>();
                    foreach (TreeEntry te in c.Tree)
                    {
                        if (te.Mode == Mode.Directory)
                            folderStack.Push(te);
                        else
                            result.Add(String.Format("+ {0}", te.Path));
                    }

                    while (folderStack.Count > 0)
                    {
                        TreeEntry te = folderStack.Pop();
                        Tree tree = te.Target as Tree;
                        if (tree != null)
                        {
                            foreach (TreeEntry child in tree)
                            {
                                if (child.Mode == Mode.Directory)
                                    folderStack.Push(child);
                                else
                                    result.Add(String.Format("+ {0}", child.Path));
                            }
                        }
                    }
                }
                // normal compare
                else
                {
                    TreeChanges changes = repo.Diff.Compare<TreeChanges>(p.Tree, c.Tree);
                    if (changes == null)
                        return result.ToArray();

                    if (changes.Modified != null)
                        foreach (TreeEntryChanges tec in changes.Modified)
                            result.Add(String.Format("  {0}", tec.Path));

                    if (changes.Added != null)
                        foreach (TreeEntryChanges tec in changes.Added)
                            result.Add(String.Format("+ {0}", tec.Path));

                    if (changes.Deleted != null)
                        foreach (TreeEntryChanges tec in changes.Deleted)
                            result.Add(String.Format("- {0}", tec.Path));
                }
            }

            return result.OrderBy(f => f.Substring(1)).ToArray();
        }

        /// <summary>
        /// The the content for the given file and the given version.
        /// </summary>
        /// <param name="file">The file to get the content for.</param>
        /// <param name="version">The version of the file content to get.  If null the most recent commit is returned.</param>
        /// <returns>The requested content or null if not found.</returns>
        public string GetContent(SourceFile file, SimpleRepositoryCommit version)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            return GetContent(file.FileName, version);
        }

        /// <summary>
        /// The the content for the given file and the given version.
        /// </summary>
        /// <param name="fileName">The file name to get the content for.</param>
        /// <param name="version">The version of the file content to get.  If null the most recent commit is returned.</param>
        /// <returns>The requested content or null if not found.</returns>
        public string GetContent(string fileName, SimpleRepositoryCommit version)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                return null;

            if (!String.IsNullOrWhiteSpace(SubFolder))
                fileName = Path.Combine(SubFolder, fileName);

            // get the requested version of the file
            if (version == null)
            {
                SimpleRepositoryCommit[] commits = GetHistory(fileName);
                if (commits.Length > 0)
                    version = commits[0];
            }

            if (version != null)
            {
                Validate();

                using (Repository repo = Init(false))
                {
                    Commit commit = repo.Lookup(version.Sha) as Commit;

                    if (commit != null)
                    {
                        TreeEntry entry = commit[fileName];

                        if (entry != null && entry.Target is Blob)
                            return (entry.Target as Blob).GetContentText();
                    }
                }
            }

            // not found
            return null;
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

            string fileName = (!String.IsNullOrWhiteSpace(SubFolder)) ?
                Path.Combine(SubFolder, file.FileName) :
                file.FileName;

            Validate();
            
            using (Repository repo = Init(false))
            {
                // get the different versions of the file
                Commit olderCommit = repo.Lookup(older.Sha) as Commit;
                Commit newerCommit = repo.Lookup(newer.Sha) as Commit;

                if (olderCommit != null && newerCommit != null)
                {
                    TreeEntry olderEntry = olderCommit[fileName];
                    TreeEntry newerEntry = newerCommit[fileName];

                    if (olderEntry != null && olderEntry.Target is Blob && 
                        newerEntry != null && newerEntry.Target is Blob)
                    {
                        string olderText = (olderEntry.Target as Blob).GetContentText();
                        string newerText = (newerEntry.Target as Blob).GetContentText();

                        // do diff
                        return DiffUtility.Diff(olderText, newerText);
                    }
                }
            }

            return String.Empty;
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
            SubFolder = reader["subFolder"];
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
            writer.WriteAttributeString("subFolder", SubFolder);
            writer.WriteAttributeString("username", Username);
            writer.WriteAttributeString("password", Password);
        }

        #endregion
    }
}
