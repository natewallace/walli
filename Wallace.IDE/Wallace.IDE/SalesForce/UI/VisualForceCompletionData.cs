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
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Code completions for visual force.
    /// </summary>
    public class VisualForceCompletionData : ICompletionData
    {
        #region Fields

        /// <summary>
        /// The text for the completion.
        /// </summary>
        private string _text;

        /// <summary>
        /// Flag that indicates if this is a tag.
        /// </summary>
        private bool _isTag;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text for the completion.</param>
        /// <param name="isTag">If true this completion is treated as a tag, if false it's treated as an attribute.</param>
        public VisualForceCompletionData(string text, bool isTag)
        {
            _text = text;
            _isTag = isTag;
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
            if (_isTag)
            {
                textArea.Document.Replace(completionSegment, String.Format("{0}>", Text));
            }
            else
            {
                textArea.Document.Replace(completionSegment, String.Format("{0}=\"\"", Text));
            }

            textArea.Caret.Offset = textArea.Caret.Offset - 1;
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
            get { return null; }
        }

        /// <summary>
        /// The image for the symbol.
        /// </summary>
        public ImageSource Image
        {
            get
            {
                if (_isTag)
                {
                    return VisualHelper.LoadBitmap("Element.png");
                }
                else
                {
                    if (Text.StartsWith("on", StringComparison.CurrentCultureIgnoreCase))
                        return VisualHelper.LoadBitmap("Event.png");
                    else
                        return VisualHelper.LoadBitmap("Field.png");
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
            get { return _text; }
        }

        #endregion
    }
}
