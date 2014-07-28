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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// A document that can be opened in the UI.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Id that uniquely identifies this document.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Called by the UI when the document is ready for display.
        /// </summary>
        /// <param name="presenter">The presenter used to display the document.</param>
        void Init(IDocumentPresenter presenter);

        /// <summary>
        /// Called by the UI when the document is about to be closed.
        /// </summary>
        /// <returns>true if it's ok to close the document, false if the document can't be closed.</returns>
        bool Closing();

        /// <summary>
        /// Called by the UI when the document is closed.
        /// </summary>
        void Closed();

        /// <summary>
        /// Called when the UI has opened this document and is displayed for the first time.
        /// </summary>        
        void Opened();

        /// <summary>
        /// Called when the UI has made this document the current document.
        /// </summary>
        void Activated();

        /// <summary>
        /// Called when the document should update its display.
        /// </summary>
        /// <param name="isFirstUpdate">This is true when it's the first time this method has been called.</param>
        void Update(bool isFirstUpdate);

        /// <summary>
        /// If this document represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this document represents the given entity.</returns>
        bool RepresentsEntity(object entity);
    }
}
