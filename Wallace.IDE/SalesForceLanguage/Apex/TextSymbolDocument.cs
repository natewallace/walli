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

using System.Collections.Generic;

namespace SalesForceLanguage.Apex
{
    /// <summary>
    /// A document of symbols.
    /// </summary>
    public class TextSymbolDocument : IEnumerable<IList<TextSymbol>>
    {
        #region Fields

        /// <summary>
        /// Holds the list of symbols.
        /// </summary>
        private IList<IList<TextSymbol>> _list;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextSymbolDocument()
        {
            _list = new List<IList<TextSymbol>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the lines(if needed) up to the given index.
        /// </summary>
        /// <param name="index">The index to stop initializing new lines on. 0 based.</param>
        private void InitializeLines(int index)
        {
            if (_list.Count < index + 1)
                for (int i = _list.Count; i <= index; i++)
                    _list.Add(new List<TextSymbol>(10));
        }

        /// <summary>
        /// Adds the symbol to the document.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        public void Add(TextSymbol symbol)
        {
            int index = symbol.Location.StartLine - 1;
            InitializeLines(index);
            IList<TextSymbol> line = _list[index];

            int insertionIndex = 0;
            bool replace = false;
            for (; insertionIndex < line.Count; insertionIndex++)
            {
                int compareResult = symbol.Location.CompareTo(line[insertionIndex].Location);
                if (compareResult == 0)
                {
                    replace = true;
                    break;
                }
                else if (compareResult < 0)
                {
                    break;
                }
            }

            if (replace)
                line[insertionIndex] = symbol;
            else
                line.Insert(insertionIndex, symbol);
        }

        /// <summary>
        /// Add a range of symbols.
        /// </summary>
        /// <param name="symbols">The symbols to add.</param>
        public void AddRange(IEnumerable<TextSymbol> symbols)
        {
            foreach (TextSymbol symbol in symbols)
                Add(symbol);
        }

        /// <summary>
        /// Get a line of symbols from the document.
        /// </summary>
        /// <param name="index">Index of the line to get.  1 based.</param>
        /// <returns>The requested line.</returns>
        public IList<TextSymbol> this[int index]
        {
            get
            {
                return _list[index - 1];
            }
        }

        /// <summary>
        /// The number of lines in this document.
        /// </summary>
        public int LineCount
        {
            get
            {
                return _list.Count;
            }
        }

        #endregion

        #region IEnumerable<IList<TextSymbol>> Members

        /// <summary>
        /// Get an enumerator for this document.
        /// </summary>
        /// <returns>An enumerator for this document.</returns>
        public IEnumerator<IList<TextSymbol>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Get an enumerator for this document.
        /// </summary>
        /// <returns>An enumerator for this document.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
