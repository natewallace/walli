using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A symbol that has a type.
    /// </summary>
    public class TypedSymbol : VisibilitySymbol
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="name"></param>
        /// <param name="span"></param>
        /// <param name="visibility"></param>
        /// <param name="type"></param>
        public TypedSymbol(TextPosition location, string name, TextSpan span, SymbolVisibility visibility, string type)
            : base(location, name, span, visibility)
        {
            Type = type ?? String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type for the symbol.
        /// </summary>
        public string Type { get; private set; }

        #endregion
    }
}
