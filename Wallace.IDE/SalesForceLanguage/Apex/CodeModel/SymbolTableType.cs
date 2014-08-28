using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// The different types of symbol tables.
    /// </summary>
    public enum SymbolTableType
    {
        /// <summary>
        /// A class definition.
        /// </summary>
        Class,

        /// <summary>
        /// An interface definition.
        /// </summary>
        Interface,

        /// <summary>
        /// An enum definition.
        /// </summary>
        Enum,

        /// <summary>
        /// An SObject definition.
        /// </summary>
        SObject,

        /// <summary>
        /// A namespace.
        /// </summary>
        Namespace
    }
}
