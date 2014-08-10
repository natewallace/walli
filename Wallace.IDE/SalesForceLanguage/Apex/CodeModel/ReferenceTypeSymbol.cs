using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A reference type symbol.
    /// </summary>
    public class ReferenceTypeSymbol : Symbol
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="parts">Parts.</param>
        public ReferenceTypeSymbol(TextPosition location, string name, TextSpan span, TextSpan[] parts)
            : base(location, name, span)
        {
            Parts = parts ?? new TextSpan[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The individual parts of the type which should be highlighted.
        /// </summary>
        public TextSpan[] Parts { get; private set; }

        #endregion
    }
}
