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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SalesForceData
{
    /// <summary>
    /// Credentals used to logon to a salesforce server.
    /// </summary>
    public class SalesForceCredential : IComparable, IXmlSerializable
    {
        #region Constructors

        /// <summary>
        /// Don't use.  Required by xml serializer.
        /// </summary>
        public SalesForceCredential()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="domain">Domain.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="token">Token.</param>
        public SalesForceCredential(
            SalesForceDomain domain, 
            string username, 
            string password, 
            string token)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username is null or empty.");

            Domain = domain;
            Username = username.Trim().ToLower();
            Password = password;
            Token = token;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of server to logon to.
        /// </summary>
        public SalesForceDomain Domain { get; private set; }

        /// <summary>
        /// The username to logon with.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// The password to logon with.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The token to login with.
        /// </summary>
        public string Token { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compare two SalesForceCredential objects.
        /// </summary>
        /// <param name="a">The first object in the comparison.</param>
        /// <param name="b">The second object in the comparison.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the lexical relationship between the
        /// two comparands.  
        /// Less than zero a is less than b. 
        /// Zero a equals b. 
        /// Greater than zero a is greater than b.
        /// </returns>
        public static int Compare(SalesForceCredential a, SalesForceCredential b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;

            return String.Compare(a.Username, b.Username, true);
        }

        /// <summary>
        /// Check to see if the given object is logically equivalent to this object.
        /// </summary>
        /// <param name="obj">The object to compare with this object.</param>
        /// <returns>true if the given object is logically equal to this object, false if it isn't.</returns>
        public override bool Equals(object obj)
        {
            return (Compare(this, obj as SalesForceCredential) == 0);
        }

        /// <summary>
        /// Use the hash code from the Username property.
        /// </summary>
        /// <returns>The hash code from the Username property.</returns>
        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }

        /// <summary>
        /// The Username property is returned.
        /// </summary>
        /// <returns>The Username property.</returns>
        public override string ToString()
        {
            return Username;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: 
        /// Less than zero = This instance precedes obj in the sort order. 
        /// Zero = This instance occurs in the same position in the sort order as obj. 
        /// Greater than zero = This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            return Compare(this, obj as SalesForceCredential);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Not used.
        /// </summary>
        /// <returns>null.</returns>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read this object from xml.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public void ReadXml(XmlReader reader)
        {
            Domain = (SalesForceDomain)Enum.Parse(typeof(SalesForceDomain), reader["domain"]);
            Username = reader["username"];
            Password = reader["password"];
            Token = reader["token"];
        }

        /// <summary>
        /// Write this object to xml.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("domain", Domain.ToString());
            writer.WriteAttributeString("username", Username);
            writer.WriteAttributeString("password", Password);
            writer.WriteAttributeString("token", Token);
        }

        #endregion
    }
}
