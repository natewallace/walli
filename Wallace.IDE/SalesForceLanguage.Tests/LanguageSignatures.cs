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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesForceLanguage;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.CodeModel;
using SalesForceLanguage.Apex.Parser;

namespace Wallace.Language.Apex.Tests
{
    /// <summary>
    /// This test method is used to generate xml for system classes.
    /// </summary>
    [TestClass]
    public class LanguageSignatures
    {
        /// <summary>
        /// Generates the xml.
        /// </summary>
        [TestMethod]
        public void GenerateSignatureXML()
        {
            using (FileStream fs = File.OpenRead(".\\TestInput\\SystemSignatures.txt"))
            {
                ApexLexer scanner = new ApexLexer(fs);
                ApexParser parser = new ApexParser(scanner);
                ParseResult result = parser.ParseApex();

                if (result.Errors.Length > 0)
                {
                    foreach (LanguageError err in result.Errors)
                        Console.WriteLine(err.ToString());
                    Assert.Fail("Errors in text.");
                }
                else
                {
                    PropertyInfo nameInfo = typeof(Symbol).GetProperty("Name");
                    FieldInfo fi = typeof(TypedSymbol).GetField("_type", BindingFlags.NonPublic | BindingFlags.Instance);
                    PropertyInfo pi = typeof(SymbolTable).GetProperty("TableType");

                    List<SymbolTable> symbols = new List<SymbolTable>();
                    foreach (SymbolTable symbol in result.Symbols.InnerClasses)
                    {
                        if (symbol.Name == "trigger")
                        {
                            fi.SetValue(symbol, "System.Trigger");
                            nameInfo.SetValue(symbol, "Trigger");
                        }
                        else
                        {
                            fi.SetValue(symbol, String.Format("System.{0}", symbol.Name));
                        }

                        if (symbol.InnerClasses.Length > 0 &&
                            symbol.Fields.Length == 0 &&
                            symbol.Constructors.Length == 0 &&
                            symbol.Properties.Length == 0 &&
                            symbol.Methods.Length == 0 &&
                            symbol.Interfaces.Length == 0 &&
                            String.IsNullOrWhiteSpace(symbol.Extends))
                            pi.SetValue(symbol, SymbolTableType.Namespace);
                        symbols.Add(symbol);
                    }

                    XmlSerializer ser = new XmlSerializer(typeof(SymbolTable[]));
                    StringBuilder sb = new StringBuilder();
                    using (StringWriter sw = new StringWriter(sb))
                    {
                        ser.Serialize(sw, symbols.ToArray());
                        Console.Write(sb.ToString());
                    }
                }
            }
        }
    }
}
