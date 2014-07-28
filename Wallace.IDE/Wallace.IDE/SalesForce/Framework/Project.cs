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
using System.IO;
using System.Xml;
using SalesForceData;

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

        #endregion

        #region Methods

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
            if (Client != null)
                Client.Dispose();
        }

        #endregion
    }
}
