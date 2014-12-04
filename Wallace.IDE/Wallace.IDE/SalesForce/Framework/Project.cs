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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using SalesForceData;
using SalesForceLanguage;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.CodeModel;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// A local salesforce project.
    /// </summary>
    public class Project : IDisposable
    {
        #region Fields

        /// <summary>
        /// The folder that holds data for this application.
        /// </summary>
        private static readonly string APP_DATA_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Walli");

        /// <summary>
        /// The root folder that holds all salesforce projects.
        /// </summary>
        private static readonly string ROOT_FOLDER = Path.Combine(APP_DATA_FOLDER, "SalesForce Projects");

        /// <summary>
        /// Used for encryption of credentials.  Not real secure but it's better than storing them in plain text.
        /// </summary>
        private static readonly System.Security.SecureString PROJECT_PASSWORD = SecureStringUtility.CreateSecureString("{70B204A2-3766-4899-8F75-5CB9BFB038E6}");

        /// <summary>
        /// Supports the Credential property.
        /// </summary>
        private SalesForceCredential _credential;

        /// <summary>
        /// Signaled after symbols have been downloaded.
        /// </summary>
        private EventWaitHandle _symbolsDownloaded;

        /// <summary>
        /// This is set to true when the project symbols are being loaded.
        /// </summary>
        private volatile bool _symbolsDownloading;

        /// <summary>
        /// This is set to true when the downloading of symbols is canceled.
        /// </summary>
        private volatile bool _symbolsDownloadCancel;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Project()
        {
            if (!Directory.Exists(APP_DATA_FOLDER))
                Directory.CreateDirectory(APP_DATA_FOLDER);
            if (!Directory.Exists(ROOT_FOLDER))
                Directory.CreateDirectory(ROOT_FOLDER);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="credential">The credential for the project.</param>
        public Project(SalesForceCredential credential)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");

            Credential = credential;
            ProjectFolder = Path.Combine(ROOT_FOLDER, credential.Username);
            ProjectFile = Path.Combine(ProjectFolder, String.Format("{0}.sfdcProject", credential.Username));

            DeployFolder = Path.Combine(ProjectFolder, "Deploy");
            if (!Directory.Exists(DeployFolder))
                Directory.CreateDirectory(DeployFolder);

            ManifestFolder = Path.Combine(DeployFolder, "Manifest");
            if (!Directory.Exists(ManifestFolder))
                Directory.CreateDirectory(ManifestFolder);
            UpgradeManifestFiles();

            PackageFolder = Path.Combine(DeployFolder, "Package");
            if (!Directory.Exists(PackageFolder))
                Directory.CreateDirectory(PackageFolder);

            SymbolsFolder = Path.Combine(ProjectFolder, "Symbols");
            if (!Directory.Exists(SymbolsFolder))
                Directory.CreateDirectory(SymbolsFolder);

            Language = new LanguageManager(SymbolsFolder);

            _symbolsDownloaded = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The salesforce credential for the project.
        /// </summary>
        public SalesForceCredential Credential
        {
            get
            {
                return _credential;
            }
            set
            {
                _credential = value;

                if (Client != null)
                    Client.Dispose();

                Client = new SalesForceClient(_credential);
            }
        }

        /// <summary>
        /// The client to use with this project.
        /// </summary>
        public SalesForceClient Client { get; private set; }

        /// <summary>
        /// The language manager to use with this project.
        /// </summary>
        public LanguageManager Language { get; private set; }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName
        {
            get
            {
                return Credential.Username;
            }
        }

        /// <summary>
        /// The folder path for this project.
        /// </summary>
        public string ProjectFolder { get; private set; }

        /// <summary>
        /// The file path for this project.
        /// </summary>
        public string ProjectFile { get; private set; }

        /// <summary>
        /// The file path for the deploy folder of this project.
        /// </summary>
        public string DeployFolder { get; private set; }

        /// <summary>
        /// The file path for the manifest folder of this project.
        /// </summary>
        public string ManifestFolder { get; private set; }

        /// <summary>
        /// The file path for the package folder of this project.
        /// </summary>
        public string PackageFolder { get; private set; }

        /// <summary>
        /// The file path for the symbols folder of this project.
        /// </summary>
        public string SymbolsFolder { get; private set; }

        /// <summary>
        /// All of the project names of projects that have been saved.
        /// </summary>
        public static string[] Projects
        {
            get
            {
                string[] folders = Directory.GetDirectories(ROOT_FOLDER);
                List<string> result = new List<string>();
                foreach (string folder in folders)
                    result.Add(Path.GetFileName(folder));

                result.Sort();
                return result.ToArray();
            }
        }

        /// <summary>
        /// Returns true if symbols are currently being downloaded.
        /// </summary>
        public bool IsDownloadingSymbols
        {
            get { return _symbolsDownloading; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manifest files used to have a .manifest extension.  They should now have a .xml extension.
        /// This method will rename any files in the manifest file with the .manifest extension to have the .xml extension.
        /// </summary>
        private void UpgradeManifestFiles()
        {
            string[] files = Directory.GetFiles(ManifestFolder, "*.manifest");
            foreach (string file in files)
            {
                string path = Path.GetDirectoryName(file);
                string newName = Path.Combine(path, String.Format("{0}.xml", Path.GetFileNameWithoutExtension(file)));
                if (!File.Exists(newName))
                    File.Move(file, newName);
            }
        }

        /// <summary>
        /// Checks to see if a project with the given name already exists.
        /// </summary>
        /// <param name="projectName">The name of the project to check for.</param>
        /// <returns>true if the project exists.  false if it doesn't .</returns>
        public static bool ProjectExists(string projectName)
        {
            if (String.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("projectName is null or whitespace.", "projectName");

            foreach (string existingProjectName in Projects)
                if (String.Compare(existingProjectName, projectName, true) == 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Deletes the given project.
        /// </summary>
        /// <param name="projectName">The project to delete.</param>
        public static void DeleteProject(string projectName)
        {
            string folderName = Path.Combine(ROOT_FOLDER, projectName);
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);
        }

        /// <summary>
        /// Open the project with the given name.
        /// </summary>
        /// <param name="projectName">The name of the project to open.</param>
        /// <returns>The project with the given name.</returns>
        public static Project OpenProject(string projectName)
        {
            if (String.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("projectName is null or whitespace.", "projectName");

            string fileName = Path.Combine(ROOT_FOLDER, projectName);
            fileName = Path.Combine(fileName, String.Format("{0}.sfdcProject", projectName));
            if (!File.Exists(fileName))
                throw new Exception("Couldn't find project: " + projectName);

            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreComments = true;
            xmlSettings.IgnoreWhitespace = true;

            Project project = null;

            using (FileStream stream = File.OpenRead(fileName))
            {
                using (XmlReader xml = XmlReader.Create(stream, xmlSettings))
                {
                    if (!xml.ReadToFollowing("project"))
                        throw new Exception("Invalid xml.  No 'project' element found.");

                    if (!xml.ReadToDescendant("credential"))
                        throw new Exception("Invalid xml.  No 'credential' element found.");

                    CryptoContainer<SalesForceCredential> crypto = new CryptoContainer<SalesForceCredential>();
                    crypto.ReadXml(xml);
                    SalesForceCredential credential = crypto.GetData(PROJECT_PASSWORD);

                    xml.Close();

                    project = new Project(credential);
                }

                stream.Close();
            }

            return project;
        }

        /// <summary>
        /// Get the list of manifests for this project.
        /// </summary>
        /// <returns>The list of manifests for this project.</returns>
        public Manifest[] GetManifests()
        {
            List<Manifest> result = new List<Manifest>();
            foreach (string file in Directory.GetFiles(ManifestFolder))
                result.Add(new Manifest(file));

            return result.ToArray();
        }

        /// <summary>
        /// Get the list of packages for this project.
        /// </summary>
        /// <returns>The list of packages for this project.</returns>
        public Package[] GetPackages()
        {
            List<Package> result = new List<Package>();
            foreach (string file in Directory.GetFiles(PackageFolder))
                result.Add(new Package(file));

            return result.ToArray();
        }

        /// <summary>
        /// Calling this method will cancel the downloading of symbols.
        /// </summary>
        public void CancelLoadSymbolsAsync()
        {
            _symbolsDownloadCancel = true;
        }

        /// <summary>
        /// Reload the symbols cache.
        /// </summary>
        public void ReloadSymbolsAsync()
        {
            if (IsDownloadingSymbols)
                return;

            Language.ResetSymbols();
            LoadSymbolsAsync(true);
        }

        /// <summary>
        /// Convert an sObject type to a SymbolTable.
        /// </summary>
        /// <param name="objectType">The sObject type to convert.</param>
        /// <returns>The newly created symbol table.</returns>
        public static SymbolTable ConvertToSymbolTable(SObjectType objectType)
        {
            List<Field> fields = new List<Field>();
            foreach (SObjectFieldType sObjectField in objectType.Fields)
            {
                string fieldType = null;
                switch (sObjectField.FieldType)
                {
                    case FieldType.String:
                    case FieldType.Picklist:
                    case FieldType.MultiPicklist:
                    case FieldType.Combobox:
                    case FieldType.TextArea:
                    case FieldType.Phone:
                    case FieldType.Url:
                    case FieldType.Email:
                    case FieldType.EncryptedString:
                        fieldType = "System.String";
                        break;

                    case FieldType.Id:
                    case FieldType.Reference:
                        fieldType = "System.Id";
                        break;

                    case FieldType.Date:
                        fieldType = "System.Date";
                        break;

                    case FieldType.DateTime:
                        fieldType = "System.DateTime";
                        break;

                    case FieldType.Time:
                        fieldType = "System.Time";
                        break;

                    case FieldType.Base64:
                        fieldType = "System.Long";
                        break;

                    case FieldType.Double:
                    case FieldType.Percent:
                        fieldType = "System.Double";
                        break;

                    case FieldType.Boolean:
                        fieldType = "System.Boolean";
                        break;

                    case FieldType.Int:
                    case FieldType.Currency:
                        fieldType = "System.Integer";
                        break;

                    default:
                        break;
                }

                // add the field
                fields.Add(new Field(
                    new TextPosition(0, 0),
                    sObjectField.Name,
                    null,
                    SymbolModifier.Public,
                    fieldType,
                    false));

                // add reference field if appropriate
                if (sObjectField.FieldType == FieldType.Reference &&
                    sObjectField.ReferenceTo != null &&
                    sObjectField.ReferenceTo.Length > 0)
                {
                    if (sObjectField.Name.EndsWith("__c", StringComparison.CurrentCultureIgnoreCase))
                    {
                        fields.Add(new Field(
                            new TextPosition(0, 0),
                            sObjectField.Name.Replace("__c", "__r"),
                            null,
                            SymbolModifier.Public,
                            sObjectField.ReferenceTo[0],
                            false));
                    }
                    else if (sObjectField.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        fields.Add(new Field(
                            new TextPosition(0, 0),
                            sObjectField.Name.Substring(0, sObjectField.Name.Length - 2),
                            null,
                            SymbolModifier.Public,
                            sObjectField.ReferenceTo[0],
                            false));
                    }
                }
            }

            // return the symbol table
            return new SymbolTable(new TextPosition(0, 0),
                                   objectType.Name,
                                   null,
                                   null,
                                   SymbolModifier.Public,
                                   SymbolTableType.SObject,
                                   null,
                                   fields.ToArray(),
                                   null,
                                   null,
                                   null,
                                   "System.sObject",
                                   null,
                                   null);
        }

        /// <summary>
        /// Checks to see if symbols have already been downloaded.  If they haven't, they are loaded from the server
        /// in the background.
        /// </summary>
        /// <param name="forceReload">If true the symbols will be loaded even if there are already symbols present.</param>
        public void LoadSymbolsAsync(bool forceReload)
        {
            if (_symbolsDownloading)
                return;

            _symbolsDownloading = true;
            _symbolsDownloadCancel = false;

            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    try
                    {
                        // download apex, parse it and then save to cache
                        if (forceReload || Directory.GetFiles(SymbolsFolder).Length == 0)
                        {
                            App.Instance.Dispatcher.Invoke(() => App.SetStatus("Downloading symbols..."));

                            object[] parameters = state as object[];
                            Project project = parameters[0] as Project;
                            SalesForceClient client = parameters[1] as SalesForceClient;
                            LanguageManager language = parameters[2] as LanguageManager;

                            try
                            {
                                project._symbolsDownloaded.Reset();

                                // get class ids
                                DataSelectResult data = client.DataSelect("SELECT Id FROM ApexClass");
                                Queue<string> classIds = new Queue<string>();
                                foreach (DataRow row in data.Data.Rows)
                                    classIds.Enqueue(row["Id"] as string);

                                // download symbols in groups of 10
                                while (classIds.Count > 0)
                                {
                                    if (project._symbolsDownloadCancel)
                                        break;

                                    StringBuilder query = new StringBuilder("SELECT Id, Body FROM ApexClass WHERE Id IN (");
                                    for (int i = 0; i < 10 && classIds.Count > 0; i++)
                                        query.AppendFormat("'{0}',", classIds.Dequeue());

                                    query.Length = query.Length - 1;
                                    query.Append(")");

                                    DataSelectResult classData = client.DataSelect(query.ToString());
                                    foreach (DataRow row in classData.Data.Rows)
                                        language.ParseApex(row["Body"] as string, false, true);
                                }

                                // download symbols for SObjects
                                if (!project._symbolsDownloadCancel)
                                {
                                    SObjectTypePartial[] sObjects = client.DataDescribeGlobal();
                                    foreach (SObjectTypePartial sObject in sObjects)
                                    {
                                        if (project._symbolsDownloadCancel)
                                            break;

                                        SObjectType sObjectDetail = client.DataDescribeObjectType(sObject);
                                        
                                        language.UpdateSymbols(
                                            ConvertToSymbolTable(sObjectDetail), 
                                            false, 
                                            true);
                                    }
                                }

                                // remove symbols if the download was interupted
                                if (_symbolsDownloadCancel)
                                {
                                    Directory.Delete(project.SymbolsFolder, true);
                                }
                            }
                            finally
                            {
                                project._symbolsDownloaded.Set();
                            }
                        }
                        // load cached symbols
                        else
                        {
                            App.Instance.Dispatcher.Invoke(() => App.SetStatus("Loading symbols..."));

                            XmlSerializer ser = new XmlSerializer(typeof(SymbolTable));

                            foreach (string file in Directory.GetFiles(SymbolsFolder))
                            {
                                using (FileStream fs = File.OpenRead(file))
                                {
                                    SymbolTable st = ser.Deserialize(fs) as SymbolTable;
                                    Language.UpdateSymbols(st, false, false);
                                    fs.Close();
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        App.Instance.Dispatcher.Invoke(() => App.HandleException(err));
                    }
                    finally
                    {
                        App.Instance.Dispatcher.Invoke(() => App.SetStatus(null));
                        _symbolsDownloading = false;
                    }
                },
                new object[] { this, Client, Language });
        }

        /// <summary>
        /// Saves the project to disk.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(ROOT_FOLDER))
                Directory.CreateDirectory(ROOT_FOLDER);

            if (!Directory.Exists(ProjectFolder))
                Directory.CreateDirectory(ProjectFolder);

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using (FileStream stream = File.Open(ProjectFile, FileMode.Create))
            {
                using (XmlWriter xml = XmlWriter.Create(stream, xmlSettings))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("project");

                    xml.WriteStartElement("credential");
                    CryptoContainer<SalesForceCredential> crypto = new CryptoContainer<SalesForceCredential>(Credential, PROJECT_PASSWORD);
                    crypto.WriteXml(xml);
                    xml.WriteEndElement();

                    xml.WriteEndElement();
                    xml.WriteEndDocument();

                    xml.Close();
                }

                stream.Close();
            }
        }

        /// <summary>
        /// Returns the ProjectName property.
        /// </summary>
        /// <returns>The ProjectName property.</returns>
        public override string ToString()
        {
            return ProjectName;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of the Client property.
        /// </summary>
        public void Dispose()
        {
            CancelLoadSymbolsAsync();
            _symbolsDownloaded.WaitOne();

            if (Client != null)
                Client.Dispose();
        }

        #endregion
    }
}
