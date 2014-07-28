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
using System.Text;
using System.Xml;

namespace SalesForceData
{
    /// <summary>
    /// A manifest that is part of a package.
    /// </summary>
    public class PackageManifest
    {
        #region Fields

        /// <summary>
        /// Supports the Groups property.
        /// </summary>
        private List<PackageManifestItemGroup> _groups;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public PackageManifest()
        {
            _groups = new List<PackageManifestItemGroup>();
            Groups = _groups;
        }

        #endregion

        #region Properties


        /// <summary>
        /// The groups that belong to this package manifest.
        /// </summary>
        public IReadOnlyCollection<PackageManifestItemGroup> Groups { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Load the package from file.
        /// </summary>
        /// <param name="input">The stream to read the input from.</param>
        /// <returns>The loaded package manifest.</returns>
        public static PackageManifest Load(System.IO.Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PackageManifest manifest = new PackageManifest();
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

                    PackageManifestItemGroup group = new PackageManifestItemGroup(groupName.InnerText);
                    manifest.AddGroup(group);

                    XmlNodeList members = node.SelectNodes("ns:members", namespaces);
                    if (members != null)
                        foreach (XmlNode member in members)
                            group.AddItem(new PackageManifestItem(member.InnerText));
                }
            }

            return manifest;
        }

        /// <summary>
        /// Save the package to file.
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
                foreach (PackageManifestItemGroup group in Groups)
                {
                    xml.WriteStartElement("types");

                    // items in group
                    foreach (PackageManifestItem item in group.Items)
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
                xml.WriteString("29.0");
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
        public bool AddGroup(PackageManifestItemGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            foreach (PackageManifestItemGroup g in Groups)
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
        public bool RemoveGroup(PackageManifestItemGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

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

        #endregion
    }
}
