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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Folding strategy for apex code.
    /// </summary>
    public class ApexFoldingStrategy : AbstractFoldingStrategy
    {
        #region Methods

        /// <summary>
        /// Get foldings.
        /// </summary>
        /// <param name="document">The document to provide foldings for.</param>
        /// <param name="firstErrorOffset">Not used.</param>
        /// <returns>The foldings for this document.</returns>
        public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            firstErrorOffset = -1;

            List<NewFolding> result = new List<NewFolding>();
            MatchCollection regions = Regex.Matches(document.Text, "^[ \t]*//(end)?region.*", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Stack<Match> startStack = new Stack<Match>();
            foreach (Match region in regions)
            {
                if (region.Value.Trim().StartsWith("//region", StringComparison.CurrentCultureIgnoreCase))
                {
                    startStack.Push(region);
                }
                else
                {
                    if (startStack.Count > 0)
                    {
                        Match startRegion = startStack.Pop();
                        NewFolding folding = new NewFolding(startRegion.Index, region.Index + region.Length);
                        int index =  startRegion.Value.IndexOf("//region", StringComparison.CurrentCultureIgnoreCase);
                        index += 8;
                        if (index < startRegion.Value.Length)
                            folding.Name = String.Format(" {0} ", startRegion.Value.Substring(index).Trim());

                        result.Add(folding);
                    }
                }
            }

            return result.OrderBy(f => f.StartOffset);
        }

        #endregion
    }
}
