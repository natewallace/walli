// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

namespace QUT.Gppg
{
    /// <summary>
    /// Stack utility for the shift-reduce parser.
    /// GPPG parsers have three instances:
    /// (1) The parser state stack, T = QUT.Gppg.State,
    /// (2) The semantic value stack, T = TValue,
    /// (3) The location stack, T = TSpan.
    /// </summary>
    /// <typeparam name="T">The type of objects held in the stack.</typeparam>
    public class PushdownPrefixState<T>
    {
        #region Fields

        /// <summary>
        /// Note that we cannot use the BCL Stack&lt;T&gt; class
        /// here as derived types need to index into stacks.
        /// </summary>
        private T[] array = new T[8];

        /// <summary>
        /// top of stack.
        /// </summary>
        private int tos = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Indexer for values of the stack below the top.
        /// </summary>
        /// <param name="index">index of the element, starting from the bottom</param>
        /// <returns>the selected element</returns>
        public T this[int index] { get { return array[index]; } }

        /// <summary>
        /// The current depth of the stack.
        /// </summary>
        public int Depth 
        { 
            get { return tos; } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add item to the stack.
        /// </summary>
        /// <param name="value">The item to add.</param>
        internal void Push(T value)
        {
            if (tos >= array.Length)
            {
                T[] newarray = new T[array.Length * 2];
                System.Array.Copy(array, newarray, tos);
                array = newarray;
            }
            array[tos++] = value;
        }

        /// <summary>
        /// Remove top item from the stack.
        /// </summary>
        /// <returns>The item that was removed.</returns>
        internal T Pop()
        {
            T rslt = array[--tos];
            array[tos] = default(T);

            return rslt;
        }

        /// <summary>
        /// The item on top of the stack.
        /// </summary>
        /// <returns>The item on top of the stack.</returns>
        internal T TopElement()
        {
            return array[tos - 1];
        }

        /// <summary>
        /// Check to see if there are any items on the stack.
        /// </summary>
        /// <returns>true if there are no items on the stack, false otherwise.</returns>
        internal bool IsEmpty()
        {
            return tos == 0;
        }

        #endregion
    }
}
