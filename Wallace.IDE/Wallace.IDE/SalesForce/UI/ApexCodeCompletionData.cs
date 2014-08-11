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
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using SalesForceLanguage.Apex.CodeModel;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Apex code completion item.
    /// </summary>
    public class ApexCodeCompletionData : ICompletionData
    {
        #region Fields

        /// <summary>
        /// The symbol used for the code completion.
        /// </summary>
        private Symbol _symbol;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol used for the code completion.</param>
        public ApexCodeCompletionData(Symbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            _symbol = symbol;
        }

        #endregion

        #region ICompletionData Members

        /// <summary>
        /// Do the completion.
        /// </summary>
        /// <param name="textArea">The text area to use.</param>
        /// <param name="completionSegment">The completion segment.</param>
        /// <param name="insertionRequestEventArgs">Insertion request arguments.</param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        /// <summary>
        /// Returns the text.
        /// </summary>
        public object Content
        {
            get { return Text; }
        }

        /// <summary>
        /// The type of symbol.
        /// </summary>
        public object Description
        {
            get { return _symbol.GetType().Name; }
        }

        /// <summary>
        /// The image for the symbol.
        /// </summary>
        public ImageSource Image
        {
            get 
            {
                if (_symbol is Property)
                {
                    return VisualHelper.LoadBitmap("Property.png");
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Not used.  Returns 1.
        /// </summary>
        public double Priority
        {
            get { return 1; }
        }

        /// <summary>
        /// The symbol name.
        /// </summary>
        public string Text
        {
            get { return _symbol.Name; }
        }

        #endregion
    }
}
