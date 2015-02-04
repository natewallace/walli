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
            if (String.IsNullOrWhiteSpace(AuthorName))
                throw new Exception("AuthorName is null or whitespace.");
            if (String.IsNullOrWhiteSpace(AuthorEmail))
                throw new Exception("AuthorEmail is null or whitespace.");
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
        public Repository Init(bool latest, bool reset)
        {
            Validate();

            // don't get latest our clone from remote
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
                    // extract package to the working directory
                    string[] files = Package.Extract(package, WorkingPath, true, false);

                    // stage, commit, and then push
                    repo.Stage(files);

                    Signature author = new Signature(AuthorName, AuthorEmail, DateTime.Now);
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
                        currentEntry = currentCommit[file.FileName];
                        currentSha = (currentEntry != null) ? currentEntry.Target.Sha : null;

                        if (lastCommit != null)
                        {
                            lastEntry = lastCommit[file.FileName];
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
                    currentEntry = currentCommit[file.FileName];
                    currentSha = (currentEntry != null) ? currentEntry.Target.Sha : null;

                    if (lastCommit != null)
                    {
                        lastEntry = lastCommit[file.FileName];
                        lastSha = (lastEntry != null) ? lastEntry.Target.Sha : null;
                    }
                    else
                    {
                        lastEntry = null;
                        lastSha = null;
                    }

                    if (currentEntry != null && lastSha != currentSha)
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
        /// Get a list of the files that were changed in the commit.
        /// </summary>
        /// <param name="commit">The commit to get changed files for.</param>
        /// <returns>The changed files.</returns>
        public string[] GetChangedFiles(SimpleRepositoryCommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");

            List<string> result = new List<string>();

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

            using (Repository repo = Init(false))
            {
                Commit c = repo.Lookup(commit.Sha) as Commit;
                if (c != null)
                {
                    TreeEntry entry = c[file.FileName];
                    if (entry != null && entry.Target is Blob)
                    {
                        return (entry.Target as Blob).GetContentText();
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Get the difference between the two versions of text.
        /// </summary>
        /// <param name="olderText">The older version of the text in the comparison.</param>
        /// <param name="newerText">The newer version of the text in the comparison.</param>
        /// <returns>A patch file that describes the differences.</returns>
        public static string Diff(string olderText, string newerText)
        {
            StringBuilder result = new StringBuilder();

            diff_match_patch dmp = new diff_match_patch();
            List<Diff> diffs = dmp.diff_main_line(olderText, newerText);

            foreach (Diff d in diffs)
            {
                // get prefix
                string prefix = null;
                switch (d.operation)
                {
                    case Operation.EQUAL:
                        prefix = "    ";
                        break;

                    case Operation.DELETE:
                        prefix = "-   ";
                        break;

                    default:
                        prefix = "+   ";
                        break;
                }

                // format the lines
                using (StringReader reader = new StringReader(d.text))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                        result.AppendFormat("{0}{1}{2}", prefix, line, Environment.NewLine);
                }
            }

            return result.ToString();
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
            
            using (Repository repo = Init(false))
            {
                // get the different versions of the file
                Commit olderCommit = repo.Lookup(older.Sha) as Commit;
                Commit newerCommit = repo.Lookup(newer.Sha) as Commit;

                if (olderCommit != null && newerCommit != null)
                {
                    TreeEntry olderEntry = olderCommit[file.FileName];
                    TreeEntry newerEntry = newerCommit[file.FileName];

                    if (olderEntry != null && olderEntry.Target is Blob && 
                        newerEntry != null && newerEntry.Target is Blob)
                    {
                        string olderText = (olderEntry.Target as Blob).GetContentText();
                        string newerText = (newerEntry.Target as Blob).GetContentText();

                        // do diff
                        return Diff(olderText, newerText);
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
