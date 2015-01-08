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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Interface to a document manager.
    /// </summary>
    public interface IDocumentManager
    {
        /// <summary>
        /// Open a document.
        /// </summary>
        /// <param name="document">The document to open.</param>
        void OpenDocument(IDocument document);

        /// <summary>
        /// Close a document.
        /// </summary>
        /// <param name="document">The document to close.</param>
        /// <returns>true if the document was closed, false if it wasn't.</returns>
        bool CloseDocument(IDocument document);

        /// <summary>
        /// All of the currently open documents.
        /// </summary>
        IDocument[] OpenDocuments { get; }

        /// <summary>
        /// The document that is currently being displayed or null if there isn't one.
        /// </summary>
        IDocument ActiveDocument { get; set; }

        /// <summary>
        /// Close all open documents.
        /// </summary>
        /// <returns>true all documents were closed, false if they weren't.</returns>
        bool CloseAllDocuments();

        /// <summary>
        /// Get all documents that represent the given entity.
        /// </summary>
        /// <param name="entity">The entity to get documents for.</param>
        /// <returns>All documents that represent the given entity.</returns>
        IDocument[] GetDocumentsByEntity(object entity);

        /// <summary>
        /// Raised when the active document is changed.
        /// </summary>
        event EventHandler ActiveDocumentChanged;
    }
}
