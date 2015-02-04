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

using SalesForceData;

namespace Wallace.IDE.SalesForce.Document
{
    /// <summary>
    /// Interface for text editors
    /// </summary>
    public interface ISourceFileEditorDocument
    {
        /// <summary>
        /// The text displayed.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Returns false when a text document isn't displaying it's text.
        /// </summary>
        bool IsTextVisible { get; }

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
        /// The file being edited.
        /// </summary>
        SourceFile File { get; }

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

        /// <summary>
        /// Update the editor settings.
        /// </summary>
        void UpdateEditorSettings();
    }
}
