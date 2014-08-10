using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A property.
    /// </summary>
    public class Property : TypedSymbol
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        /// <param name="span">Span.</param>
        /// <param name="visibility">Visibility.</param>
        /// <param name="type">Type.</param>
        public Property(
            TextPosition location, 
            string name, 
            TextSpan span,
            SymbolVisibility visibility, 
            string type)
            : base(location, name, span, visibility, type)
        {
        }
    }
}
