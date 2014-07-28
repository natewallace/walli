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
    /// A class declaration.
    /// </summary>
    public class ClassDeclaration : CodeElementBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="location">Location.</param>
        /// <param name="fields">Fields.</param>
        /// <param name="constructors">Constructors.</param>
        /// <param name="properties">Properties.</param>
        /// <param name="methods">Methods.</param>
        /// <param name="innerClasses">InnerClasses.</param>
        /// <param name="innerInterfaces">InnerInterfaces.</param>
        public ClassDeclaration(
            NameDeclaration name,
            TextLocation location,
            FieldDeclaration[] fields,
            ConstructorDeclaration[] constructors,
            PropertyDeclaration[] properties,
            MethodDeclaration[] methods,
            ClassDeclaration[] innerClasses,
            InterfaceDeclaration[] innerInterfaces)
            : base(name, location)
        {
            Fields = fields ?? new FieldDeclaration[0];
            Constructors = constructors ?? new ConstructorDeclaration[0];
            Properties = properties ?? new PropertyDeclaration[0];
            Methods = methods ?? new MethodDeclaration[0];
            InnerClasses = innerClasses ?? new ClassDeclaration[0];
            InnerInterfaces = innerInterfaces ?? new InterfaceDeclaration[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The fields that belong to this class.
        /// </summary>
        public FieldDeclaration[] Fields { get; private set; }

        /// <summary>
        /// The constructors that belong to this class.
        /// </summary>
        public ConstructorDeclaration[] Constructors { get; private set; }

        /// <summary>
        /// The properties that belong to this class.
        /// </summary>
        public PropertyDeclaration[] Properties { get; private set; }

        /// <summary>
        /// The methods that belong to this class.
        /// </summary>
        public MethodDeclaration[] Methods { get; private set; }

        /// <summary>
        /// The classes that belong to this class.
        /// </summary>
        public ClassDeclaration[] InnerClasses { get; private set; }

        /// <summary>
        /// The interfaces that belong to this class.
        /// </summary>
        public InterfaceDeclaration[] InnerInterfaces { get; private set; }

        #endregion
    }
}
