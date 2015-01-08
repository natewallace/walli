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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesForceLanguage;
using SalesForceLanguage.Apex.CodeModel;

namespace Wallace.Language.Apex.Tests
{
    /// <summary>
    /// Test the symbol related code.
    /// </summary>
    [TestClass]
    public class SymbolTest
    {
        #region Methods

        /// <summary>
        /// Test the serialization methods.
        /// </summary>
        [TestMethod]
        public void Symbol_Serialize()
        {
            Constructor constructorOne = new Constructor(
                new TextPosition(1, 10),
                "MyFirstConstructor",
                new TextSpan(new TextPosition(1, 10), new TextPosition(1, 20)),
                SymbolModifier.Public,
                new Parameter[] 
                { 
                    new Parameter(new TextPosition(0,0), "one", null, "string"), 
                    new Parameter(new TextPosition(0,0), "two", null, "integer")
                });

            Constructor constructorTwo = new Constructor(
                new TextPosition(5, 10),
                "MySecondConstructor",
                new TextSpan(new TextPosition(5, 20), new TextPosition(5, 25)),
                SymbolModifier.Public,
                new Parameter[] 
                { 
                    new Parameter(new TextPosition(0,0), "num", null, "integer"), 
                    new Parameter(new TextPosition(0,0), "input", null, "mytype")
                });

            Property propertyOne = new Property(
                new TextPosition(7, 1),
                "MyPropertyNumberOne",
                new TextSpan(new TextPosition(7, 1), new TextPosition(9, 10)),
                SymbolModifier.Private,
                "string");

            Property propertyTwo = new Property(
                new TextPosition(12, 1),
                "MyPropertyNumberTwo",
                new TextSpan(new TextPosition(12, 1), new TextPosition(15, 10)),
                SymbolModifier.Public,
                "mytype");

            Method methodOne = new Method(
                new TextPosition(1, 10),
                "MyFirstMethod",
                new TextSpan(new TextPosition(1, 10), new TextPosition(1, 20)),
                SymbolModifier.Public,
                "mytype",
                new Parameter[] 
                { 
                    new Parameter(new TextPosition(0,0), "onex", null, "string"), 
                    new Parameter(new TextPosition(0,0), "twox", null, "integer")
                });

            Method methodTwo = new Method(
                new TextPosition(1, 10),
                "MySecondMethod",
                new TextSpan(new TextPosition(1, 10), new TextPosition(1, 20)),
                SymbolModifier.Public,
                "string",
                new Parameter[] 
                { 
                    new Parameter(new TextPosition(0,0), "onex", null, "string"), 
                    new Parameter(new TextPosition(0,0), "twox", null, "integer")
                });

            SymbolTable symbolTable = new SymbolTable(
                new TextPosition(1, 1),
                "MyTestClass",
                new TextSpan(new TextPosition(1, 1), new TextPosition(20, 20)),
                null,
                SymbolModifier.Public,
                SymbolTableType.Class,
                null,
                null,
                new Constructor[] { constructorOne, constructorTwo },
                new Property[] { propertyOne, propertyTwo },
                new Method[] { methodOne, methodTwo },
                null,
                new string[] { "interfaceOne", "interfaceTwo", "interfaceThree" },
                null);

            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(typeof(SymbolTable[]));
                ser.Serialize(ms, new SymbolTable[] { symbolTable, symbolTable });

                ms.Position = 0;
                Console.WriteLine(Encoding.ASCII.GetString(ms.ToArray()));

                ms.Position = 0;
                SymbolTable[] actual = ser.Deserialize(ms) as SymbolTable[];
                //Assert.AreEqual(2, actual.Constructors.Length, "constructors");
                //Assert.AreEqual(2, actual.Properties.Length, "properties");
                //Assert.AreEqual(2, actual.Methods.Length, "methods");
                //Assert.AreEqual(3, actual.Interfaces.Length, "interfaces");
            }
        }

        #endregion
    }
}
