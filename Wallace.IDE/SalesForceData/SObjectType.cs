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

namespace SalesForceData
{
    /// <summary>
    /// Full details about an SObject type.
    /// </summary>
    public class SObjectType
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">The result to build this object from.</param>
        public SObjectType(SalesForceAPI.Partner.DescribeSObjectResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            Activateable = result.activateable;
            Createable = result.createable;
            Custom = result.custom;
            CustomSetting = result.customSetting;
            Deletable = result.deletable;
            DeprecatedAndHidden = result.deprecatedAndHidden;
            FeedEnabled = result.feedEnabled;
            KeyPrefix = result.keyPrefix;
            Label = result.label;
            LabelPlural = result.labelPlural;
            Layoutable = result.layoutable;
            Mergeable = result.mergeable;
            Name = result.name;
            Queryable = result.queryable;
            Replicateable = result.replicateable;
            Retrieveable = result.retrieveable;
            Searchable = result.searchable;
            Triggerable = result.triggerable;
            Undeletable = result.undeletable;
            Updateable = result.updateable;

            List<SObjectFieldType> fields = new List<SObjectFieldType>();
            if (result.fields != null)
                foreach (SalesForceAPI.Partner.Field f in result.fields)
                    fields.Add(new SObjectFieldType(f));

            Fields = fields.ToArray();
            Array.Sort(Fields);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if an object of this type can be activated.
        /// </summary>
        public bool Activateable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be created.
        /// </summary>
        public bool Createable { get; private set; }

        /// <summary>
        /// Indicates if this object type is custom.
        /// </summary>
        public bool Custom { get; private set; }

        /// <summary>
        /// Indicates if this object is a custom setting.
        /// </summary>
        public bool CustomSetting { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be deleted.
        /// </summary>
        public bool Deletable { get; private set; }

        /// <summary>
        /// Indicates if this object type is deprecated and hidden.
        /// </summary>
        public bool DeprecatedAndHidden { get; private set; }

        /// <summary>
        /// Indicates if the chatter feed is enabled for this object.
        /// </summary>
        public bool FeedEnabled { get; private set; }

        /// <summary>
        /// The key prefix for objects of this type.
        /// </summary>
        public string KeyPrefix { get; private set; }

        /// <summary>
        /// The label for this object type.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// The plural label for this object type.
        /// </summary>
        public string LabelPlural { get; private set; }

        /// <summary>
        /// Indicates if this object has a layout.
        /// </summary>
        public bool Layoutable { get; private set; }

        /// <summary>
        /// Indicates if objects of this type can be merged.
        /// </summary>
        public bool Mergeable { get; private set; }

        /// <summary>
        /// The name of this object type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be queried.
        /// </summary>
        public bool Queryable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be replicated.
        /// </summary>
        public bool Replicateable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be retrieved.
        /// </summary>
        public bool Retrieveable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be searched.
        /// </summary>
        public bool Searchable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be triggered.
        /// </summary>
        public bool Triggerable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be undeleted.
        /// </summary>
        public bool Undeletable { get; private set; }

        /// <summary>
        /// Indicates if an object of this type can be updated.
        /// </summary>
        public bool Updateable { get; private set; }

        /// <summary>
        /// The fields on the object type.
        /// </summary>
        public SObjectFieldType[] Fields { get; private set; }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare this object with another object.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: 
        /// Less than zero - This instance precedes obj in the sort order. 
        /// Zero - This instance occurs in the same position in the sort order as obj. 
        /// Greater than zero - This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            SObjectTypePartial other = obj as SObjectTypePartial;
            if (other == null)
                return -1;

            if (this.Name == null)
                return 1;

            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}
