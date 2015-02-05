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

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// Used to create difference files.
    /// </summary>
    public class DiffUtility
    {
        /// <summary>
        /// Get the difference between the two versions of text using the Myer algorithm.
        /// </summary>
        /// <param name="olderText">The older version of the text in the comparison.</param>
        /// <param name="newerText">The newer version of the text in the comparison.</param>
        /// <returns>A patch file that describes the differences.</returns>
        public static string Myer(string olderText, string newerText)
        {
            StringBuilder result = new StringBuilder();

            DiffMatchPatch.Differencer dmp = new DiffMatchPatch.Differencer();
            List<DiffMatchPatch.Diff> diffs = dmp.diff_main_line(olderText, newerText);

            foreach (DiffMatchPatch.Diff d in diffs)
            {
                // get prefix
                string prefix = null;
                switch (d.operation)
                {
                    case DiffMatchPatch.Operation.EQUAL:
                        prefix = "    ";
                        break;

                    case DiffMatchPatch.Operation.DELETE:
                        prefix = "-   ";
                        break;

                    default:
                        prefix = "+   ";
                        break;
                }

                // format the lines
                using (StringReader reader = new StringReader(d.text))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                        result.AppendFormat("{0}{1}{2}", prefix, line, Environment.NewLine);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Get the difference between the two versions of text using the Patience algorithm.
        /// </summary>
        /// <param name="olderText">The older version of the text in the comparison.</param>
        /// <param name="newerText">The newer version of the text in the comparison.</param>
        /// <returns>A patch file that describes the differences.</returns>
        public static string Patience(string olderText, string newerText)
        {
            // format inputs
            if (olderText == null)
                olderText = String.Empty;
            List<string> olderList = new List<string>();
            using (StringReader reader = new StringReader(olderText))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    olderList.Add(line);
                }
            }

            if (newerText == null)
                newerText = String.Empty;
            List<string> newerList = new List<string>();
            using (StringReader reader = new StringReader(newerText))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                    newerList.Add(line);
            }

            // get differences
            DiffLib.PatienceSequenceMatcher<string> matcher = new DiffLib.PatienceSequenceMatcher<string>();
            DiffLib.Differencer<string> differencer = new DiffLib.Differencer<string>(matcher);
            IEnumerable<DiffLib.DifferenceInstruction> diffs = differencer.FindDifferences(
                olderList,
                newerList);

            // create text result
            StringBuilder result = new StringBuilder();
            foreach (DiffLib.DifferenceInstruction di in diffs)
            {
                switch (di.Operation)
                {
                    case DiffLib.DifferenceOperation.Inserted:
                        for (int i = di.SubSequence.RightIndex; i <= di.SubSequence.RightEndIndex; i++)
                            result.AppendLine(String.Format("+   {0}", newerList[i]));
                        break;

                    case DiffLib.DifferenceOperation.Removed:
                        for (int i = di.SubSequence.LeftIndex; i <= di.SubSequence.LeftEndIndex; i++)
                            result.AppendLine(String.Format("-   {0}", olderList[i]));
                        break;

                    case DiffLib.DifferenceOperation.Replaced:
                        for (int i = di.SubSequence.LeftIndex; i <= di.SubSequence.LeftEndIndex; i++)
                            result.AppendLine(String.Format("-   {0}", olderList[i]));
                        for (int i = di.SubSequence.RightIndex; i <= di.SubSequence.RightEndIndex; i++)
                            result.AppendLine(String.Format("+   {0}", newerList[i]));
                        break;

                    case DiffLib.DifferenceOperation.Equal:
                        for (int i = di.SubSequence.LeftIndex; i <= di.SubSequence.LeftEndIndex; i++)
                            result.AppendLine(String.Format("    {0}", olderList[i]));
                        break;

                    default:
                        break;
                }
            }

            return result.ToString();
        }
    }
}
