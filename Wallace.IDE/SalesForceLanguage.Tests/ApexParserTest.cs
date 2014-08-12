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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesForceLanguage.Apex.Parser;
using SalesForceLanguage.Apex;
using SalesForceLanguage;

namespace Wallace.Language.Apex.Tests
{
    /// <summary>
    /// Test the ApexParser class.
    /// </summary>
    [TestClass]
    public class ApexParserTest
    {
        #region Methods

        /// <summary>
        /// Test the ParseApex method.
        /// </summary>
        [TestMethod]
        public void ApexParser_ParseApex()
        {
            foreach (string file in Directory.GetFiles(".\\TestInput", "apexTest*.txt"))
            {
                Console.WriteLine("File: " + file);

                using (FileStream fs = File.OpenRead(file))
                {
                    ApexLexer scanner = new ApexLexer(fs);
                    ApexParser parser = new ApexParser(scanner);
                    ParseResult result = parser.ParseApex();

                    foreach (LanguageError err in result.Errors)
                        Console.WriteLine(err.ToString());

                    Assert.AreEqual(0, result.Errors.Length, "Errors occured parsing: " + file);
                }

                Console.WriteLine();
            }
        }

        #endregion
    }
}
