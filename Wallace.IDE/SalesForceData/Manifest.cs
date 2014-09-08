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
using System.Text;
using System.Xml;

namespace SalesForceData
{
    /// <summary>
    /// A package manifest.
    /// </summary>
    public class Manifest : IComparable
    {
        #region Fields

        /// <summary>
        /// Supports the Groups property.
        /// </summary>
        private List<ManifestItemGroup> _groups;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        public Manifest(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is null or whitespace.", "fileName");

            FileName = fileName;
            Name = Path.GetFileNameWithoutExtension(FileName);

            _groups = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The filename for this manifest.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The name of the manifest.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The groups that belong to this manifest.
        /// </summary>
        public IReadOnlyCollection<ManifestItemGroup> Groups
        {
            get
            {
                Load();
                return _groups;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete this manifest.
        /// </summary>
        public void Delete()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }

        /// <summary>
        /// Load the data for this manifest.  Only the first call of this method will load data.
        /// </summary>
        public void Load()
        {
            if (_groups == null)
            {
                _groups = new List<ManifestItemGroup>();
                if (File.Exists(FileName))
                {
                    using (Stream input = File.OpenRead(FileName))
                        Load(input);
                }
            }
        }

        /// <summary>
        /// Load the manifest from file.
        /// </summary>
        /// <param name="input">The stream to read the input from.</param>
        /// <returns>The loaded manifest.</returns>
        private void Load(System.IO.Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            XmlDocument xml = new XmlDocument();
            xml.Load(input);

            XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);
            namespaces.AddNamespace("ns", "http://soap.sforce.com/2006/04/metadata");
            XmlNodeList nodes = xml.SelectNodes("/ns:Package/ns:types", namespaces);
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    XmlNode groupName = node.SelectSingleNode("ns:name", namespaces);
                    if (groupName == null)
                        continue;

                    ManifestItemGroup group = new ManifestItemGroup(groupName.InnerText);
                    AddGroup(group);

                    XmlNodeList members = node.SelectNodes("ns:members", namespaces);
                    if (members != null)
                        foreach (XmlNode member in members)
                            group.AddItem(new ManifestItem(member.InnerText));
                }
            }
        }

        /// <summary>
        /// Save the manifest to file.
        /// </summary>
        public void Save()
        {
            Load();
            using (Stream output = File.Open(FileName, FileMode.Create))
                Save(output);
        }

        /// <summary>
        /// Save the manifest to file.
        /// </summary>
        /// <param name="output">The stream to save to.</param>
        public void Save(System.IO.Stream output)
        {
            using (XmlWriter xml = XmlWriter.Create(output, new XmlWriterSettings() { 
                Indent = true, 
                IndentChars = "\t",
                Encoding = Encoding.UTF8 }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("Package", "http://soap.sforce.com/2006/04/metadata");

                // groups
                foreach (ManifestItemGroup group in Groups)
                {
                    xml.WriteStartElement("types");

                    // items in group
                    foreach (ManifestItem item in group.Items)
                    {
                        xml.WriteStartElement("members");
                        xml.WriteString(item.Name);
                        xml.WriteEndElement();
                    }

                    // group name
                    xml.WriteStartElement("name");
                    xml.WriteString(group.Name);
                    xml.WriteEndElement();

                    xml.WriteEndElement();
                }

                // version
                xml.WriteStartElement("version");
                xml.WriteString("31.0");
                xml.WriteEndElement();

                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        /// <summary>
        /// Add the given group to this manifest if it isn't already a member.
        /// </summary>
        /// <param name="group">The group to add.</param>
        /// <returns>true if the group was added, false if it wasn't.</returns>
        public bool AddGroup(ManifestItemGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            Load();

            foreach (ManifestItemGroup g in Groups)
                if (String.Compare(g.Name, group.Name, true) == 0)
                    return false;

            group.Manifest = this;
            _groups.Add(group);
            return true;
        }

        /// <summary>
        /// Removes the given group from this manifest if it exists.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        /// <returns>true if the group was removed, false if it wasn't.</returns>
        public bool RemoveGroup(ManifestItemGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            Load();

            for (int i = 0; i < _groups.Count; i++)
            {
                if (String.Compare(_groups[i].Name, group.Name, true) == 0)
                {
                    _groups[i].Manifest = null;
                    _groups.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clear items from this manifest.
        /// </summary>
        public void Clear()
        {
            Load();
            _groups.Clear();
        }

        /// <summary>
        /// Add an item that corresponds to the given source file.
        /// </summary>
        /// <param name="item">The source file to create an item for.</param>
        public void AddItem(SourceFile item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Load();

            ManifestItemGroup group = null;
            foreach (ManifestItemGroup g in Groups)
            {
                if (g.Name == item.FileType.Name)
                {
                    group = g;
                    break;
                }
            }

            if (group == null)
            {
                group = new ManifestItemGroup(item.FileType.Name, this);
                _groups.Add(group);
            }

            group.AddItem(new ManifestItem(item.Name, group));
        }

        /// <summary>
        /// Compare this object with another object.
        /// </summary>
        /// <param name="obj">The other object to compare with this object.</param>
        /// <returns>true if the other object is logically equivalent to this object.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Get a hash code for this object.
        /// </summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            if (FileName == null)
                return 0;
            else
                return FileName.ToLower().GetHashCode();
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare this object with the given object.
        /// </summary>
        /// <param name="obj">The other object to compare with this object.</param>
        /// <returns>
        /// A negative Integer if this object precedes the other object.
        /// A positive Integer if this object follows the other object.
        /// Zero if this object is equal to the other object.
        /// </returns>
        public int CompareTo(object obj)
        {
            Manifest other = obj as Manifest;
            if (other == null)
                return -1;

            return String.Compare(this.FileName, other.FileName, true);
        }

        #endregion
    }
}
