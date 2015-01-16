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
    /// A checked out file.
    /// </summary>
    public class CheckoutFile
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityId">The id of the file.</param>
        /// <param name="userId">The id of the user.</param>
        /// <param name="names">A combination of the user name and the file name seperated by a ':' character.</param>
        internal CheckoutFile(string entityId, string userId, string names)
        {
            EntityId = entityId;

            string userName = null;
            string fileName = null;

            int index = names.IndexOf(':');
            if (index != -1)
            {
                userName = names.Substring(0, index);
                fileName = names.Substring(index + 1, names.Length - index - 1);
            }
            else
            {
                userName = names;
                fileName = "Unknown";
            }

            User = new User(userId, userName);
            EntityName = fileName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityId">The id of the file.</param>
        /// <param name="user">The user who has the file checked out.</param>
        /// <param name="entityName">The entity name.</param>
        internal CheckoutFile(string entityId, User user, string entityName)
        {
            EntityId = entityId;
            User = user;
            EntityName = entityName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id of the file.
        /// </summary>
        public string EntityId { get; private set; }

        /// <summary>
        /// The display name for the file.
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// The user that has the file checked out.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// The combination of the entity name and the user name.
        /// </summary>
        public string CombinedName
        {
            get
            {
                string userName = (User == null) ? String.Empty : User.Name.Replace(':', ' ');
                string fileName = (EntityName == null) ? String.Empty : EntityName.Replace(':', ' ');

                if (userName.Length + fileName.Length < 128)
                    return String.Format("{0}:{1}", userName, fileName);

                StringBuilder sb = new StringBuilder();
                int total = 128;

                if (userName.Length > 64)
                {
                    sb.Append(userName.Substring(0, 64));
                    total -= 64;
                }
                else
                {
                    sb.Append(userName);
                    total -= userName.Length;
                }

                sb.Append(":");
                total--;

                if (fileName.Length > total)
                    sb.Append(fileName.Substring(0, total));
                else
                    sb.Append(fileName);

                return sb.ToString();
            }
        }

        /// <summary>
        /// The file type name and file name.
        /// </summary>
        public string FullEntityName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(EntityName))
                    return EntityId;

                return EntityName;
            }
        }

        #endregion
    }
}
