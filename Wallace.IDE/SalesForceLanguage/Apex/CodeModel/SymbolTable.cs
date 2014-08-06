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

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A class or interface.
    /// </summary>
    public class SymbolTable : Symbol
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="constructors">Constructors</param>
        /// <param name="properties">Properties</param>
        /// <param name="methods">Methods</param>
        /// <param name="interfaces">Interfaces</param>
        /// <param name="innerClasses">InnerClasses</param>
        public SymbolTable(
            TextPosition location,
            string name,
            Constructor[] constructors,
            VisibilitySymbol[] properties,
            Method[] methods,
            string[] interfaces,
            SymbolTable[] innerClasses)
            : base(location, name, name)
        {
            Constructors = constructors ?? new Constructor[0];
            Properties = properties ?? new VisibilitySymbol[0];
            Methods = methods ?? new Method[0];
            Interfaces = interfaces ?? new string[0];
            InnerClasses = innerClasses ?? new SymbolTable[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Constructors for the table.
        /// </summary>
        public Constructor[] Constructors { get; set; }

        /// <summary>
        /// Properties for the table.
        /// </summary>
        public VisibilitySymbol[] Properties { get; private set; }

        /// <summary>
        /// Methods for the table.
        /// </summary>
        public Method[] Methods { get; set; }

        /// <summary>
        /// Interfaces or the table.
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// Inner classes or interfaces for the table.
        /// </summary>
        public SymbolTable[] InnerClasses { get; set; }

        #endregion
    }
}
