﻿/*
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
using System.IO;
using System.IO.Compression;

namespace SalesForceData
{
    /// <summary>
    /// A package that contains source files which can be deployed.
    /// </summary>
    public class Package : IComparable
    {
        #region Fields

        /// <summary>
        /// Supports Manifest property.
        /// </summary>
        private Manifest _manifest;

        /// <summary>
        /// Supports IsDestructive property.
        /// </summary>
        private bool _isDestructive;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        public Package(string fileName)
            : this(fileName, false, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        /// <param name="isDestructive">IsDestructive.</param>
        public Package(string fileName, bool isDestructive)
            : this(fileName, isDestructive, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        /// <param name="isDestructive">IsDestructive.</param>
        /// <param name="manifest">Manifest.</param>
        public Package(string fileName, bool isDestructive, Manifest manifest)
        {
            FileName = fileName;
            if (FileName != null)
                Name = System.IO.Path.GetFileNameWithoutExtension(FileName);
            else
                Name = String.Empty;

            IsDestructive = isDestructive;
            Manifest = manifest;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The file name of the package. 
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Get the name for the package.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The manifest for this package.
        /// </summary>
        public Manifest Manifest
        {
            get
            {
                Load();
                return _manifest;
            }
            private set
            {
                _manifest = value;
            }
        }

        /// <summary>
        /// If true this is a destructive package.
        /// </summary>
        public bool IsDestructive
        {
            get
            {
                Load();
                return _isDestructive;
            }
            private set
            {
                _isDestructive = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the package from file into memory.
        /// </summary>
        public void Load()
        {
            if (_manifest != null)
                return;

            if (File.Exists(FileName))
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Open))
                    Load(fs);
            }
            else
            {
                Manifest = new Manifest(null);
            }
        }

        /// <summary>
        /// Load the package from file into memory.
        /// </summary>
        /// <param name="input">The input stream to load from.</param>
        public void Load(Stream input)
        {
            if (_manifest != null)
                return;

            using (ZipArchive zip = new ZipArchive(input, ZipArchiveMode.Read))
            {
                ZipArchiveEntry zipEntry = zip.GetEntry("destructiveChanges.xml");
                if (zipEntry == null)
                {
                    IsDestructive = false;
                    zipEntry = zip.GetEntry("package.xml");
                }
                else
                {
                    IsDestructive = true;
                }

                using (Stream zipStream = zipEntry.Open())
                {
                    Manifest = new Manifest(null);
                    Manifest.Load(zipStream);
                }
            }
        }

        /// <summary>
        /// Save the package to file.
        /// </summary>
        /// <param name="client">The client to download files from.</param>
        public void Save(SalesForceClient client)
        {
            if (FileName == null)
                throw new Exception("Can't save to file.  FileName is null.");

            if (File.Exists(FileName))
                File.Delete(FileName);

            if (Manifest.Groups.Count == 0)
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry zipEntry = zip.CreateEntry(IsDestructive ? "destructiveChanges.xml" : "package.xml");
                        using (Stream output = zipEntry.Open())
                            Manifest.Save(output);
                    }
                }
            }
            else if (IsDestructive)
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry zipEntry = zip.CreateEntry("destructiveChanges.xml");
                        using (Stream output = zipEntry.Open())
                        {
                            Manifest.Save(output);
                        }

                        Manifest empty = new Manifest("package");
                        ZipArchiveEntry packageEntry = zip.CreateEntry("package.xml");
                        using (Stream packageOutput = packageEntry.Open())
                            empty.Save(packageOutput);
                    }
                }
            }
            else
            {
                if (client == null)
                    throw new ArgumentNullException("client");

                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    byte[] bits = client.GetSourceFileContentAsPackage(Manifest);
                    fs.Write(bits, 0, bits.Length);
                }
            }
        }

        /// <summary>
        /// Delete the package.
        /// </summary>
        public void Delete()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }

        /// <summary>
        /// Check for logical equality.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>true if the obj parameter is logically equivalent with this object.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns a hashcode based on the LocalPath value.
        /// </summary>
        /// <returns>A hashcode for this object.</returns>
        public override int GetHashCode()
        {
            if (FileName == null)
                return 0;
            else
                return FileName.ToUpper().GetHashCode();
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
            Package other = obj as Package;
            if (other == null)
                return -1;

            return String.Compare(this.FileName, other.FileName, true);
        }

        #endregion
    }
}
