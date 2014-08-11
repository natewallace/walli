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
using System.Xml;
using System.Xml.Serialization;

namespace SalesForceData
{
    /// <summary>
    /// The type for a source file.
    /// </summary>
    public class SourceFileType : IComparable, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Don't use.  Required by xml serializer.
        /// </summary>
        public SourceFileType()
        {
            Children = new SourceFileType[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="metadataObject">The object to build this file type from.</param>
        internal SourceFileType(SalesForceAPI.Metadata.DescribeMetadataObject metadataObject)
        {
            if (metadataObject == null)
                throw new ArgumentNullException("metadataObject");
            if (String.IsNullOrWhiteSpace(metadataObject.xmlName))
                throw new ArgumentException("metadataObject.xmlName is null or whitespace.", "metadataObject.xmlName");
            
            Name = metadataObject.xmlName;

            List<SourceFileType> children = new List<SourceFileType>();
            if (metadataObject.childXmlNames != null)
                foreach (string objectChildName in metadataObject.childXmlNames)
                    if (!String.IsNullOrWhiteSpace(objectChildName))
                        children.Add(new SourceFileType(objectChildName, this));
            Children = children.ToArray();

            InFolder = metadataObject.inFolder;
            MetaFile = metadataObject.metaFile;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="parent">The parent for this file type.  Can be null if there is no parent.</param>
        internal SourceFileType(string name, SourceFileType parent)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or whitespace.", "name");

            Name = name;
            Parent = parent;
            Children = new SourceFileType[0];
            InFolder = false;
            MetaFile = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates if this file type is in a folder or note.
        /// </summary>
        public bool InFolder { get; private set; }

        /// <summary>
        /// Indicates if files of this type will have an accompanying metadata file.
        /// </summary>
        public bool MetaFile { get; private set; }

        /// <summary>
        /// The full name which include the parent name as well as this objects name.
        /// </summary>
        public string FullName
        {
            get
            {
                if (Parent == null)
                    return Name;
                else
                    return String.Format("{0}.{1}", Parent.FullName, Name);
            }
        }

        /// <summary>
        /// The name of the file type to use in a package.
        /// </summary>
        public string PackageMemberName
        {
            get
            {
                if (InFolder && Name != "EmailTemplate")
                {
                    return String.Format("{0}Folder", Name);
                }
                else
                {
                    return Name;
                }
            }
        }

        /// <summary>
        /// The children types for this type if there are any.  This will be an empty array if there
        /// are no children.
        /// </summary>
        public SourceFileType[] Children { get; private set; }

        /// <summary>
        /// The parent of this type or null if there is no parent. 
        /// </summary>
        public SourceFileType Parent { get; private set; }

        /// <summary>
        /// Returns true if this type is a child of another type.
        /// </summary>
        public bool IsChild
        {
            get { return (Parent != null); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the name of this type.
        /// </summary>
        /// <returns>The Name property.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Compare two source file type objects.
        /// The comparison is done by evaulating the name.  null values will always follow source file types.
        /// </summary>
        /// <param name="a">The source file type to compare with b.</param>
        /// <param name="b">The source file type to compare with a.</param>
        /// <returns>
        /// A negative Integer if a precedes b.
        /// A positive Integer if a follows b.
        /// Zero if a is equal to b object.
        /// </returns>
        public static int Compare(SourceFileType a, SourceFileType b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return 1;
            if (b == null)
                return -1;

            return a.CompareTo(b);
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
            SourceFileType other = obj as SourceFileType;
            if (other == null)
                return -1;

            return String.Compare(this.Name, other.Name);
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
        /// Read in this object from xml.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            Name = reader["name"];
            InFolder = Convert.ToBoolean(reader["inFolder"]);
            MetaFile = Convert.ToBoolean(reader["metaFile"]);

            reader.Read();
        }

        /// <summary>
        /// Write this object out to xml.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("inFolder", InFolder.ToString());
            writer.WriteAttributeString("metaFile", MetaFile.ToString());
        }

        #endregion
    }
}
