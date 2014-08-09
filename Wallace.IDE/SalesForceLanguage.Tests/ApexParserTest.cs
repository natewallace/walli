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
                    parser.ParseApex();

                    foreach (LanguageError err in parser.ParserErrors)
                        Console.WriteLine(err.ToString());

                    Assert.AreEqual(0, parser.ParserErrors.Length, "Errors occured parsing: " + file);
                }

                Console.WriteLine();
            }
        }

        #endregion
    }
}
