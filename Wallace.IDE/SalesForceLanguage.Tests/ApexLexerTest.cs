using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesForceLanguage.Apex;
using SalesForceLanguage.Apex.Parser;

namespace Wallace.Language.Apex.Tests
{
    /// <summary>
    /// Test the ApexLexer class.
    /// </summary>
    [TestClass]
    public class ApexLexerTest
    {
        #region Methods

        /// <summary>
        /// Test the scan method.
        /// </summary>
        [TestMethod]
        public void ApexLexer_Scan()
        {
            foreach (string file in Directory.GetFiles(".\\TestInput", "lexerTest*.txt"))
            {
                Console.WriteLine("File: " + file);

                using (FileStream fs = File.OpenRead(file))
                {
                    ApexLexer scanner = new ApexLexer(fs);
                    int token = 0;
                    while ((token = scanner.yylex()) != (int)Tokens.EOF)
                    {
                        Console.WriteLine((Tokens)token);
                    }
                }

                Console.WriteLine();
            }
        }

        #endregion
    }
}
