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

using ICSharpCode.AvalonEdit.Document;
using System;
using System.IO;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// A stream that reads characters from a document.
    /// </summary>
    public class DocumentCharStream : Stream
    {
        #region Fields

        /// <summary>
        /// The document being read from.
        /// </summary>
        private TextDocument _document;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="document">The document to read from.</param>
        public DocumentCharStream(TextDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="document">The document to read from.</param>
        /// <param name="position">The starting position for the stream.</param>
        public DocumentCharStream(TextDocument document, int position)
            : this(document)
        {
            Position = position;
        }

        #endregion

        #region Stream Members

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Doesn't do anything.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// The length of the document.
        /// </summary>
        public override long Length
        {
            get { return _document.TextLength; }
        }

        /// <summary>
        /// The current position in the stream.
        /// </summary>
        public override long Position { get; set; }

        /// <summary>
        /// Read the data into the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to write to.</param>
        /// <returns>The number of bytes read in.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (Position >= Length)
                    return i;

                buffer[offset + i] = (byte)_document.GetCharAt((int)Position);
                Position++;
            }

            return count;
        }

        /// <summary>
        /// Seek to the given position.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The position to seek from.</param>
        /// <returns>The new position.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.End:
                    Position = Length - offset;
                    break;

                default:
                    Position = Position + offset;
                    break;
            }

            return Position;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">Not used.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer">Not used.</param>
        /// <param name="offset">Not used.</param>
        /// <param name="count">Not used.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
