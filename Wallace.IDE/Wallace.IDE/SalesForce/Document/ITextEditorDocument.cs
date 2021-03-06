﻿/*
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

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Interface for text based editor documents.
    /// </summary>
    public interface ITextEditorDocument
    {
        /// <summary>
        /// The text displayed.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        int CurrentLineNumber { get; }

        /// <summary>
        /// Flag that indicates if there are unsaved changes.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Flag that indicates if the document is read only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Save changes made to the content.
        /// </summary>
        void Save();

        /// <summary>
        /// Reload the document.
        /// </summary>
        /// <returns>true if the document was reloaded.</returns>
        bool Reload();

        /// <summary>
        /// Open the text search dialog.
        /// </summary>
        void SearchText();

        /// <summary>
        /// Open the text search dialog with the given text.
        /// </summary>
        /// <param name="text">The text to search with.</param>
        void SearchText(string text);

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
        /// Insert the given text.
        /// </summary>
        /// <param name="text">The text to insert.</param>
        void InsertText(string text);

        /// <summary>
        /// Go to the given line number in the document.
        /// </summary>
        /// <param name="line">The line number to go to. (1 based)</param>
        void GotToLine(int line);

        /// <summary>
        /// Update the editor settings.
        /// </summary>
        void UpdateEditorSettings();
    }
}
