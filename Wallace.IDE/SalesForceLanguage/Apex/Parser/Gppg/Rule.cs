// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

namespace QUT.Gppg
{
    /// <summary>
    /// Rule representation at runtime.
    /// </summary>
    public class Rule
    {
        #region Fields

        /// <summary>
        /// Symbol.
        /// </summary>
        internal int LeftHandSide;

        /// <summary>
        /// Symbols.
        /// </summary>
        internal int[] RightHandSide;

        #endregion

        #region Constructors

        /// <summary>
        /// Rule constructor.  This holds the ordinal of
        /// the left hand side symbol, and the list of
        /// right hand side symbols, in lexical order.
        /// </summary>
        /// <param name="left">The LHS non-terminal</param>
        /// <param name="right">The RHS symbols, in lexical order</param>
        public Rule(int left, int[] right)
        {
            this.LeftHandSide = left;
            this.RightHandSide = right;
        }

        #endregion
    }
}
