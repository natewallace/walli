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
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SalesForceData
{
    /// <summary>
    /// The meta system.
    /// </summary>
    public class MetaSystem
    {
        #region Fields

        /// <summary>
        /// The client for the checkout table.
        /// </summary>
        private SalesForceClient _client;

        /// <summary>
        /// The amount of time to wait on a save operation before throwing an exception.
        /// </summary>
        private static readonly TimeSpan SAVE_TIMEOUT = new TimeSpan(0, 1, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The client to use.</param>
        internal MetaSystem(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _client = client;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deploy a package.
        /// </summary>
        /// <param name="package">The package to deploy.</param>
        /// <param name="checkOnly">When true only a deployment check is performed.</param>
        /// <param name="runAllTests">When true all tests are run.</param>
        /// <returns>The id of the deployment that was started.</returns>
        public string DeployPackage(Package package, bool checkOnly, bool runAllTests)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            return DeployPackage(package.ToByteArray(), checkOnly, runAllTests);
        }

        /// <summary>
        /// Deploy a package.
        /// </summary>
        /// <param name="package">The package to deploy.</param>
        /// <param name="checkOnly">When true only a deployment check is performed.</param>
        /// <param name="runAllTests">When true all tests are run.</param>
        /// <returns>The id of the deployment that was started.</returns>
        public string DeployPackage(byte[] package, bool checkOnly, bool runAllTests)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            SalesForceAPI.Metadata.deployRequest deployRequest = new SalesForceAPI.Metadata.deployRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                null,
                package,
                new SalesForceAPI.Metadata.DeployOptions()
                {
                    checkOnly = checkOnly,
                    runAllTests = runAllTests,
                    singlePackage = true,
                    rollbackOnError = true
                });

            return _client.MetadataClient.deploy(deployRequest).result.id;
        }

        /// <summary>
        /// Check the status of a package deployment.
        /// </summary>
        /// <param name="id">The id of the package deployment that was returned in a call to DeployPackage.</param>
        /// <returns>The current package deployment status.</returns>
        public PackageDeployResult CheckPackageDeploy(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null or whitespace.", "id");

            SalesForceAPI.Metadata.checkDeployStatusRequest checkRequest = new SalesForceAPI.Metadata.checkDeployStatusRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                id,
                true);

            return new PackageDeployResult(_client.MetadataClient.checkDeployStatus(checkRequest).result);
        }

        /// <summary>
        /// Cancel the deployment of a package.
        /// </summary>
        /// <param name="id">The id of the deployment to cancel.</param>
        public void CancelPackageDeploy(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null or whitespace.", "id");

            _client.MetadataClient.cancelDeploy(new SalesForceAPI.Metadata.cancelDeployRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                id));
        }

        /// <summary>
        /// Delete the given file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        public void DeleteSourceFile(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            // do a checkout if checkouts are enabled and it's not currently checked out to anyone
            bool isCheckout = false;
            if (_client.Checkout.IsEnabled())
            {
                if (file.CheckedOutBy != null && !file.CheckedOutBy.Equals(_client.User))
                    throw new Exception("Unable to delete file: it is checked out by another user.");

                if (!String.IsNullOrWhiteSpace(file.Id) && file.CheckedOutBy == null)
                    _client.Checkout.CheckoutFile(file);

                isCheckout = true;
            }

            try
            {
                switch (file.FileType.Name)
                {
                    case "ApexClass":
                    case "ApexTrigger":
                    case "ApexPage":
                    case "ApexComponent":

                        // parse name for apex items
                        string itemName = null;
                        string namespacePrefix = null;

                        string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                        if (classNameParts.Length == 1)
                        {
                            itemName = classNameParts[0];
                            namespacePrefix = _client.Namespace;
                        }
                        else if (classNameParts.Length == 2)
                        {
                            itemName = classNameParts[1];
                            namespacePrefix = classNameParts[0];
                        }
                        else
                        {
                            itemName = file.Name;
                            namespacePrefix = _client.Namespace;
                        }

                        // get record
                        DataSelectResult objectQueryResult = _client.Data.Select(String.Format("SELECT id FROM {0} WHERE Name = '{1}' AND NamespacePrefix = '{2}'", file.FileType.Name, itemName, namespacePrefix));
                        string objectId = null;
                        if (objectQueryResult.Data.Rows.Count > 0)
                            objectId = objectQueryResult.Data.Rows[0]["id"] as string;
                        else
                            throw new Exception("Couldn't find id of object.");

                        // delete record
                        SalesForceAPI.Tooling.deleteResponse response = _client.ToolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new string[] { objectId }));

                        // process any error messages
                        if (response == null || response.result == null || response.result.Length != 1)
                            throw new Exception("Invalid response received.");

                        if (response.result[0].errors != null && response.result[0].errors.Length > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                                sb.AppendLine(error.message);

                            throw new Exception(sb.ToString());
                        }

                        break;

                    default:
                        throw new Exception("The given file type is not supported: " + file.FileType.Name);
                }
            }
            finally
            {
                if (isCheckout)
                    _client.Checkout.CheckinFile(file);
            }
        }        

        /// <summary>
        /// Get the content for the given source file.
        /// </summary>
        /// <param name="file">The file to get content for.</param>
        /// <returns>The requested content.</returns>
        public SourceFileContent GetSourceFileContent(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            // parse name for apex items
            string itemName = null;
            string namespacePrefix = null;

            string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            if (classNameParts.Length == 1)
            {
                itemName = classNameParts[0];
                namespacePrefix = _client.Namespace;
            }
            else if (classNameParts.Length == 2)
            {
                itemName = classNameParts[1];
                namespacePrefix = classNameParts[0];
            }
            else
            {
                itemName = file.Name;
                namespacePrefix = _client.Namespace;
            }

            switch (file.FileType.Name)
            {
                case "ApexClass":

                    DataSelectResult classResult = _client.Data.Select(String.Format(
                        "SELECT Id, Body, LastModifiedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (classResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex class named: " + file.Name);

                    string id = classResult.Data.Rows[0]["Id"] as string;
                    string classBody = classResult.Data.Rows[0]["Body"] as string;
                    string lastModifiedDate = classResult.Data.Rows[0]["LastModifiedDate"] as string;
                    bool isClassReadOnly = false;

                    // managed classes don't expose source code so lookup the symbol table
                    if (classBody == "(hidden)")
                    {
                        isClassReadOnly = true;
                        SalesForceAPI.Tooling.retrieveRequest request = new SalesForceAPI.Tooling.retrieveRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            "SymbolTable",
                            "ApexClass",
                            new string[] { id });

                        SalesForceAPI.Tooling.retrieveResponse response = _client.ToolingClient.retrieve(request);
                        if (response != null && response.result != null && response.result.Length == 1)
                        {
                            SalesForceAPI.Tooling.ApexClass apexClass = response.result[0] as SalesForceAPI.Tooling.ApexClass;
                            if (apexClass != null && apexClass.SymbolTable != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("/**\n * The following apex was generated by the application and represents the SalesForce definition of this class.\n */\n");
                                SymbolTableToText(apexClass.SymbolTable, sb, 0);
                                classBody = sb.ToString();
                            }
                        }
                    }

                    return new SourceFileContent(
                        file.FileType.Name,
                        classBody,
                        lastModifiedDate,
                        null,
                        isClassReadOnly);

                case "ApexTrigger":

                    DataSelectResult triggerResult = _client.Data.Select(String.Format(
                        "SELECT Body, LastModifiedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (triggerResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex trigger named: " + file.Name);

                    string triggerBody = triggerResult.Data.Rows[0]["Body"] as string;
                    bool isTriggerReadOnly = false;

                    // managed classes don't expose source code
                    if (triggerBody == "(hidden)")
                        isTriggerReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        triggerBody,
                        triggerResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isTriggerReadOnly);

                case "ApexPage":

                    DataSelectResult pageResult = _client.Data.Select(String.Format(
                        "SELECT Markup, LastModifiedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (pageResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex page named: " + file.Name);

                    string pageBody = pageResult.Data.Rows[0]["Markup"] as string;
                    bool isPageReadOnly = false;

                    // managed classes don't expose source code
                    if (pageBody == "(hidden)")
                        isPageReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        pageBody,
                        pageResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isPageReadOnly);

                case "ApexComponent":

                    DataSelectResult componentResult = _client.Data.Select(String.Format(
                        "SELECT Markup, LastModifiedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (componentResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex component named: " + file.Name);

                    string componentBody = componentResult.Data.Rows[0]["Markup"] as string;
                    bool isComponentReadOnly = false;

                    // managed classes don't expose source code
                    if (componentBody == "(hidden)")
                        isComponentReadOnly = true;

                    return new SourceFileContent(
                        file.FileType.Name,
                        componentBody,
                        componentResult.Data.Rows[0]["LastModifiedDate"] as string,
                        null,
                        isComponentReadOnly);

                case "Profile":
                    SalesForceAPI.Metadata.readMetadataRequest profileRequest = new SalesForceAPI.Metadata.readMetadataRequest(
                        new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                        null,
                        file.FileType.Name,
                        new string[] { file.Name });

                    SalesForceAPI.Metadata.readMetadataResponse profileResponse = _client.MetadataClient.readMetadata(profileRequest);
                    if (profileResponse == null || profileResponse.result.Length != 1)
                        throw new Exception("Couldn't get profile: " + file.Name);

                    if (profileResponse.result[0] is SalesForceAPI.Metadata.Profile)
                    {
                        using (StringWriter profileWriter = new StringWriter())
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(SalesForceAPI.Metadata.Profile));
                            ser.Serialize(profileWriter, profileResponse.result[0]);
                            return new SourceFileContent(
                                file.FileType.Name,
                                profileWriter.ToString(),
                                GetSourceFileContentLastModifiedTimeStamp(file),
                                String.Empty,
                                false);
                        }
                    }
                    else
                    {
                        throw new Exception("Returned object is not a Profile.");
                    }

                default:

                    byte[] zipFile = GetSourceFileContentAsPackage(new SourceFile[] { file });
                    using (MemoryStream ms = new MemoryStream(zipFile))
                    {
                        using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
                        {
                            string text = null;
                            ZipArchiveEntry zipEntry = zip.GetEntry(file.FileName);
                            if (zipEntry == null)
                                throw new Exception("Couldn't find file named " + file.FileName + " in the downloaded zip archive.");
                            using (StreamReader reader = new StreamReader(zipEntry.Open()))
                                text = reader.ReadToEnd();

                            string metadataText = null;
                            ZipArchiveEntry metadataZipEntry = zip.GetEntry(file.MetadataFileName);
                            if (metadataZipEntry != null)
                            {
                                using (StreamReader metadataReader = new StreamReader(metadataZipEntry.Open()))
                                    metadataText = metadataReader.ReadToEnd();
                            }

                            return new SourceFileContent(
                                file.FileType.Name,
                                text,
                                GetSourceFileContentLastModifiedTimeStamp(file),
                                metadataText,
                                false);
                        }
                    }
            }
        }

        /// <summary>
        /// Convert the given symbol table into apex text.
        /// </summary>
        /// <param name="symbolTable">The symbol table to convert.</param>
        /// <param name="sb">The output for the apex text.</param>
        /// <param name="indentCount">The indent count to start text at.</param>
        private void SymbolTableToText(
            SalesForceAPI.Tooling.SymbolTable symbolTable,
            StringBuilder sb,
            int indentCount)
        {
            if (symbolTable == null)
                return;

            // start namespace
            if (!String.IsNullOrWhiteSpace(symbolTable.@namespace))
            {
                if (indentCount == 0)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("public class {0} {{\n\n", symbolTable.@namespace);
                    indentCount++;
                }
            }

            // start class definition
            if (symbolTable.tableDeclaration != null)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.AppendFormat("public class {0} {{\n\n", symbolTable.tableDeclaration.name);
                indentCount++;
            }

            // constructors
            if (symbolTable.constructors != null && symbolTable.constructors.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Constructors\n\n");

                foreach (SalesForceAPI.Tooling.Constructor constructor in symbolTable.constructors)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1}(",
                        constructor.visibility.ToString().ToLower(),
                        constructor.name);

                    if (constructor.parameters != null && constructor.parameters.Length > 0)
                    {
                        foreach (SalesForceAPI.Tooling.Parameter param in constructor.parameters)
                            sb.AppendFormat("{0} {1}, ", param.type, param.name);

                        sb.Length = sb.Length - 2;
                    }

                    sb.Append(");\n\n");
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // properties
            if (symbolTable.properties != null && symbolTable.properties.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Properties\n\n");

                foreach (SalesForceAPI.Tooling.VisibilitySymbol prop in symbolTable.properties)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1} {2} {{ get; set; }}\n\n",
                        prop.visibility.ToString().ToLower(),
                        prop.type,
                        prop.name);
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // methods
            if (symbolTable.methods != null && symbolTable.methods.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Methods\n\n");

                foreach (SalesForceAPI.Tooling.Method method in symbolTable.methods)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.AppendFormat("{0} {1} {2}(",
                        method.visibility.ToString().ToLower(),
                        method.returnType,
                        method.name);

                    if (method.parameters != null && method.parameters.Length > 0)
                    {
                        foreach (SalesForceAPI.Tooling.Parameter param in method.parameters)
                            sb.AppendFormat("{0} {1}, ", param.type, param.name);

                        sb.Length = sb.Length - 2;
                    }

                    sb.Append(");\n\n");
                }

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // inner classes
            if (symbolTable.innerClasses != null && symbolTable.innerClasses.Length > 0)
            {
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//region Inner Classes\n\n");

                foreach (SalesForceAPI.Tooling.SymbolTable st in symbolTable.innerClasses)
                    SymbolTableToText(st, sb, indentCount);

                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("//endregion\n\n");
            }

            // end class definition
            if (symbolTable.tableDeclaration != null)
            {
                indentCount--;
                sb.Append(String.Empty.PadRight(indentCount, '\t'));
                sb.Append("}\n\n");
            }

            // end namespace
            if (!String.IsNullOrWhiteSpace(symbolTable.@namespace))
            {
                indentCount--;

                if (indentCount == 0)
                {
                    sb.Append(String.Empty.PadRight(indentCount, '\t'));
                    sb.Append("}\n\n");
                }
            }
        }

        /// <summary>
        /// Gets the timestamp for when the source file content was last updated.
        /// </summary>
        /// <param name="file">The file to get the timestamp for.</param>
        /// <returns>The timestamp for the given file's content.</returns>
        public string GetSourceFileContentLastModifiedTimeStamp(SourceFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            // parse name for apex items
            string itemName = null;
            string namespacePrefix = null;

            string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            if (classNameParts.Length == 1)
            {
                itemName = classNameParts[0];
                namespacePrefix = _client.Namespace;
            }
            else if (classNameParts.Length == 2)
            {
                itemName = classNameParts[1];
                namespacePrefix = classNameParts[0];
            }
            else
            {
                itemName = file.Name;
                namespacePrefix = _client.Namespace;
            }

            switch (file.FileType.Name)
            {
                case "ApexClass":

                    DataSelectResult classResult = _client.Data.Select(String.Format(
                        "SELECT LastModifiedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (classResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex class named: " + file.Name);

                    return classResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexTrigger":

                    DataSelectResult triggerResult = _client.Data.Select(String.Format(
                        "SELECT LastModifiedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (triggerResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex trigger named: " + file.Name);

                    return triggerResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexPage":

                    DataSelectResult pageResult = _client.Data.Select(String.Format(
                        "SELECT LastModifiedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (pageResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex page named: " + file.Name);

                    return pageResult.Data.Rows[0]["LastModifiedDate"] as string;

                case "ApexComponent":

                    DataSelectResult componentResult = _client.Data.Select(String.Format(
                        "SELECT LastModifiedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", itemName, namespacePrefix));
                    if (componentResult.Data.Rows.Count == 0)
                        throw new Exception("Couldn't find apex component named: " + file.Name);

                    return componentResult.Data.Rows[0]["LastModifiedDate"] as string;

                default:
                    SalesForceAPI.Metadata.ListMetadataQuery query = new SalesForceAPI.Metadata.ListMetadataQuery();
                    query.type = file.FileType.Name;
                    SalesForceAPI.Metadata.FileProperties[] fileProperties = _client.MetadataClient.listMetadata(new SalesForceAPI.Metadata.listMetadataRequest(
                        new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                        null,
                        new SalesForceAPI.Metadata.ListMetadataQuery[] { query },
                        SalesForceClient.METADATA_VERSION)).result;

                    SalesForceAPI.Metadata.FileProperties fileProp = fileProperties.Where(fp => fp.fullName == file.Name).FirstOrDefault();
                    if (fileProp == null)
                        throw new Exception("Couldn't find file named: " + file.Name);

                    return fileProp.lastModifiedDate.ToString();
            }
        }

        /// <summary>
        /// Create a new class.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="header">Optional header text to include at the head of the class.</param>
        /// <returns>The source file for the newly created class.</returns>
        public SourceFile CreateClass(string name, string header)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexClass apexClass = new SalesForceAPI.Tooling.ApexClass();
            if (header != null)
                apexClass.Body = String.Format("{0}{1}public class {2} {{{1}}}", header, Environment.NewLine, name);
            else
                apexClass.Body = String.Format("public class {0} {{{1}}}", name, Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexClass }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = _client.Data.Select(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexClass WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, _client.Namespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="objectName">The name of the object to create the trigger on.</param>
        /// <param name="triggerEvents">The events for the trigger.</param>
        /// <param name="header">Optional header text to include at the head of the trigger.</param>
        /// <returns>The newly created trigger.</returns>
        public SourceFile CreateTrigger(string triggerName, string objectName, TriggerEvents triggerEvents, string header)
        {
            if (String.IsNullOrWhiteSpace(triggerName))
                throw new ArgumentException("triggerName", "triggerName is null or whitespace.");
            if (String.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("objectName", "objectName is null or whitespace.");
            if (triggerEvents == TriggerEvents.None)
                throw new ArgumentException("triggerEvents", "triggerEvents can't be None.");

            StringBuilder triggerEventsText = new StringBuilder();
            if (triggerEvents.HasFlag(TriggerEvents.BeforeInsert))
                triggerEventsText.Append("before insert, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterInsert))
                triggerEventsText.Append("after insert, ");
            if (triggerEvents.HasFlag(TriggerEvents.BeforeUpdate))
                triggerEventsText.Append("before update, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterUpdate))
                triggerEventsText.Append("after update, ");
            if (triggerEvents.HasFlag(TriggerEvents.BeforeDelete))
                triggerEventsText.Append("before delete, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterDelete))
                triggerEventsText.Append("after delete, ");
            if (triggerEvents.HasFlag(TriggerEvents.AfterUndelete))
                triggerEventsText.Append("after undelete, ");
            triggerEventsText.Length = triggerEventsText.Length - 2;

            SalesForceAPI.Tooling.ApexTrigger apexTrigger = new SalesForceAPI.Tooling.ApexTrigger();
            apexTrigger.Name = triggerName;
            apexTrigger.TableEnumOrId = objectName;

            if (header != null)
                apexTrigger.Body = String.Format("{0}{4}trigger {1} on {2}({3}) {{{4}}}",
                    header,
                    triggerName,
                    objectName,
                    triggerEventsText.ToString(),
                    Environment.NewLine);
            else
                apexTrigger.Body = String.Format("trigger {0} on {1}({2}) {{{3}}}",
                    triggerName,
                    objectName,
                    triggerEventsText.ToString(),
                    Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexTrigger }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = _client.Data.Select(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexTrigger WHERE Name = '{0}' AND NamespacePrefix = '{1}'", triggerName, _client.Namespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new component.
        /// </summary>
        /// <param name="name">The name of the component to create.</param>
        /// <param name="header">Optional header text to include at the head of the page.</param>
        /// <returns>The newly created component.</returns>
        public SourceFile CreateComponent(string name, string header)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexComponent apexComponent = new SalesForceAPI.Tooling.ApexComponent();
            apexComponent.Name = name;
            apexComponent.MasterLabel = name;

            if (header != null)
                apexComponent.Markup = String.Format("{0}{1}<apex:component>{1}</apex:component>", header, Environment.NewLine);
            else
                apexComponent.Markup = String.Format("<apex:component>{0}</apex:component>", Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexComponent }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = _client.Data.Select(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexComponent WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, _client.Namespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Create a new page.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="header">Optional header text to include at the head of the page.</param>
        /// <returns>The source file for the newly created page.</returns>
        public SourceFile CreatePage(string name, string header)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("name is null or whitespace.");

            SalesForceAPI.Tooling.ApexPage apexPage = new SalesForceAPI.Tooling.ApexPage();
            apexPage.Name = name;
            apexPage.MasterLabel = name;

            if (header != null)
                apexPage.Markup = String.Format("{0}{1}<apex:page>{1}</apex:page>", header, Environment.NewLine);
            else
                apexPage.Markup = String.Format("<apex:page>{0}</apex:page>", Environment.NewLine);

            SalesForceAPI.Tooling.createResponse response = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest()
            {
                SessionHeader = new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                sObjects = new SalesForceAPI.Tooling.sObject[] { apexPage }
            });

            if (response == null || response.result == null || response.result.Length != 1)
                throw new Exception("Invalid response received.");

            if (response.result[0].errors != null && response.result[0].errors.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SalesForceAPI.Tooling.Error error in response.result[0].errors)
                    sb.AppendLine(error.message);

                throw new Exception(sb.ToString());
            }

            DataSelectResult result = _client.Data.Select(String.Format("SELECT Id, Name, CreatedById, CreatedDate FROM ApexPage WHERE Name = '{0}' AND NamespacePrefix = '{1}'", name, _client.Namespace));
            return new SourceFile(result.Data);
        }

        /// <summary>
        /// Save the source file content.
        /// </summary>
        /// <param name="file">The file to save the content for.</param>
        /// <param name="contentValue">The content to save.</param>
        /// <returns>Any errors that occured.</returns>
        public SalesForceError[] SaveSourceFileContent(SourceFile file, string contentValue)
        {
            return SaveSourceFileContent(file, contentValue, null);
        }

        /// <summary>
        /// Save the source file content.
        /// </summary>
        /// <param name="file">The file to save the content for.</param>
        /// <param name="contentValue">The content to save.</param>
        /// <param name="metadataValue">The metadata value.</param>
        /// <returns>Any errors that occured.</returns>
        public SalesForceError[] SaveSourceFileContent(SourceFile file, string contentValue, string metadataValue)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            // do a checkout if checkouts are enabled and it's not currently checked out to anyone
            bool isTempCheckout = false;
            if (_client.Checkout.IsEnabled())
            {
                if (file.CheckedOutBy != null && !file.CheckedOutBy.Equals(_client.User))
                    throw new Exception("Unable to save file: it is checked out by another user.");

                if (!String.IsNullOrWhiteSpace(file.Id) && file.CheckedOutBy == null)
                {
                    isTempCheckout = true;
                    _client.Checkout.CheckoutFile(file);
                }
            }

            try
            {
                switch (file.FileType.Name)
                {
                    case "ApexClass":
                    case "ApexPage":
                    case "ApexTrigger":
                    case "ApexComponent":

                        // parse name for apex items
                        string itemName = null;
                        string namespacePrefix = null;

                        string[] classNameParts = file.Name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                        if (classNameParts.Length == 1)
                        {
                            itemName = classNameParts[0];
                            namespacePrefix = _client.Namespace;
                        }
                        else if (classNameParts.Length == 2)
                        {
                            itemName = classNameParts[1];
                            namespacePrefix = classNameParts[0];
                        }
                        else
                        {
                            itemName = file.Name;
                            namespacePrefix = _client.Namespace;
                        }

                        // get item record
                        DataSelectResult objectQueryResult = _client.Data.Select(String.Format("SELECT id FROM {0} WHERE Name = '{1}' AND NamespacePrefix = '{2}'", file.FileType.Name, itemName, namespacePrefix));
                        string objectId = null;
                        if (objectQueryResult.Data.Rows.Count > 0)
                            objectId = objectQueryResult.Data.Rows[0]["id"] as string;

                        // create metadata container
                        SalesForceAPI.Tooling.MetadataContainer metadataContainer = new SalesForceAPI.Tooling.MetadataContainer();
                        metadataContainer.Name = Guid.NewGuid().ToString("N");
                        SalesForceAPI.Tooling.createResponse containerResponse = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new SalesForceAPI.Tooling.sObject[] { metadataContainer }));

                        if (containerResponse == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse returned was null.", null) };
                        if (containerResponse.result == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse.result returned was null.", null) };
                        if (containerResponse.result.Length != 1)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  containerResponse.result length was an unexpected value: " + containerResponse.result.Length, null) };
                        if (!containerResponse.result[0].success)
                        {
                            List<SalesForceError> errors = new List<SalesForceError>();
                            if (containerResponse.result[0].errors != null)
                                foreach (SalesForceAPI.Tooling.Error err in containerResponse.result[0].errors)
                                    errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                            else
                                errors.Add(new SalesForceError("SYSTEM", "containerResponse.result inidcated failure but reported no error details.", null));

                            return errors.ToArray();
                        }

                        // stage object
                        SalesForceAPI.Tooling.sObject stageObject = null;

                        if (file.FileType.Name == "ApexClass")
                        {
                            SalesForceAPI.Tooling.ApexClassMember apexClass = new SalesForceAPI.Tooling.ApexClassMember();
                            apexClass.ContentEntityId = objectId;
                            apexClass.Body = contentValue ?? String.Empty;
                            apexClass.MetadataContainerId = containerResponse.result[0].id;
                            stageObject = apexClass;
                        }
                        else if (file.FileType.Name == "ApexTrigger")
                        {
                            SalesForceAPI.Tooling.ApexTriggerMember apexTrigger = new SalesForceAPI.Tooling.ApexTriggerMember();
                            apexTrigger.ContentEntityId = objectId;
                            apexTrigger.Body = contentValue ?? String.Empty;
                            apexTrigger.MetadataContainerId = containerResponse.result[0].id;
                            stageObject = apexTrigger;
                        }
                        else if (file.FileType.Name == "ApexPage")
                        {
                            SalesForceAPI.Tooling.ApexPageMember apexPage = new SalesForceAPI.Tooling.ApexPageMember();
                            apexPage.ContentEntityId = objectId;
                            apexPage.Body = contentValue ?? String.Empty;
                            apexPage.MetadataContainerId = containerResponse.result[0].id;
                            stageObject = apexPage;
                        }
                        else if (file.FileType.Name == "ApexComponent")
                        {
                            SalesForceAPI.Tooling.ApexComponentMember apexComponent = new SalesForceAPI.Tooling.ApexComponentMember();
                            apexComponent.ContentEntityId = objectId;
                            apexComponent.Body = contentValue ?? String.Empty;
                            apexComponent.MetadataContainerId = containerResponse.result[0].id;
                            stageObject = apexComponent;
                        }


                        SalesForceAPI.Tooling.createResponse stageApexResponse = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new SalesForceAPI.Tooling.sObject[] { stageObject }));

                        if (stageApexResponse == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse returned was null.", null) };
                        if (stageApexResponse.result == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse.result returned was null.", null) };
                        if (stageApexResponse.result.Length != 1)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  stageApexResponse.result length was an unexpected value: " + stageApexResponse.result.Length, null) };
                        if (!stageApexResponse.result[0].success)
                        {
                            List<SalesForceError> errors = new List<SalesForceError>();
                            if (stageApexResponse.result[0].errors != null)
                                foreach (SalesForceAPI.Tooling.Error err in stageApexResponse.result[0].errors)
                                    errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                            else
                                errors.Add(new SalesForceError("SYSTEM", "stageApexResponse.result inidcated failure but reported no error details.", null));

                            return errors.ToArray();
                        }

                        // save apex
                        SalesForceAPI.Tooling.ContainerAsyncRequest apexSaveRequest = new SalesForceAPI.Tooling.ContainerAsyncRequest();
                        apexSaveRequest.IsCheckOnly = false;
                        apexSaveRequest.MetadataContainerId = containerResponse.result[0].id;
                        SalesForceAPI.Tooling.createResponse apexSaveResponse = _client.ToolingClient.create(new SalesForceAPI.Tooling.createRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new SalesForceAPI.Tooling.sObject[] { apexSaveRequest }));

                        if (apexSaveResponse == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse returned was null.", null) };
                        if (apexSaveResponse.result == null)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse.result returned was null.", null) };
                        if (apexSaveResponse.result.Length != 1)
                            return new SalesForceError[] { new SalesForceError("SYSTEM", "Failed to save apex file.  apexSaveResponse.result length was an unexpected value: " + apexSaveResponse.result.Length, null) };
                        if (!apexSaveResponse.result[0].success)
                        {
                            List<SalesForceError> errors = new List<SalesForceError>();
                            if (apexSaveResponse.result[0].errors != null)
                                foreach (SalesForceAPI.Tooling.Error err in apexSaveResponse.result[0].errors)
                                    errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));
                            else
                                errors.Add(new SalesForceError("SYSTEM", "apexSaveResponse.result inidcated failure but reported no error details.", null));

                            return errors.ToArray();
                        }
                        string saveRequestId = apexSaveResponse.result[0].id;

                        // get result        
                        DateTime startTime = DateTime.Now;
                        apexSaveRequest.State = "Queued";
                        while (apexSaveRequest.State == "Queued")
                        {
                            SalesForceAPI.Tooling.queryResponse pollResponse = _client.ToolingClient.query(new SalesForceAPI.Tooling.queryRequest(
                                new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                                String.Format("SELECT Id, State, CompilerErrors, ErrorMsg FROM ContainerAsyncRequest where id = '{0}'", saveRequestId)));

                            if (pollResponse != null &&
                                pollResponse.result != null &&
                                pollResponse.result.records.Length == 1)
                            {
                                apexSaveRequest = pollResponse.result.records[0] as SalesForceAPI.Tooling.ContainerAsyncRequest;
                            }

                            if (apexSaveRequest.State == "Queued")
                            {
                                if (DateTime.Now - startTime > SAVE_TIMEOUT)
                                    throw new Exception("A client side timeout occured while trying to save a file to SalesForce.");

                                System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                            }
                        }

                        _client.ToolingClient.delete(new SalesForceAPI.Tooling.deleteRequest(
                            new SalesForceAPI.Tooling.SessionHeader() { sessionId = _client.SessionId },
                            new string[] { containerResponse.result[0].id }));

                        switch (apexSaveRequest.State)
                        {
                            case "Completed":
                                DataSelectResult objectNameQueryResult = _client.Data.Select(String.Format("SELECT Name FROM {0} WHERE Id = '{1}'", file.FileType.Name, objectId));
                                if (objectNameQueryResult.Data.Rows.Count > 0)
                                {
                                    string name = objectNameQueryResult.Data.Rows[0]["Name"] as string;
                                    file.UpdateName(name);
                                }

                                // update checkout with new file name
                                if (file.IsNameUpdated && _client.Checkout.IsEnabled() && !isTempCheckout)
                                    _client.Checkout.UpdateCheckout(file);

                                return new SalesForceError[0];

                            default:
                                List<SalesForceError> errors = new List<SalesForceError>();
                                errors.Add(new SalesForceError("SYSTEM", "Failed to save apex file.", null));
                                if (!String.IsNullOrWhiteSpace(apexSaveRequest.ErrorMsg))
                                    errors.Add(new SalesForceError("ERROR", apexSaveRequest.ErrorMsg, null));
                                if (!String.IsNullOrWhiteSpace(apexSaveRequest.CompilerErrors))
                                    errors.Add(new SalesForceError("COMPILE ERROR", apexSaveRequest.CompilerErrors, null));
                                return errors.ToArray();
                        }

                    // profiles
                    case "Profile":

                        // deserialize
                        SalesForceAPI.Metadata.Profile profileData = null;
                        using (StringReader reader = new StringReader(contentValue))
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(SalesForceAPI.Metadata.Profile));
                            profileData = ser.Deserialize(reader) as SalesForceAPI.Metadata.Profile;
                        }
                        if (profileData == null)
                            throw new Exception("Could not deserialize profile object.");

                        // send request
                        SalesForceAPI.Metadata.updateMetadataRequest profileRequest = new SalesForceAPI.Metadata.updateMetadataRequest(
                            new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                            null,
                            new SalesForceAPI.Metadata.Metadata[] { profileData });

                        SalesForceAPI.Metadata.updateMetadataResponse profileResponse = _client.MetadataClient.updateMetadata(profileRequest);

                        // process response
                        if (profileResponse != null && profileResponse.result != null && profileResponse.result.Length == 1)
                        {
                            if (!profileResponse.result[0].success)
                            {
                                if (profileResponse.result[0].errors != null && profileResponse.result[0].errors.Length > 0)
                                {
                                    List<SalesForceError> errors = new List<SalesForceError>();
                                    foreach (SalesForceAPI.Metadata.Error err in profileResponse.result[0].errors)
                                        errors.Add(new SalesForceError(err.statusCode.ToString(), err.message, err.fields));

                                    return errors.ToArray();
                                }
                                else
                                {
                                    return new SalesForceError[] 
                                    { 
                                        new SalesForceError(
                                            String.Empty,
                                            "An unknown exception occured when trying to update MetaData.",
                                            null) 
                                    };
                                }
                            }
                        }

                        return new SalesForceError[0];

                    // all other source files
                    default:
                        using (MemoryStream msZip = new MemoryStream())
                        {
                            using (ZipArchive zip = new ZipArchive(msZip, ZipArchiveMode.Create))
                            {
                                ZipArchiveEntry fileEntry = zip.CreateEntry(file.FileName);
                                using (StreamWriter fileWriter = new StreamWriter(fileEntry.Open()))
                                    fileWriter.Write(contentValue);

                                if (metadataValue != null)
                                {
                                    ZipArchiveEntry metadataEntry = zip.CreateEntry(file.MetadataFileName);
                                    using (StreamWriter metadataWriter = new StreamWriter(metadataEntry.Open()))
                                        metadataWriter.Write(metadataValue);
                                }

                                Manifest manifest = new Manifest("package");
                                manifest.AddGroup(new ManifestItemGroup(file.FileType.Name));
                                manifest.Groups.ElementAt(0).AddItem(new ManifestItem(file.Name));
                                ZipArchiveEntry manifestEntry = zip.CreateEntry("package.xml");
                                using (Stream manifestWriter = manifestEntry.Open())
                                    manifest.Save(manifestWriter);
                            }

                            msZip.Flush();

                            SalesForceAPI.Metadata.AsyncResult result = _client.MetadataClient.deploy(new SalesForceAPI.Metadata.deployRequest(
                                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                                null,
                                null,
                                msZip.ToArray(),
                                new SalesForceAPI.Metadata.DeployOptions()
                                {
                                    singlePackage = true,
                                    rollbackOnError = true
                                })).result;

                            int pollCount = 0;

                            SalesForceAPI.Metadata.DeployResult deployResult = new SalesForceAPI.Metadata.DeployResult();
                            deployResult.id = result.id;
                            while (!deployResult.done && String.IsNullOrWhiteSpace(deployResult.errorMessage))
                            {
                                if (pollCount > 100)
                                {
                                    return new SalesForceError[] 
                                { 
                                    new SalesForceError(
                                        null, 
                                        "Save timed out waiting for response from the server.  Note that your save might have gone through.",
                                        null)
                                };
                                }

                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.2));

                                deployResult = _client.MetadataClient.checkDeployStatus(new SalesForceAPI.Metadata.checkDeployStatusRequest(
                                    new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                                    null,
                                    result.id,
                                    true)).result;

                                pollCount++;
                            }

                            if (deployResult.success)
                                return new SalesForceError[0];

                            return new SalesForceError[] 
                            { 
                                new SalesForceError(
                                    null, 
                                    "Unable to save. " + deployResult.errorMessage,
                                    null)
                            };
                        }
                }
            }
            finally
            {
                if (isTempCheckout)
                    _client.Checkout.CheckinFile(file);
            }
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="groupedFiles">The files to download grouped by their file type.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        private byte[] GetSourceFileContentAsPackage(Dictionary<string, HashSet<string>> groupedFiles)
        {
            if (groupedFiles == null)
                throw new ArgumentNullException("groupedFiles");

            // create request
            SalesForceAPI.Metadata.RetrieveRequest request = new SalesForceAPI.Metadata.RetrieveRequest();
            request.apiVersion = SalesForceClient.METADATA_VERSION;
            request.unpackaged = new SalesForceAPI.Metadata.Package();
            request.unpackaged.version = SalesForceClient.METADATA_VERSION.ToString("N1");
            request.singlePackage = true;

            // translate files into package request
            List<SalesForceAPI.Metadata.PackageTypeMembers> types = new List<SalesForceAPI.Metadata.PackageTypeMembers>();
            foreach (KeyValuePair<string, HashSet<string>> kvp in groupedFiles)
            {
                SalesForceAPI.Metadata.PackageTypeMembers memberGroup = new SalesForceAPI.Metadata.PackageTypeMembers();
                memberGroup.name = kvp.Key;
                memberGroup.members = kvp.Value.ToArray();
                types.Add(memberGroup);
            }

            request.unpackaged.types = types.ToArray();

            // submit request
            SalesForceAPI.Metadata.retrieveResponse retrieveResponse = _client.MetadataClient.retrieve(new SalesForceAPI.Metadata.retrieveRequest1(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                request));

            if (retrieveResponse == null)
                throw new Exception("There was an error trying to retrieve metadata.  retrieveResponse was null.");
            if (retrieveResponse.result == null)
                throw new Exception("There was an error trying to retrieve metadata.  retrieveResponse.result property was null.");
            if (retrieveResponse.result.state == SalesForceAPI.Metadata.AsyncRequestState.Error)
                throw new Exception(String.Format("There was an error trying to retrieve metadata. ({0}) {1}", retrieveResponse.result.statusCode, retrieveResponse.result.message));

            SalesForceAPI.Metadata.AsyncResult statusResult = retrieveResponse.result;

            // wait for request to be fulfilled
            int loopCount = 0;
            TimeSpan sleepTime = TimeSpan.FromSeconds(1);
            while (!statusResult.done)
            {
                if (loopCount < 5)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                else if (loopCount > 5 && loopCount < 20)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                else if (loopCount > 20)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

                SalesForceAPI.Metadata.checkStatusResponse checkStatusResponse = _client.MetadataClient.checkStatus(new SalesForceAPI.Metadata.checkStatusRequest(
                    new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                    null,
                    new string[] { statusResult.id }));

                if (checkStatusResponse == null)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse was null.");
                if (checkStatusResponse.result == null)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse.result property was null.");
                if (checkStatusResponse.result.Length != 1)
                    throw new Exception("There was an error trying to check the status of a metadata retrieve.  checkStatusResponse.result didn't have exactly one result.");

                statusResult = checkStatusResponse.result[0];
                loopCount++;
            }

            // download package
            SalesForceAPI.Metadata.checkRetrieveStatusResponse checkRetrieveStatusResponse = _client.MetadataClient.checkRetrieveStatus(new SalesForceAPI.Metadata.checkRetrieveStatusRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                statusResult.id));

            if (checkRetrieveStatusResponse == null)
                throw new Exception("There was an error trying to check the retrieve status of a metadata retrieve.  checkRetrieveStatusResponse was null.");
            if (checkRetrieveStatusResponse.result == null)
                throw new Exception("There was an error trying to check the retrieve status of a metadata retrieve.  checkRetrieveStatusResponse.result property was null.");

            return checkRetrieveStatusResponse.result.zipFile;
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="files">The files to download.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        public byte[] GetSourceFileContentAsPackage(IEnumerable<SourceFile> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");
            if (files.Count() == 0)
                throw new ArgumentException("files count is zero.", "files");

            // group files by type
            Dictionary<string, HashSet<string>> memberMap = new Dictionary<string, HashSet<string>>();
            foreach (SourceFile file in files)
            {
                HashSet<string> memberSet = null;
                if (memberMap.ContainsKey(file.FileType.Name))
                {
                    memberSet = memberMap[file.FileType.Name];
                }
                else
                {
                    memberSet = new HashSet<string>();
                    memberMap.Add(file.FileType.Name, memberSet);
                }

                memberSet.Add(file.Name);

                // add children that are in a folder
                foreach (SourceFile child in file.Children)
                {
                    if (child.FileName != file.FileName)
                    {
                        HashSet<string> childMemberSet = null;
                        if (memberMap.ContainsKey(child.FileType.Name))
                        {
                            childMemberSet = memberMap[child.FileType.Name];
                        }
                        else
                        {
                            childMemberSet = new HashSet<string>();
                            memberMap.Add(child.FileType.Name, childMemberSet);
                        }

                        childMemberSet.Add(child.Name);
                    }
                }
            }

            return GetSourceFileContentAsPackage(memberMap);
        }

        /// <summary>
        /// Download the requested source files in a package.
        /// </summary>
        /// <param name="manifest">The manifest that lists the files to download.</param>
        /// <returns>The downloaded source files in a zipped package.</returns>
        public byte[] GetSourceFileContentAsPackage(Manifest manifest)
        {
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            Dictionary<string, HashSet<string>> memberMap = new Dictionary<string, HashSet<string>>();
            foreach (ManifestItemGroup group in manifest.Groups)
            {
                HashSet<string> groupSet = new HashSet<string>();
                foreach (ManifestItem item in group.Items)
                    groupSet.Add(item.Name);
                memberMap.Add(group.Name, groupSet);
            }

            return GetSourceFileContentAsPackage(memberMap);
        }

        /// <summary>
        /// Get all of the source file types.
        /// </summary>
        /// <returns>All source file types.</returns>
        public SourceFileType[] GetSourceFileTypes()
        {
            return GetSourceFileTypes(null);
        }

        /// <summary>
        /// Get the source file types.
        /// </summary>
        /// <param name="excluded">The names of types that should be excluded from the returned list.  If null then no types will be excluded.</param>
        /// <returns>All source file types.</returns>
        public SourceFileType[] GetSourceFileTypes(IEnumerable<string> excluded)
        {
            HashSet<string> excludedSet = (excluded == null) ? new HashSet<string>() : new HashSet<string>(excluded);

            List<SourceFileType> results = new List<SourceFileType>();
            if (_client.GetOrgInfo().metadataObjects != null)
            {
                foreach (SalesForceAPI.Metadata.DescribeMetadataObject metadataObject in _client.GetOrgInfo().metadataObjects)
                {
                    if (metadataObject != null && !excludedSet.Contains(metadataObject.xmlName))
                        results.Add(new SourceFileType(metadataObject));
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Get the source files for the given types.
        /// </summary>
        /// <param name="fileTypes">The types of source files to get.</param>
        /// <param name="includeChildren">If true any children will be retrieved as well.</param>
        /// <returns>The requested source files.</returns>
        public SourceFile[] GetSourceFiles(SourceFileType[] fileTypes, bool includeChildren)
        {
            return GetSourceFiles(fileTypes, includeChildren, null);
        }

        /// <summary>
        /// Get the source files for the given types.
        /// </summary>
        /// <param name="fileTypes">The types of source files to get.</param>
        /// <param name="includeChildren">If true any children will be retrieved as well.</param>
        /// <param name="stateFilter">Only return files that have one of these states.  If null then no files are filtered out.</param>
        /// <returns>The requested source files.</returns>
        public SourceFile[] GetSourceFiles(SourceFileType[] fileTypes, bool includeChildren, IEnumerable<SourceFileState> stateFilter)
        {
            HashSet<SourceFileState> stateFilterSet = (stateFilter == null) ? null : new HashSet<SourceFileState>(stateFilter);
            if (stateFilterSet != null && stateFilterSet.Count == 0)
                throw new ArgumentException("stateFilter is empty.  Set to null if you don't want a filter.", "stateFilter");

            // flatten the file types.
            Dictionary<string, SourceFileType> mapFileTypes = new Dictionary<string, SourceFileType>();
            foreach (SourceFileType fileType in fileTypes)
            {
                mapFileTypes.Add(fileType.PackageMemberName, fileType);
                if (includeChildren)
                    foreach (SourceFileType childFileType in fileType.Children)
                        mapFileTypes.Add(childFileType.PackageMemberName, childFileType);
            }

            // make calls to get file properties
            List<Task<SalesForceAPI.Metadata.FileProperties[]>> tasks = new List<Task<SalesForceAPI.Metadata.FileProperties[]>>();
            for (int i = 0; i < mapFileTypes.Values.Count; i += 3)
            {
                List<SourceFileType> buf = new List<SourceFileType>();
                for (int j = i; j < mapFileTypes.Values.Count && j < i + 3; j++)
                    buf.Add(mapFileTypes.Values.ElementAt(j));

                tasks.Add(GetFilePropertiesAsync(buf.ToArray()));
            }

            // collect and sort file properties
            Dictionary<string, List<SalesForceAPI.Metadata.FileProperties>> fileProperties = new Dictionary<string, List<SalesForceAPI.Metadata.FileProperties>>();
            foreach (Task<SalesForceAPI.Metadata.FileProperties[]> task in tasks)
            {
                task.Wait();

                foreach (SalesForceAPI.Metadata.FileProperties fp in task.Result)
                {
                    if (stateFilterSet != null && !stateFilterSet.Contains(SourceFile.GetState(fp)))
                        continue;

                    // for files that appear in folders, format the name so it is that of the folder it's in.
                    string fileName = fp.fileName;
                    int indexOfFirstSlash = fileName.IndexOf('/');
                    int indexOfLastSlash = fileName.LastIndexOf('/');
                    if (indexOfFirstSlash != indexOfLastSlash)
                        fileName = fileName.Substring(0, indexOfLastSlash);

                    // fix for difference in workflow folder names
                    if (fileName.StartsWith("workflows"))
                        fileName = String.Format("Workflow{0}", fileName.Substring(9));

                    List<SalesForceAPI.Metadata.FileProperties> list = null;
                    if (!fileProperties.ContainsKey(fileName))
                    {
                        list = new List<SalesForceAPI.Metadata.FileProperties>();
                        fileProperties.Add(fileName, list);
                    }
                    else
                    {
                        list = fileProperties[fileName];
                    }

                    list.Add(fp);
                }
            }

            // build source files from results 
            List<SourceFile> result = new List<SourceFile>();
            foreach (KeyValuePair<string, List<SalesForceAPI.Metadata.FileProperties>> kvp in fileProperties)
            {
                SalesForceAPI.Metadata.FileProperties parent = null;
                SourceFileType parentType = null;
                List<SourceFile> children = new List<SourceFile>();

                foreach (SalesForceAPI.Metadata.FileProperties fp in kvp.Value)
                {
                    bool isInFolder = false;

                    // check for types that are in folders
                    string fileTypeName = fp.type;
                    if (!mapFileTypes.ContainsKey(fileTypeName))
                    {
                        fileTypeName = String.Format("{0}Folder", fileTypeName);
                        if (!mapFileTypes.ContainsKey(fileTypeName))
                        {
                            if (!String.IsNullOrWhiteSpace(fp.type))
                                result.Add(new SourceFile(new SourceFileType(fp.type, null), fp));

                            continue;
                        }
                        else
                        {
                            isInFolder = true;
                        }
                    }

                    SourceFileType fileType = mapFileTypes[fileTypeName];
                    if (!fileType.IsChild && !isInFolder)
                    {
                        if (parent != null)
                        {
                            result.Add(new SourceFile(parentType, parent, children));
                            parent = null;
                            children = new List<SourceFile>();
                        }

                        parent = fp;
                        parentType = fileType;
                    }
                    else
                    {
                        children.Add(new SourceFile(fileType, fp));
                    }
                }

                if (parent != null)
                    result.Add(new SourceFile(parentType, parent, children));
                else
                    result.AddRange(children);
            }

            // mark files that are checked out
            if (_client.Checkout.IsEnabled())
            {
                IDictionary<string, SourceFile> checkoutTable = _client.Checkout.GetCheckouts();

                foreach (SourceFile file in result)
                {
                    if (!String.IsNullOrEmpty(file.Id) && checkoutTable.ContainsKey(file.Id))
                        file.CheckedOutBy = checkoutTable[file.Id].CheckedOutBy;

                    foreach (SourceFile childFile in file.Children)
                        if (!String.IsNullOrEmpty(childFile.Id) && checkoutTable.ContainsKey(childFile.Id))
                            childFile.CheckedOutBy = checkoutTable[childFile.Id].CheckedOutBy;
                }
            }

            SourceFile[] files = result.ToArray();
            Array.Sort(files);
            return files;
        }

        /// <summary>
        /// An async call to get file properties for the given types.
        /// </summary>
        /// <param name="sourceFileTypes">The file types to get metadata files for.  A max of 3 types is allowed.</param>
        /// <returns>The running task that is getting the files.</returns>
        private Task<SalesForceAPI.Metadata.FileProperties[]> GetFilePropertiesAsync(SourceFileType[] sourceFileTypes)
        {
            if (sourceFileTypes == null || sourceFileTypes.Length == 0)
                throw new ArgumentException("At least one sourceFileTypes must be provided.", "sourceFileTypes");
            if (sourceFileTypes.Length > 3)
                throw new ArgumentException("Only up to 3 sourceFileTypes are allowed.", "sourceFileTypes");

            return Task<SalesForceAPI.Metadata.FileProperties[]>.Factory.StartNew(() =>
            {
                return GetFileProperties(sourceFileTypes);
            });
        }

        /// <summary>
        /// A call to get file properties for the given types.
        /// </summary>
        /// <param name="sourceFileTypes">The file types to get metadata files for.  A max of 3 types is allowed.</param>
        /// <returns>The resulting files.</returns>
        private SalesForceAPI.Metadata.FileProperties[] GetFileProperties(SourceFileType[] sourceFileTypes)
        {
            if (sourceFileTypes == null || sourceFileTypes.Length == 0)
                throw new ArgumentException("At least one sourceFileTypes must be provided.", "sourceFileTypes");
            if (sourceFileTypes.Length > 3)
                throw new ArgumentException("Only up to 3 sourceFileTypes are allowed.", "sourceFileTypes");

            List<string> sourceFileTypeNames = new List<string>();
            foreach (SourceFileType fileType in sourceFileTypes)
                sourceFileTypeNames.Add(fileType.PackageMemberName);

            List<SalesForceAPI.Metadata.ListMetadataQuery> queries = new List<SalesForceAPI.Metadata.ListMetadataQuery>();
            foreach (string metadataTypeName in sourceFileTypeNames)
            {
                if (String.IsNullOrWhiteSpace(metadataTypeName))
                    throw new ArgumentException("sourceFileTypeNames contains a null or empty type name.", "sourceFileTypeNames");

                queries.Add(new SalesForceAPI.Metadata.ListMetadataQuery() { type = metadataTypeName });
            }

            SalesForceAPI.Metadata.listMetadataRequest request = new SalesForceAPI.Metadata.listMetadataRequest(
                new SalesForceAPI.Metadata.SessionHeader() { sessionId = _client.SessionId },
                null,
                queries.ToArray(),
                SalesForceClient.METADATA_VERSION);

            SalesForceAPI.Metadata.listMetadataResponse response = null;

            try
            {
                response = _client.MetadataClient.listMetadata(request);
            }
            catch (System.ServiceModel.FaultException err)
            {
                if (!err.Message.StartsWith("INVALID_TYPE")) // suppress invalid type errors.  no way to identify them in advance.
                    throw err;

                response = null;

                // if there is more than one file type then break it out into multiple calls for the file types that aren't invalid.
                if (sourceFileTypes.Length > 1)
                {
                    List<SalesForceAPI.Metadata.FileProperties> result = new List<SalesForceAPI.Metadata.FileProperties>();
                    foreach (SourceFileType sft in sourceFileTypes)
                        result.AddRange(GetFileProperties(new SourceFileType[] { sft }));

                    return result.ToArray();
                }
            }

            if (response != null && response.result != null)
            {
                // for files that are folders do further query to get their contents
                queries.Clear();
                foreach (SalesForceAPI.Metadata.FileProperties fp in response.result)
                {
                    if (fp.type != null && fp.type.EndsWith("Folder"))
                        queries.Add(new SalesForceAPI.Metadata.ListMetadataQuery()
                        {
                            folder = fp.fullName,
                            type = fp.type.Substring(0, fp.type.Length - 6)
                        });
                }

                if (queries.Count > 0)
                {
                    List<SalesForceAPI.Metadata.FileProperties> result = new List<SalesForceAPI.Metadata.FileProperties>(response.result);
                    foreach (SalesForceAPI.Metadata.ListMetadataQuery query in queries)
                    {
                        request.queries = new SalesForceAPI.Metadata.ListMetadataQuery[] { query };
                        response = _client.MetadataClient.listMetadata(request);
                        if (response != null & response.result != null)
                            result.AddRange(response.result);
                    }

                    return result.ToArray();
                }
                else
                {
                    return response.result;
                }
            }
            else
            {
                return new SalesForceAPI.Metadata.FileProperties[0];
            }
        }

        /// <summary>
        /// Look up the user name for the given ids.
        /// </summary>
        /// <param name="userIds">The ids to get names for.</param>
        private IDictionary<string, string> GetUserNames(IEnumerable<string> userIds)
        {
            if (userIds == null)
                throw new ArgumentException("userIds is null.", "userIds");

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (userIds.Count() == 0)
                return result;

            DataSelectResult usersResult = _client.Data.Select(String.Format("SELECT Id, Name FROM User WHERE Id IN ({0})", String.Join(",", userIds)));
            do
            {
                foreach (DataRow row in usersResult.Data.Rows)
                    result.Add(row["Id"] as string, row["Name"] as string);

                usersResult = (usersResult.IsMore) ? _client.Data.Select(usersResult) : null;
            }
            while (usersResult != null);

            return result;
        }

        #endregion
    }
}
