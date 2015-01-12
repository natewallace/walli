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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// A user in SalesForce.
    /// </summary>
    public class User
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="name">Name.</param>
        public User(string id, string name)
        {
            Id = id;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The user id.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The user name.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Check to see if the two users are logically equal.
        /// </summary>
        /// <param name="a">The first user in the comparison.</param>
        /// <param name="b">The second user in the comparison.</param>
        /// <returns>true if the two users are logically equal, false if they are not.</returns>
        public static bool Equals(User a, User b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;

            return a.Id == b.Id;
        }

        /// <summary>
        /// Check to see if the given object is logically equal to this object.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns>true if this object is logically equal to the other object, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            return User.Equals(this, obj as User);
        }

        /// <summary>
        /// Get a hashcode for this object.
        /// </summary>
        /// <returns>A hashcode for this object.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns the Name property.
        /// </summary>
        /// <returns>The Name property.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
