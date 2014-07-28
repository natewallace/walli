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
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Do auto indent for block comments.
    /// </summary>
    public class ApexIndentationStrategy : DefaultIndentationStrategy
    {
        #region Methods

        /// <summary>
        /// Do indents.
        /// </summary>
        /// <param name="document">The document to do indents on.</param>
        /// <param name="line">The newly added line.</param>
        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (line == null)
                throw new ArgumentNullException("line");

            bool customIndent = false;             
             
            DocumentLine previousLine = line.PreviousLine;
            if (previousLine != null)
            {
                // check for opening block comment
                string previousLineText = document.GetText(previousLine.Offset, previousLine.Length);
                if (previousLineText != null)
                {
                    Match match = Regex.Match(previousLineText, @"^[ \t]*/?\*(?!/)");
                    if (match != null && match.Success)
                    {
                        customIndent = true;
                        StringBuilder sb = new StringBuilder();
                        foreach (char c in match.Value)
                        {
                            switch (c)
                            {
                                case '/':
                                    sb.Append(' ');
                                    break;

                                case '*':
                                    sb.Append('*');
                                    break;

                                default:
                                    sb.Append(c);
                                    break;
                            }
                        }

                        sb.Append(' ');
                        document.Insert(line.Offset, sb.ToString());
                    }
                }
            }

            // default
            if (!customIndent)
                base.IndentLine(document, line);
        }

        #endregion
    }
}
