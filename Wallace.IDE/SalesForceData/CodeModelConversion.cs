using SalesForceLanguage;
using SalesForceLanguage.Apex.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// This class is used to convert between the local and the remote code model.
    /// </summary>
    internal static class CodeModelConversion
    {
        #region Methods

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="symbolTable">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static SymbolTable Convert(SalesForceAPI.Tooling.SymbolTable symbolTable)
        {
            if (symbolTable == null || symbolTable.tableDeclaration == null)
                return null;

            return new SymbolTable(
                Convert(symbolTable.tableDeclaration.location),
                symbolTable.tableDeclaration.name,
                null,
                Convert(symbolTable.constructors),
                Convert(symbolTable.properties),
                Convert(symbolTable.methods),
                symbolTable.interfaces,
                Convert(symbolTable.innerClasses));
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="symbolTables">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static SymbolTable[] Convert(IEnumerable<SalesForceAPI.Tooling.SymbolTable> symbolTables)
        {
            if (symbolTables == null)
                return new SymbolTable[0];

            List<SymbolTable> result = new List<SymbolTable>();
            foreach (SalesForceAPI.Tooling.SymbolTable symbolTable in symbolTables)
                result.Add(Convert(symbolTable));

            return result.ToArray();
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="method">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Method Convert(SalesForceAPI.Tooling.Method method)
        {
            if (method == null)
                return null;

            return new Method(
                Convert(method.location),
                method.name,
                null,
                Convert(method.visibility),
                method.returnType,
                Convert(method.parameters));
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="methods">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Method[] Convert(IEnumerable<SalesForceAPI.Tooling.Method> methods)
        {
            if (methods == null)
                return new Method[0];

            List<Method> result = new List<Method>();
            foreach (SalesForceAPI.Tooling.Method method in methods)
                result.Add(Convert(method));

            return result.ToArray();
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="constructor">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Constructor Convert(SalesForceAPI.Tooling.Constructor constructor)
        {
            if (constructor == null)
                return null;

            return new Constructor(
                Convert(constructor.location),
                constructor.name,
                null,
                Convert(constructor.visibility),
                Convert(constructor.parameters));
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="constructors">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Constructor[] Convert(IEnumerable<SalesForceAPI.Tooling.Constructor> constructors)
        {
            if (constructors == null)
                return new Constructor[0];

            List<Constructor> result = new List<Constructor>();
            foreach (SalesForceAPI.Tooling.Constructor constructor in constructors)
                result.Add(Convert(constructor));

            return result.ToArray();
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="parameter">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Parameter Convert(SalesForceAPI.Tooling.Parameter parameter)
        {
            if (parameter == null)
                return null;

            return new Parameter(
                parameter.type,
                parameter.name);
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="parameters">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Parameter[] Convert(IEnumerable<SalesForceAPI.Tooling.Parameter> parameters)
        {
            if (parameters == null)
                return new Parameter[0];

            List<Parameter> result = new List<Parameter>();
            foreach (SalesForceAPI.Tooling.Parameter p in parameters)
                result.Add(Convert(p));

            return result.ToArray();
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="symbol">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Property Convert(SalesForceAPI.Tooling.VisibilitySymbol symbol)
        {
            if (symbol == null)
                return null;

            return new Property(
                Convert(symbol.location),
                symbol.name,
                null,
                Convert(symbol.visibility),
                symbol.type);
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="symbols">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static Property[] Convert(IEnumerable<SalesForceAPI.Tooling.VisibilitySymbol> symbols)
        {
            if (symbols == null)
                return new Property[0];

            List<Property> result = new List<Property>();
            foreach (SalesForceAPI.Tooling.VisibilitySymbol symbol in symbols)
                result.Add(Convert(symbol));

            return result.ToArray();
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="visibility">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static SymbolVisibility Convert(SalesForceAPI.Tooling.SymbolVisibility visibility)
        {
            switch (visibility)
            {
                case SalesForceAPI.Tooling.SymbolVisibility.Global:
                    return SymbolVisibility.Global;

                case SalesForceAPI.Tooling.SymbolVisibility.Public:
                    return SymbolVisibility.Public;

                default:
                    return SymbolVisibility.Private;
            }
        }

        /// <summary>
        /// Convert between the two object types.
        /// </summary>
        /// <param name="position">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static TextPosition Convert(SalesForceAPI.Tooling.Position position)
        {
            if (position == null)
                return new TextPosition();

            return new TextPosition(position.line, position.column);
        }

        #endregion
    }
}
