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

namespace SalesForceData
{
    /// <summary>
    /// A group of items within a pagkage manifest.
    /// </summary>
    public class ManifestItemGroup
    {
        #region Fields

        /// <summary>
        /// Supports the Items property.
        /// </summary>
        private List<ManifestItem> _items;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        public ManifestItemGroup(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or empty.", "name");

            Name = name;
            Manifest = null;
            _items = new List<ManifestItem>();
            Items = _items;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="manifest">Manifest.</param>
        public ManifestItemGroup(string name, Manifest manifest)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is null or empty.", "name");

            Name = name;
            Manifest = manifest;
            _items = new List<ManifestItem>();
            Items = _items;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of this group.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The manifest this group belongs to.
        /// </summary>
        public Manifest Manifest { get; set; }

        /// <summary>
        /// The items that belong to this group.
        /// </summary>
        public IReadOnlyCollection<ManifestItem> Items { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add the given item to this group if it isn't already a member.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>true if the item was added, false if it wasn't.</returns>
        public bool AddItem(ManifestItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            foreach (ManifestItem i in Items)
                if (String.Compare(i.Name, item.Name, true) == 0)
                    return false;

            item.Group = this;
            _items.Add(item);
            return true;
        }

        /// <summary>
        /// Removes the given item from this group if it exists.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if the item was removed, false if it wasn't.</returns>
        public bool RemoveItem(ManifestItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            for (int i = 0; i < _items.Count; i++)
            {
                if (String.Compare(_items[i].Name, item.Name, true) == 0)
                {
                    _items[i].Group = null;
                    _items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the name of this group.
        /// </summary>
        /// <returns>The name of this group.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
