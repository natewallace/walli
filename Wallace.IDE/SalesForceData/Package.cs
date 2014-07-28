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
using System.IO;
using System.IO.Compression;

namespace SalesForceData
{
    /// <summary>
    /// A package that contains source files which can be deployed.
    /// </summary>
    public class Package : IComparable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="localPath">LocalPath.</param>
        public Package(string localPath)
        {
            if (String.IsNullOrWhiteSpace(localPath))
                throw new ArgumentException("localPath is null or empty.", "localPath");

            LocalPath = localPath;

            if (File.Exists(LocalPath))
            {
                using (FileStream fs = new FileStream(LocalPath, FileMode.Open))
                {
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read))
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

                        using (Stream input = zipEntry.Open())
                        {
                            Manifest = PackageManifest.Load(input);
                        }
                    }
                }
            }
            else
            {
                Manifest = new PackageManifest();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The path to the package that is stored locally. 
        /// </summary>
        public string LocalPath { get; private set; }

        /// <summary>
        /// Get the name for the package.
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(LocalPath); }
        }

        /// <summary>
        /// The manifest for this package.
        /// </summary>
        public PackageManifest Manifest { get; private set; }

        /// <summary>
        /// If true this is a destructive package.
        /// </summary>
        public bool IsDestructive { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Save the package to file.
        /// </summary>
        /// <param name="client">The client to download files from.</param>
        public void Save(SalesForceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (File.Exists(LocalPath))
                File.Delete(LocalPath);

            if (Manifest.Groups.Count == 0)
            {
                using (FileStream fs = new FileStream(LocalPath, FileMode.Create))
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
                using (FileStream fs = new FileStream(LocalPath, FileMode.Create))
                {
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry zipEntry = zip.CreateEntry("destructiveChanges.xml");
                        using (Stream output = zipEntry.Open())
                        {
                            Manifest.Save(output);
                        }

                        PackageManifest empty = new PackageManifest();
                        ZipArchiveEntry packageEntry = zip.CreateEntry("package.xml");
                        using (Stream packageOutput = packageEntry.Open())
                            empty.Save(packageOutput);
                    }
                }
            }
            else
            {
                using (FileStream fs = new FileStream(LocalPath, FileMode.Create))
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
            if (File.Exists(LocalPath))
                File.Delete(LocalPath);
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
            return LocalPath.ToUpper().GetHashCode();
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

            return String.Compare(this.LocalPath, other.LocalPath, true);
        }

        #endregion
    }
}
