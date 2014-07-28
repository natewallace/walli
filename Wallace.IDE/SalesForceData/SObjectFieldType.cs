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
    /// A field on an SObject.
    /// </summary>
    public class SObjectFieldType : IComparable
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field">The field to build this object with.</param>
        public SObjectFieldType(SalesForceAPI.Partner.Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            AutoNumber = field.autoNumber;
            ByteLength = field.byteLength;
            Calculated = field.calculated;
            CalculatedFormula = field.calculatedFormula;
            CascadeDelete = field.cascadeDelete;
            CascadeDeleteSpecified = field.cascadeDeleteSpecified;
            CaseSensitive = field.caseSensitive;
            ControllerName = field.controllerName;
            Createable = field.createable;
            Custom = field.custom;
            DefaultValueFormula = field.defaultValueFormula;
            DefaultedOnCreate = field.defaultedOnCreate;
            DependentPicklist = field.dependentPicklist;
            DependentPicklistSpecified = field.dependentPicklistSpecified;
            DeprecatedAndHidden = field.deprecatedAndHidden;
            Digits = field.digits;
            DisplayLocationInDecimal = field.displayLocationInDecimal;
            DisplayLocationInDecimalSpecified = field.displayLocationInDecimalSpecified;
            ExternalId = field.externalId;
            ExternalIdSpecified = field.externalIdSpecified;
            Filterable = field.filterable;
            Groupable = field.groupable;
            HtmlFormatted = field.htmlFormatted;
            HtmlFormattedSpecified = field.htmlFormattedSpecified;
            IdLookup = field.idLookup;
            InlineHelpText = field.inlineHelpText;
            Label = field.label;
            Length = field.length;
            Name = field.name;
            NameField = field.nameField;
            NamePointing = field.namePointing;
            NamePointingSpecified = field.namePointingSpecified;
            Nillable = field.nillable;
            Permissionable = field.permissionable;
            Precision = field.precision;
            ReferenceTo = field.referenceTo;
            RelationshipName = field.relationshipName;
            RelationshipOrder = field.relationshipOrder;
            RelationshipOrderSpecified = field.relationshipOrderSpecified;
            RestrictedDelete = field.restrictedDelete;
            RestrictedDeleteSpecified = field.restrictedDeleteSpecified;
            RestrictedPicklist = field.restrictedPicklist;
            Scale = field.scale;
            Sortable = field.sortable;
            SortableSpecified = field.sortableSpecified;
            Unique = field.unique;
            Updateable = field.updateable;
            WriteRequiresMasterRead = field.writeRequiresMasterRead;
            WriteRequiresMasterReadSpecified = field.writeRequiresMasterReadSpecified;

            switch (field.type)
            {
                case SalesForceAPI.Partner.fieldType.@string:
                    FieldType = FieldType.String;
                    break;

                case SalesForceAPI.Partner.fieldType.picklist:
                    FieldType = FieldType.Picklist;
                    break;

                case SalesForceAPI.Partner.fieldType.multipicklist:
                    FieldType = FieldType.MultiPicklist;
                    break;

                case SalesForceAPI.Partner.fieldType.combobox:
                    FieldType = FieldType.Combobox;
                    break;

                case SalesForceAPI.Partner.fieldType.reference:
                    FieldType = FieldType.Reference;
                    break;

                case SalesForceAPI.Partner.fieldType.base64:
                    FieldType = FieldType.Base64;
                    break;

                case SalesForceAPI.Partner.fieldType.boolean:
                    FieldType = FieldType.Boolean;
                    break;

                case SalesForceAPI.Partner.fieldType.currency:
                    FieldType = FieldType.Currency;
                    break;

                case SalesForceAPI.Partner.fieldType.textarea:
                    FieldType = FieldType.TextArea;
                    break;

                case SalesForceAPI.Partner.fieldType.@int:
                    FieldType = FieldType.Int;
                    break;

                case SalesForceAPI.Partner.fieldType.@double:
                    FieldType = FieldType.Double;
                    break;

                case SalesForceAPI.Partner.fieldType.percent:
                    FieldType = FieldType.Percent;
                    break;

                case SalesForceAPI.Partner.fieldType.phone:
                    FieldType = FieldType.Phone;
                    break;

                case SalesForceAPI.Partner.fieldType.id:
                    FieldType = FieldType.Id;
                    break;

                case SalesForceAPI.Partner.fieldType.date:
                    FieldType = FieldType.Date;
                    break;

                case SalesForceAPI.Partner.fieldType.datetime:
                    FieldType = FieldType.DateTime;
                    break;

                case SalesForceAPI.Partner.fieldType.time:
                    FieldType = FieldType.Time;
                    break;

                case SalesForceAPI.Partner.fieldType.url:
                    FieldType = FieldType.Url;
                    break;

                case SalesForceAPI.Partner.fieldType.email:
                    FieldType = FieldType.Email;
                    break;

                case SalesForceAPI.Partner.fieldType.encryptedstring:
                    FieldType = FieldType.EncryptedString;
                    break;

                case SalesForceAPI.Partner.fieldType.datacategorygroupreference:
                    FieldType = FieldType.DataCategoryGroupReference;
                    break;

                case SalesForceAPI.Partner.fieldType.location:
                    FieldType = FieldType.Location;
                    break;

                case SalesForceAPI.Partner.fieldType.anyType:
                    FieldType = FieldType.Any;
                    break;

                default:
                    FieldType = FieldType.Unknown;
                    break;
            }
        }

        #endregion

        #region Properties

        public bool AutoNumber { get; private set; }

        public int ByteLength { get; private set; }

        public bool Calculated { get; private set; }

        public string CalculatedFormula { get; private set; }

        public bool CascadeDelete { get; private set; }

        public bool CascadeDeleteSpecified { get; private set; }

        public bool CaseSensitive { get; private set; }

        public string ControllerName { get; private set; }

        public bool Createable { get; private set; }

        public bool Custom { get; private set; }

        public string DefaultValueFormula { get; private set; }

        public bool DefaultedOnCreate { get; private set; }

        public bool DependentPicklist { get; private set; }

        public bool DependentPicklistSpecified { get; private set; }

        public bool DeprecatedAndHidden { get; private set; }

        public int Digits { get; private set; }

        public bool DisplayLocationInDecimal { get; private set; }

        public bool DisplayLocationInDecimalSpecified;

        public bool ExternalId { get; private set; }

        public bool ExternalIdSpecified { get; private set; }

        public bool Filterable { get; private set; }

        public bool Groupable { get; private set; }

        public bool HtmlFormatted { get; private set; }

        public bool HtmlFormattedSpecified { get; private set; }

        public bool IdLookup { get; private set; }

        public string InlineHelpText { get; private set; }

        public string Label { get; private set; }

        public int Length { get; private set; }

        public string Name { get; private set; }

        public bool NameField { get; private set; }

        public bool NamePointing { get; private set; }

        public bool NamePointingSpecified { get; private set; }

        public bool Nillable { get; private set; }

        public bool Permissionable { get; private set; }

        public int Precision { get; private set; }

        public string[] ReferenceTo { get; private set; }

        public string RelationshipName { get; private set; }

        public int RelationshipOrder { get; private set; }

        public bool RelationshipOrderSpecified { get; private set; }

        public bool RestrictedDelete { get; private set; }

        public bool RestrictedDeleteSpecified { get; private set; }

        public bool RestrictedPicklist { get; private set; }

        public int Scale { get; private set; }

        public bool Sortable { get; private set; }

        public bool SortableSpecified;

        public FieldType FieldType { get; private set; }

        public bool Unique { get; private set; }

        public bool Updateable { get; private set; }

        public bool WriteRequiresMasterRead { get; private set; }

        public bool WriteRequiresMasterReadSpecified { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the name of this field.
        /// </summary>
        /// <returns>The name of this field.</returns>
        public override string ToString()
        {
            return Name;
        }

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
            SObjectFieldType other = obj as SObjectFieldType;
            if (other == null)
                return -1;

            if (this.Name == null)
                return 1;

            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}
