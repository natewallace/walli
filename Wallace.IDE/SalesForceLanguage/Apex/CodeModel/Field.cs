using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A field.
    /// </summary>
    public class Field : TypedSymbol
    {
        #region Constructors

        /// <summary>
        /// Used by xml serializer.
        /// </summary>
        public Field()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="visibility">Visibility.</param>
        /// <param name="type">Type.</param>
        public Field(
            TextPosition location, 
            string name, 
            TextSpan span,
            SymbolVisibility visibility, 
            string type)
            : base(location, name, span, visibility, type)
        {
        }

        #endregion
    }
}
