using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesForceLanguage.Apex.CodeModel;

namespace SalesForceLanguage
{
    /// <summary>
    /// Used to do code completions.
    /// </summary>
    public class LanguageCompletion
    {
        #region Fields

        /// <summary>
        /// Used to do code completions.
        /// </summary>
        private LanguageManager _language;

        /// <summary>
        /// Holds generic code completions.
        /// </summary>
        private static List<Symbol> _genericCompletions;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LanguageCompletion()
        {
            _genericCompletions = new List<Symbol>();

            _genericCompletions.Add(new Keyword("abstract"));
            _genericCompletions.Add(new Keyword("after"));
            _genericCompletions.Add(new Keyword("before"));
            _genericCompletions.Add(new Keyword("break"));
            _genericCompletions.Add(new Keyword("catch"));
            _genericCompletions.Add(new Keyword("class"));
            _genericCompletions.Add(new Keyword("continue"));
            _genericCompletions.Add(new Keyword("delete"));
            _genericCompletions.Add(new Keyword("do"));
            _genericCompletions.Add(new Keyword("double"));
            _genericCompletions.Add(new Keyword("else"));
            _genericCompletions.Add(new Keyword("enum"));
            _genericCompletions.Add(new Keyword("extends"));
            _genericCompletions.Add(new Keyword("false"));
            _genericCompletions.Add(new Keyword("final"));
            _genericCompletions.Add(new Keyword("finally"));
            _genericCompletions.Add(new Keyword("for"));
            _genericCompletions.Add(new Keyword("get"));
            _genericCompletions.Add(new Keyword("global"));
            _genericCompletions.Add(new Keyword("if"));
            _genericCompletions.Add(new Keyword("implements"));
            _genericCompletions.Add(new Keyword("insert"));
            _genericCompletions.Add(new Keyword("interface"));
            _genericCompletions.Add(new Keyword("merge"));
            _genericCompletions.Add(new Keyword("new"));
            _genericCompletions.Add(new Keyword("null"));
            _genericCompletions.Add(new Keyword("on"));
            _genericCompletions.Add(new Keyword("override"));
            _genericCompletions.Add(new Keyword("private"));
            _genericCompletions.Add(new Keyword("protected"));
            _genericCompletions.Add(new Keyword("public"));
            _genericCompletions.Add(new Keyword("return"));
            _genericCompletions.Add(new Keyword("rollback"));
            _genericCompletions.Add(new Keyword("set"));
            _genericCompletions.Add(new Keyword("static"));
            _genericCompletions.Add(new Keyword("string"));
            _genericCompletions.Add(new Keyword("super"));
            _genericCompletions.Add(new Keyword("testmethod"));
            _genericCompletions.Add(new Keyword("this"));
            _genericCompletions.Add(new Keyword("throw"));
            _genericCompletions.Add(new Keyword("transient"));
            _genericCompletions.Add(new Keyword("trigger"));
            _genericCompletions.Add(new Keyword("true"));
            _genericCompletions.Add(new Keyword("try"));
            _genericCompletions.Add(new Keyword("undelete"));
            _genericCompletions.Add(new Keyword("update"));
            _genericCompletions.Add(new Keyword("upsert"));
            _genericCompletions.Add(new Keyword("value"));
            _genericCompletions.Add(new Keyword("virtual"));
            _genericCompletions.Add(new Keyword("void"));
            _genericCompletions.Add(new Keyword("webservice"));
            _genericCompletions.Add(new Keyword("while"));
            _genericCompletions.Add(new Keyword("with"));
            _genericCompletions.Add(new Keyword("without"));
            _genericCompletions.Add(new Keyword("sharing"));

            _genericCompletions = _genericCompletions.OrderBy(s => s.Name).ToList();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="language">Language.</param>
        public LanguageCompletion(LanguageManager language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _language = language;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get generic code completions.
        /// </summary>
        /// <param name="text">The text that appears before the position where the completion would be inserted.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Generic code completions.</returns>
        public Symbol[] GetCodeCompletionsLetter(string text, string className, TextPosition position)
        {
            // don't do code completions for text that immediately follows a type or a word
            // that isn't a keyword.
            if (text != null && text.Length > 0)
            {
                if (_language.GetSymbols(text) != null)
                    return new Symbol[0];

                text = text.Trim().ToLower();
                if (!_genericCompletions.Any(s => s.Id == text))
                    return new Symbol[0];
            }

            List<Symbol> result = new List<Symbol>();
            result.AddRange(_genericCompletions);
            result.AddRange(_language.GetAllSymbols());

            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol != null)
            {
                // add variables from the current scope
                foreach (VariableScope scope in classSymbol.VariableScopes)
                {
                    if (scope.Span.Contains(position))
                    {
                        foreach (Field field in scope.Variables)
                            if (field.Location.CompareTo(position) < 0)
                                result.Add(field);
                    }
                }

                // add local members
                result.AddRange(GetFields(classSymbol, false));
                result.AddRange(GetProperties(classSymbol, false));
                result.AddRange(GetMethods(classSymbol, false));
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Parse the text and give possible symbols that can be added to the end of the text.
        /// </summary>
        /// <param name="text">The text to get code completions for.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Valid symbols that can be used for the code completion.</returns>
        public Symbol[] GetCodeCompletionsDot(string text, string className, TextPosition position)
        {
            List<Symbol> result = new List<Symbol>();

            // empty string
            if (String.IsNullOrWhiteSpace(text))
                return result.ToArray();

            text = text.ToLower();

            // filter lines
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == null)
                    continue;

                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("//") || lines[i].StartsWith("/*"))
                    lines[i] = null;
            }

            // create parts
            List<string> parts = new List<string>();
            foreach (string line in lines)
                if (line != null)
                    parts.AddRange(line.Split(new char[] { ' ', '.', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // combine and filter out parts
            for (int i = 0; i < parts.Count; i++)
            {
                if (i + 1 < parts.Count && (parts[i + 1] == "()" || parts[i + 1] == "[]"))
                {
                    parts[i] = parts[i] + parts[i + 1];
                    parts.RemoveAt(i + 1);
                }

                if (parts[i] == "if()")
                {
                    parts.RemoveAt(i);
                    i--;
                }
            }

            // match parts to types
            SymbolTable classSymbol = _language.GetSymbols(className);
            TypedSymbol matchedSymbol = null;

            if (classSymbol != null)
            {
                bool partFound = false;

                for (int i = 0; i < parts.Count; i++)
                {
                    string part = parts[i];
                    partFound = false;

                    // method
                    if (parts[i].EndsWith("()"))
                    {
                        string methodName = part.Substring(0, part.IndexOf('('));
                        SymbolTable externalClass = (i == 0) ? classSymbol : _language.GetSymbols(matchedSymbol.Type);
                        if (externalClass != null)
                        {
                            foreach (Method m in externalClass.Methods)
                            {
                                if (m.Id == methodName)
                                {
                                    if (m.Type != "void")
                                    {
                                        matchedSymbol = m;
                                        partFound = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    // list
                    else if (part.EndsWith("[]"))
                    {
                        //TODO:
                    }
                    // variables
                    else
                    {
                        SymbolTable externalClass = (i == 0) ? classSymbol : _language.GetSymbols(matchedSymbol.Type);

                        if (i == 0)
                        {
                            // this keyword
                            if (part == "this")
                            {
                                matchedSymbol = classSymbol;
                                partFound = true;
                            }
                            else
                            {
                                // look for method parameters
                                foreach (Method method in classSymbol.Methods)
                                {
                                    if (method.Contains(position))
                                    {
                                        foreach (Parameter parameter in method.Parameters)
                                        {
                                            if (parameter.Id == part)
                                            {
                                                matchedSymbol = parameter;
                                                partFound = true;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                // look for local variable
                                if (!partFound)
                                {
                                    foreach (VariableScope scope in classSymbol.VariableScopes)
                                    {
                                        if (scope.Span.Contains(position))
                                        {
                                            foreach (Field variable in scope.Variables)
                                            {
                                                if (variable.Id == part && variable.Location.CompareTo(position) < 0)
                                                {
                                                    matchedSymbol = variable;
                                                    partFound = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (partFound)
                                            break;
                                    }
                                }
                            }
                        }

                        if (externalClass != null)
                        {
                            // look for fields
                            if (!partFound)
                            {
                                foreach (Field field in externalClass.Fields)
                                {
                                    if (field.Id == part)
                                    {
                                        matchedSymbol = field;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }

                            // look for properties
                            if (!partFound)
                            {
                                foreach (Property property in externalClass.Properties)
                                {
                                    if (property.Id == part)
                                    {
                                        matchedSymbol = property;
                                        partFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // check that the part was found
                    if (!partFound)
                    {
                        matchedSymbol = null;
                        break;
                    }
                }

                // build completions from matched symbol
                if (matchedSymbol != null)
                {
                    SymbolTable symbols = _language.GetSymbols(matchedSymbol.Type);
                    if (symbols != null)
                    {
                        bool isExternal = (classSymbol == null || classSymbol.Id != symbols.Id);

                        result.AddRange(GetFields(symbols, isExternal));
                        result.AddRange(GetProperties(symbols, isExternal));
                        result.AddRange(GetMethods(symbols, isExternal));
                    }
                }
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Get the fields for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the fields for.</param>
        /// <param name="isExternal">When true, only fields accessible to external code will be returned.</param>
        /// <returns>The fields for the given symbol table.</returns>
        private List<Field> GetFields(SymbolTable symbolTable, bool isExternal)
        {
            List<Field> result = new List<Field>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetFieldsWithModifiers(SymbolModifier.Public));
                GetInheritedFields(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Fields);
                GetInheritedFields(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the fields for the given table and all fields that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the fields for.</param>
        /// <param name="modifiers">Only get fields that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting fields are added to this list.</param>
        private void GetInheritedFields(SymbolTable symbolTable, SymbolModifier modifiers, List<Field> result)
        {
            if (symbolTable == null)
                return;

            foreach (Field field in symbolTable.Fields)
            {
                if (field.Modifier.HasFlag(modifiers))
                    result.Add(field);
            }

            GetInheritedFields(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Get the properties for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the properties for.</param>
        /// <param name="isExternal">When true, only properties accessible to external code will be returned.</param>
        /// <returns>The properties for the given symbol table.</returns>
        private List<Property> GetProperties(SymbolTable symbolTable, bool isExternal)
        {
            List<Property> result = new List<Property>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetPropertiesWithModifiers(SymbolModifier.Public));
                GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Properties);
                GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the properties for the given table and all properties that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the properties for.</param>
        /// <param name="modifiers">Only get properties that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting properties are added to this list.</param>
        private void GetInheritedProperties(SymbolTable symbolTable, SymbolModifier modifiers, List<Property> result)
        {
            if (symbolTable == null)
                return;

            foreach (Property property in symbolTable.Properties)
            {
                if (property.Modifier.HasFlag(modifiers))
                    result.Add(property);
            }

            GetInheritedProperties(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Get the methods for the given symbol table.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="isExternal">When true, only methods accessible to external code will be returned.</param>
        /// <returns>The methods for the given symbol table.</returns>
        private List<Method> GetMethods(SymbolTable symbolTable, bool isExternal)
        {
            List<Method> result = new List<Method>();
            if (isExternal)
            {
                result.AddRange(symbolTable.GetMethodsWithModifiers(SymbolModifier.Public));
                GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public, result);
            }
            else
            {
                result.AddRange(symbolTable.Methods);
                GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), SymbolModifier.Public | SymbolModifier.Protected, result);
            }

            return result.GroupBy(s => s.Name).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Recursive call that gets the methods for the given table and all methods that are inherited.
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="modifiers">Only get methods that have the at least one of these modifiers.</param>
        /// <param name="result">The resulting methods are added to this list.</param>
        private void GetInheritedMethods(SymbolTable symbolTable, SymbolModifier modifiers, List<Method> result)
        {
            if (symbolTable == null)
                return;

            foreach (Method method in symbolTable.Methods)
            {
                if (method.Modifier.HasFlag(modifiers))
                    result.Add(method);
            }

            GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        #endregion
    }
}
