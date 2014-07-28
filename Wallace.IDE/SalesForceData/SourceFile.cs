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
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SalesForceData
{
    /// <summary>
    /// A single piece of source data in a salesforce instance.
    /// </summary>
    public class SourceFile : IComparable, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Don't use.  Required by xml serializer.
        /// </summary>
        public SourceFile()
        {
        }

        /// <summary>
        /// Create a source file from a data result for one of the Apex objects.
        /// </summary>
        /// <param name="data">The data to create this source file from.</param>
        public SourceFile(DataTable data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Rows.Count != 1)
                throw new ArgumentException("data doesn't have exactly one row.");

            FileType = new SourceFileType(data.TableName, null);
            Name = data.Rows[0]["Name"] as string;

            switch (FileType.Name)
            {
                case "ApexClass":
                    FileName = String.Format("classes\\{0}.cls", Name);
                    break;

                case "ApexTrigger":
                    FileName = String.Format("triggers\\{0}.trigger", Name);
                    break;

                case "ApexPage":
                    FileName = String.Format("pages\\{0}.page", Name);
                    break;

                case "ApexComponent":
                    FileName = String.Format("components\\{0}.component", Name);
                    break;

                default:
                    throw new ArgumentException("Unsupported type: " + FileType.Name);
            }

            ChangedById = data.Rows[0]["CreatedById"] as string;
            ChangedOn = DateTime.Parse(data.Rows[0]["CreatedDate"] as string);
            CreatedById = ChangedById;
            CreatedOn = ChangedOn;
            Children = new SourceFile[0];

            Parent = null;
            State = SourceFileState.None;

            IsNameUpdated = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileType">The type of file.</param>
        /// <param name="fileProperties">The file properties.</param>
        public SourceFile(SourceFileType fileType, SalesForceAPI.Metadata.FileProperties fileProperties)
        {
            if (fileType == null)
                throw new ArgumentNullException("fileType");
            if (fileProperties == null)
                throw new ArgumentNullException("fileProperties");

            FileType = fileType;
            Name = fileProperties.fullName;
            FileName = fileProperties.fileName;
            Parent = null;
            ChangedById = fileProperties.lastModifiedById ?? String.Empty;
            ChangedByName = fileProperties.lastModifiedByName;
            ChangedOn = fileProperties.lastModifiedDate;
            CreatedById = fileProperties.createdById ?? String.Empty;
            CreatedByName = fileProperties.createdByName;
            CreatedOn = fileProperties.createdDate;
            Children = new SourceFile[0];
            State = GetState(fileProperties);

            // salesforce defect workaround
            if (FileName != null && FileName.StartsWith("Workflow/"))
                FileName = FileName.Replace("Workflow/", "workflows/");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileType">The type of file.</param>
        /// <param name="fileProperties">The file properties.</param>
        /// <param name="children">The child files for this file.</param>
        internal SourceFile(SourceFileType fileType, SalesForceAPI.Metadata.FileProperties fileProperties, IEnumerable<SourceFile> children) :
            this(fileType, fileProperties)
        {
            if (children == null)
                Children = new SourceFile[0];
            else
                Children = children.ToArray();

            foreach (SourceFile child in Children)
                child.Parent = this;

            Array.Sort(Children);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The file type.</param>
        /// <param name="name">The name.</param>
        public SourceFile(string type, string name)
        {
            if (String.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is null or whitespace.", "type");
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or whitespace.", "name");

            FileType = new SourceFileType(type, null);
            Name = name;

            FileName = null;
            Parent = null;
            ChangedById = null;
            ChangedByName = null;
            ChangedOn = DateTime.MinValue;
            CreatedById = null;
            CreatedByName = null;
            CreatedOn = DateTime.MinValue;
            Children = new SourceFile[0];
            State = SourceFileState.None;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of source file this is.
        /// </summary>
        public SourceFileType FileType { get; private set; }

        /// <summary>
        /// The name of this source file.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Set to true if the name has been updated after this object has been created.
        /// </summary>
        public bool IsNameUpdated { get; private set; }

        /// <summary>
        /// The name of the physical file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The state of the source file.
        /// </summary>
        public SourceFileState State { get; private set; }

        /// <summary>
        /// The parent of this source file if there is one, null if there isn't.
        /// </summary>
        public SourceFile Parent { get; private set; }

        /// <summary>
        /// The children of this source file if there is any.
        /// </summary>
        public SourceFile[] Children { get; private set; }

        /// <summary>
        /// The id of the user that last changed this file.
        /// </summary>
        public string ChangedById { get; private set; }

        /// <summary>
        /// The name of the user that last changed this file.
        /// </summary>
        public string ChangedByName { get; private set; }

        /// <summary>
        /// The date this file was last changed.
        /// </summary>
        public DateTime ChangedOn { get; private set; }

        /// <summary>
        /// The id of the user that created this file.
        /// </summary>
        public string CreatedById { get; private set; }

        /// <summary>
        /// The name of the user that created this file.
        /// </summary>
        public string CreatedByName { get; private set; }

        /// <summary>
        /// The date the file was created.
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// The name of the metadata file.
        /// </summary>
        public string MetadataFileName
        {
            get { return String.Format("{0}-meta.xml", FileName); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the name.  This can happen when an apex class or trigger has a name change through code.
        /// </summary>
        /// <param name="name">The new name to set.</param>
        internal void UpdateName(string name)
        {
            if (Name != name)
            {
                Name = name;
                IsNameUpdated = true;
            }
        }

        /// <summary>
        /// Get the state for the given file properties.
        /// </summary>
        /// <param name="fileProperties">The file properties to get the state for.</param>
        /// <returns>The state for the given file properties.</returns>
        internal static SourceFileState GetState(SalesForceAPI.Metadata.FileProperties fileProperties)
        {
            if (!fileProperties.manageableStateSpecified)
                return SourceFileState.None;

            switch (fileProperties.manageableState)
            {
                case SalesForceAPI.Metadata.ManageableState.beta:
                    return SourceFileState.Beta;

                case SalesForceAPI.Metadata.ManageableState.deleted:
                    return SourceFileState.Deleted;

                case SalesForceAPI.Metadata.ManageableState.deprecated:
                    return SourceFileState.Deprecated;

                case SalesForceAPI.Metadata.ManageableState.installed:
                    return SourceFileState.Installed;

                case SalesForceAPI.Metadata.ManageableState.released:
                    return SourceFileState.Released;

                case SalesForceAPI.Metadata.ManageableState.unmanaged:
                    return SourceFileState.Unmanaged;

                default:
                    throw new Exception("Unknown state: " + fileProperties.manageableState.ToString());
            }
        }

        /// <summary>
        /// Check to see if the object is logically equal to this one.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>true if the given object is equal to this one.</returns>
        public override bool Equals(object obj)
        {
            SourceFile other = obj as SourceFile;
            if (other == null)
                return false;

            return (this.FileName == other.FileName && this.Name == other.Name);
        }

        /// <summary>
        /// Get a hash code for this object.
        /// </summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            return (FileName + Name).GetHashCode();
        }

        /// <summary>
        /// Compare the 2 source files.
        /// </summary>
        /// <param name="fileA">The file to compare.</param>
        /// <param name="fileB">The other file to compare.</param>
        /// <returns>
        /// A negative Integer if this object precedes the other object.
        /// A positive Integer if this object follows the other object.
        /// Zero if this object is equal to the other object.
        /// </returns>
        public static int Compare(SourceFile fileA, SourceFile fileB)
        {
            if (fileA == null && fileB == null)
                return 0;
            if (fileA == null)
                return -1;
            if (fileB == null)
                return 1;

            return fileA.CompareTo(fileB);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Not used.
        /// </summary>
        /// <returns>null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read in this object from xml.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            Name = reader["name"];
            FileName = reader["fileName"];
            ChangedById = reader["changedById"];
            ChangedByName = reader["changedByName"];
            ChangedOn = DateTime.Parse(reader["changedOn"]);
            CreatedById = reader["createdById"];
            CreatedByName = reader["createdByName"];
            CreatedOn = DateTime.Parse(reader["createdOn"]);
            State = (SourceFileState)Enum.Parse(typeof(SourceFileState), reader["state"]);            
            FileType = new SourceFileType(reader["fileType"], null);

            List<SourceFile> children = new List<SourceFile>();
            if (!reader.IsEmptyElement)
            {
                XmlReader subTree = reader.ReadSubtree();

                if (subTree.ReadToDescendant("child"))
                {                    
                    while (subTree.NodeType == XmlNodeType.Element && subTree.Name == "child")
                    {
                        SourceFile child = new SourceFile();
                        child.ReadXml(subTree);
                        children.Add(child);
                    }                    
                }
            }

            Children = children.ToArray();

            reader.Read();
        }

        /// <summary>
        /// Write out this object to xml.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("fileType", FileType.Name);
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("fileName", FileName);
            writer.WriteAttributeString("changedById", ChangedById);
            writer.WriteAttributeString("changedByName", ChangedByName);            
            writer.WriteAttributeString("changedOn", ChangedOn.ToString());
            writer.WriteAttributeString("createdById", CreatedById);
            writer.WriteAttributeString("createdByName", CreatedByName);
            writer.WriteAttributeString("createdOn", CreatedOn.ToString());
            writer.WriteAttributeString("state", State.ToString());
        
            foreach (SourceFile child in Children)
            {
                writer.WriteStartElement("child");
                child.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare the other object with this one to see if the other object should preceed or follow this object.
        /// The comparison is done by evaulating the name.  null values will always follow source file types.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>
        /// A negative Integer if this object precedes the other object.
        /// A positive Integer if this object follows the other object.
        /// Zero if this object is equal to the other object.
        /// </returns>
        public int CompareTo(object obj)
        {
            SourceFile other = obj as SourceFile;
            if (other == null)
                return -1;

            int result = this.FileType.CompareTo(other.FileType);
            if (result != 0)
                return result;

            return String.Compare(this.Name, other.Name);
        }

        #endregion
    }
}
