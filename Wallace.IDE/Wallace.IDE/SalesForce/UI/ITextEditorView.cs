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

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interface for text editor views.
    /// </summary>
    public interface ITextEditorView
    {
        /// <summary>
        /// The text in the view.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Set errors that are displayed.
        /// </summary>
        /// <param name="errors">The errors to display.  Null or an empty collection clears all errors.</param>
        void SetErrors(IEnumerable<string> errors);

        /// <summary>
        /// Raised when the text has been changed.
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// Give focus to the text input.
        /// </summary>
        void FocusText();

        /// <summary>
        /// Open the text search dialog.
        /// </summary>
        void SearchText();

        /// <summary>
        /// Copy selected text to the clipboard.
        /// </summary>
        void CopyText();

        /// <summary>
        /// Delete selected text and copy it to the clipboard.
        /// </summary>
        void CutText();

        /// <summary>
        /// Paste text from the clipboard to the editor.
        /// </summary>
        void PasteText();

        /// <summary>
        /// Undo the last test change.
        /// </summary>
        void UndoText();

        /// <summary>
        /// Redo the last text change.
        /// </summary>
        void RedoText();

        /// <summary>
        /// Select all text.
        /// </summary>
        void SelectAllText();

        /// <summary>
        /// Go to the given line number in the document.
        /// </summary>
        /// <param name="line">The line number to go to. (1 based)</param>
        void GotToLine(int line);
    }
}
