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

namespace SalesForceData
{
    /// <summary>
    /// An item within a package manifest.
    /// </summary>
    public class PackageManifestItem
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        public PackageManifestItem(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or empty.", "name");

            Name = name;
            Group = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="group">Group.</param>
        public PackageManifestItem(string name, PackageManifestItemGroup group)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or empty.", "name");

            Name = name;
            Group = group;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The group this item belongs to.
        /// </summary>
        public PackageManifestItemGroup Group { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the name of this item.
        /// </summary>
        /// <returns>The name of this item.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
