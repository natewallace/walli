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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// Data for an identicon image.
    /// </summary>
    public class Identicon
    {
        #region Fields

        /// <summary>
        /// Holds identicions created with the Get method.
        /// </summary>
        private static Dictionary<string, Identicon> _map = new Dictionary<string,Identicon>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The data to turn into an identicon.</param>
        public Identicon(string data)
        {
            if (data == null)
                data = String.Empty;

            using (MD5 crypto = MD5.Create())
            {
                // create hash
                byte[] hash = crypto.ComputeHash(Encoding.UTF8.GetBytes(data));
                if (hash == null || hash.Length != 16)
                    throw new Exception("Failed to generate hash.");

                // get color
                Color = Color.FromRgb(hash[0], hash[1], hash[2]);

                // get bits
                List<bool> bitList = new List<bool>();
                for (int i = 3; i < 16; i++)
                {
                    bitList.Add((hash[i] & 15) % 2 == 0);
                    bitList.Add((hash[i] >> 4) % 2 == 0);
                }
                bitList.RemoveAt(25);
                Bits = bitList.ToArray();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The color for the identicon.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// The 25 bits for the identicon.
        /// </summary>
        public bool[] Bits { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get an identicon for the given data.  The results are cached so future calls are faster.
        /// </summary>
        /// <param name="data">The data to get the Identicon for.</param>
        /// <returns>An identicon for the given data.</returns>
        public static Identicon Get(string data)
        {
            if (data == null)
                data = String.Empty;

            if (_map.ContainsKey(data))
                return _map[data];

            Identicon i = new Identicon(data);
            _map.Add(data, i);
            return i;
        }

        #endregion
    }
}
