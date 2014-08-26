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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Implements IDocument using default logic.
    /// </summary>
    public abstract class DocumentBase : IDocument
    {
        #region Fields

        /// <summary>
        /// Supports Id property.
        /// </summary>
        private string _id = Guid.NewGuid().ToString();

        #endregion

        #region Properties

        /// <summary>
        /// Holds the presenter used to display this document.
        /// </summary>
        protected IDocumentPresenter Presenter { get; set; }

        #endregion

        #region IDocument Members

        /// <summary>
        /// Id that uniquely identifies this document.
        /// </summary>
        public virtual string Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// The text used to display this document.
        /// </summary>
        public virtual string Text { protected set; get; }

        /// <summary>
        /// Called by the UI when the document is ready for display.
        /// </summary>
        /// <param name="presenter">The presenter used to display the document.</param>
        public virtual void Init(IDocumentPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException("presenter");

            Presenter = presenter;
        }

        /// <summary>
        /// Called by the UI when the document is about to be closed.
        /// </summary>
        /// <returns>true if it's ok to close the document, false if the document can't be closed.</returns>
        public virtual bool Closing()
        {
            return true;
        }

        /// <summary>
        /// Called by the UI when the document is closed.
        /// </summary>
        public virtual void Closed()
        {
        }

        /// <summary>
        /// Called when the UI has opened this document and is displayed for the first time.
        /// </summary>        
        public virtual void Opened()
        {
        }

        /// <summary>
        /// Called when the UI has made this document the current document.
        /// </summary>
        public virtual void Activated()
        {
        }

        /// <summary>
        /// Called when the document should update its display.
        /// </summary>
        /// <param name="isFirstUpdate">This is true when it's the first time this method has been called.</param>
        public virtual void Update(bool isFirstUpdate)
        {
        }

        /// <summary>
        /// If this document represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this document represents the given entity.</returns>
        public virtual bool RepresentsEntity(object entity)
        {
            return false;
        }

        #endregion
    }
}
