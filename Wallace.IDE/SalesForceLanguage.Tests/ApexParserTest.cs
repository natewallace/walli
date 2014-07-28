using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesForceLanguage.Apex.Parser;
using SalesForceLanguage.Apex;

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
                    TextSymbolDocument symbolDoc = new TextSymbolDocument();
                    ApexLexer scanner = new ApexLexer(fs, symbolDoc);
                    ApexParser parser = new ApexParser(scanner);
                    parser.ParseApex();

                    Assert.AreEqual(0, parser.ParserErrors.Length, "Errors occured parsing: " + file);
                }

                Console.WriteLine();
            }
        }

        #endregion
    }
}
