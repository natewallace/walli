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

using System.Text;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A method declaration.
    /// </summary>
    public class MethodDeclaration : CodeElementBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="location">Location.</param>
        /// <param name="parameters">Parameters.</param>
        public MethodDeclaration(
            NameDeclaration name, 
            TextLocation location,
            ParameterDeclaration[] parameters)
            : base(name, location)
        {
            Parameters = parameters ?? new ParameterDeclaration[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The parameters for the method.
        /// </summary>
        public ParameterDeclaration[] Parameters { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return a human readible string that represents this instance.
        /// </summary>
        /// <returns>A human readible string that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name.Text);

            sb.Append("(");
            foreach (ParameterDeclaration p in Parameters)
                sb.AppendFormat("{0}, ", p.ParameterType);
            if (Parameters.Length > 0)
                sb.Length = sb.Length - 2;
            sb.Append(")");

            return sb.ToString();
        }

        #endregion
    }
}
