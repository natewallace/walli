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

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// Base class for code elements.
    /// </summary>
    public abstract class CodeElementBase : ICodeElement
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="location">Location.</param>
        public CodeElementBase(NameDeclaration name, TextLocation location)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (location == null)
                throw new ArgumentNullException("location");

            Name = name;
            Location = location;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Name text value is returned.
        /// </summary>
        /// <returns>The Name text value.</returns>
        public override string ToString()
        {
            return Name.Text;
        }

        #endregion

        #region ICodeElement Members

        /// <summary>
        /// The name for the element.
        /// </summary>
        public NameDeclaration Name { get; protected set; }

        /// <summary>
        /// The location of the entire element.
        /// </summary>
        public TextLocation Location { get; protected set; }

        #endregion

        #region IComparable<ICodeElement> Members

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
            ICodeElement other = obj as ICodeElement;
            if (other == null)
                return -1;

            int result = String.Compare(this.GetType().FullName, other.GetType().FullName);
            if (result != 0)
                return result;

            return String.Compare(this.Name.Text, other.Name.Text);
        }

        #endregion
    }
}
