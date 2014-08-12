using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// A scoped collection of variables.
    /// </summary>
    public class VariableScope
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="span">Span.</param>
        /// <param name="variables">Variables.</param>
        public VariableScope(TextSpan span, Field[] variables)
        {
            Span = span;
            Variables = variables ?? new Field[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The span that makes up the scope of the variables.
        /// </summary>
        public TextSpan Span { get; private set; }

        /// <summary>
        /// The variables within the scope.
        /// </summary>
        public Field[] Variables { get; private set; }

        #endregion
    }
}
