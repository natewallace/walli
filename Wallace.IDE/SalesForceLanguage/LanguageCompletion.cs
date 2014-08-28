/*
 * Copyright (c) 2014 Nathaniel Wallace
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using SalesForceLanguage.Apex.CodeModel;
using SalesForceLanguage.Apex.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Tokens for which code completions should be ignored.
        /// </summary>
        private static Tokens[] _tokensToIgnore = new Tokens[] 
        {
            Tokens.COMMENT_BLOCK,
            Tokens.COMMENT_DOC,
            Tokens.COMMENT_DOCUMENTATION,
            Tokens.COMMENT_INLINE,
            Tokens.SOQL,
            Tokens.SOSL,
            Tokens.LITERAL_STRING
        };

        /// <summary>
        /// Holds all of the visual force symbols.
        /// </summary>
        private static Dictionary<string, string[]> _visualForceSymbols;

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
            _genericCompletions.Add(new Keyword("with sharing"));
            _genericCompletions.Add(new Keyword("without sharing"));

            _genericCompletions = _genericCompletions.OrderBy(s => s.Name).ToList();

            _visualForceSymbols = new Dictionary<string, string[]>();
            _visualForceSymbols.Add("analytics:reportChart", new string[] { "cacheAge", "cacheResults", "developerName", "filter", "hideOnError", "rendered", "reportId", "showRefreshButton", "size" });
            _visualForceSymbols.Add("apex:actionFunction", new string[] { "action", "focus", "id", "immediate", "name", "namespace", "onbeforedomupdate", "oncomplete", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:actionPoller", new string[] { "action", "enabled", "id", "interval", "oncomplete", "onsubmit", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:actionRegion", new string[] { "id", "immediate", "rendered", "renderRegionOnly" });
            _visualForceSymbols.Add("apex:actionStatus", new string[] { "dir", "for", "id", "lang", "layout", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onstart", "onstop", "rendered", "startStyle", "startStyleClass", "startText", "stopStyle", "stopStyleClass", "stopText", "style", "styleClass", "title" });
            _visualForceSymbols.Add("apex:actionSupport", new string[] { "action", "disabled", "diableDefault", "event", "focus", "id", "immediate", "onbeforedomupdate", "oncomplete", "onsubmit", "rendered", "reRender", "status", "timeout" });
            _visualForceSymbols.Add("apex:areaSeries", new string[] { "axis", "colorSet", "highlight", "highlightLineWidth", "highlightOpacity", "highlightStrokeColor", "id", "opacity", "rendered", "rendererFn", "showInLegend", "tips", "title", "xField", "yField" });
            _visualForceSymbols.Add("apex:attribute", new string[] { "access", "assignTo", "default", "description", "encode", "id", "name", "required", "type" });
            _visualForceSymbols.Add("apex:axis", new string[] { "dashSize", "fields", "grid", "gridFill", "id", "margin", "maximum", "minimum", "position", "rendered", "steps", "title", "type" });
            /*_visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });
            _visualForceSymbols.Add("", new string[] { });*/
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
        /// Check to see if the given position is within a comment.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="text">The text to check.</param>
        /// <returns>true if the position is within a comment.</returns>
        private bool IsInToken(TextPosition position, Stream text, Tokens[] tokens)
        {
            long currentPosition = text.Position;

            bool result = false;
            text.Position = 0;
            ApexLexer lexer = new ApexLexer(text, true);
            while (lexer.yylex() > 2)
            {
                if (lexer.yylval != null)
                {
                    if (lexer.yylval.TextSpan.Contains(position) ||
                        lexer.yylval.TextSpan.StartLine > position.Line)
                        break;
                }
            }

            text.Position = currentPosition;

            if (lexer.yylval != null &&
                lexer.yylval.TextSpan.Contains(position) &&
                tokens.Contains(lexer.yylval.Token))
                return true;

            return result;
        }

        /// <summary>
        /// Break the given line in the given text down to distinct parts.
        /// </summary>
        /// <param name="text">The text to get parts for.</param>
        /// <returns>The parts to process.</returns>
        private string[] GetLineParts(Stream text)
        {
            string line = null;

            if (text.Position > 0)
            {
                // get line to calculate completions for
                StringBuilder lineBuilder = new StringBuilder();
                int openDelimiterCount = 0;
                bool stop = false;

                while (text.Position > 0)
                {
                    text.Position = text.Position - 1;
                    char c = (char)text.ReadByte();

                    switch (c)
                    {
                        case ')':
                        case ']':
                        case '>':
                            if (openDelimiterCount == 0)
                                lineBuilder.Insert(0, c);
                            openDelimiterCount++;
                            break;

                        case '{':
                        case '}':
                        case '(':
                        case '[':
                        case '<':
                            if (openDelimiterCount == 0)
                            {
                                stop = true;
                            }
                            else
                            {
                                openDelimiterCount--;
                                if (openDelimiterCount == 0)
                                    lineBuilder.Insert(0, c);
                            }
                            break;

                        case ';':
                        case ',':
                        case '=':
                        case '-':
                        case '+':
                        case '*':
                        case '/':
                        case ':':
                        case '&':
                        case '|':
                        case '!':
                        case '^':
                        case '?':
                            if (openDelimiterCount == 0)
                                stop = true;
                            break;

                        default:
                            if (openDelimiterCount == 0)
                                lineBuilder.Insert(0, c);
                            break;
                    }

                    if (stop)
                        break;

                    text.Position = text.Position - 1;
                }

                line = lineBuilder.ToString().ToLower();
            }

            if (String.IsNullOrWhiteSpace(line))
                return new string[0];

            // filter lines
            string[] lines = line.Split('\n');
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
            foreach (string l in lines)
                if (l != null)
                    parts.AddRange(l.Split(new char[] { ' ', '.', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // combine and filter out parts
            for (int i = 0; i < parts.Count; i++)
            {
                if (i + 1 < parts.Count)
                {
                    if (parts[i + 1] == "()")
                    {
                        parts[i] = parts[i] + parts[i + 1];
                    }
                    else if (parts[i + 1] == "[]")
                    {
                        parts[i] = "list";
                    }
                }

                if (parts[i] == "if()" ||
                    parts[i] == "()" ||
                    parts[i] == "[]" ||
                    parts[i] == "<>")
                {
                    parts.RemoveAt(i);
                    i--;
                }
            }

            return parts.ToArray();
        }

        /// <summary>
        /// Get the symbols leading up to the current text position.
        /// </summary>
        /// <param name="text">The text to get the symbols from.</param>
        /// <param name="className">The name of the class the text is in.</param>
        /// <param name="position">The position in the class text to start from.</param>
        /// <param name="includeIncompleteMethods">If true, incomplete methods will be included in matches.</param>
        /// <returns>The symbols that matched or null if a match can't be made.</returns>
        private Symbol[] MatchSymbols(Stream text, string className, TextPosition position, bool includeIncompleteMethods)
        {
            List<Symbol> result = new List<Symbol>();

            // get parts
            string[] parts = GetLineParts(text);

            // match parts to types
            SymbolTable classSymbol = _language.GetSymbols(className);
            TypedSymbol matchedSymbol = null;
            bool partFound = false;

            if (classSymbol != null)
            {
                bool typeSearchDone = false;
                bool methodSearchDone = false;

                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    partFound = false;

                    // method
                    if (parts[i].EndsWith("()"))
                    {
                        string methodName = part.Substring(0, part.IndexOf('('));
                        SymbolTable externalClass = (i == 0) ? classSymbol : _language.GetSymbols(matchedSymbol.Type);
                        while (externalClass != null && !partFound)
                        {
                            foreach (Method m in externalClass.Methods)
                            {
                                if (m.Id == methodName)
                                {
                                    if (m.Type != "void" || i == parts.Length - 1)
                                    {
                                        matchedSymbol = m;
                                        partFound = true;
                                    }
                                    break;
                                }
                            }

                            if (!partFound)
                                externalClass = _language.GetSymbols(externalClass.Extends);
                        }
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
                                matchedSymbol = new Field(new TextPosition(0, 0), "this", null, SymbolModifier.None, classSymbol.Type);
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

                    // check for inner class
                    if (!partFound && matchedSymbol != null)
                    {
                        SymbolTable symbolType = _language.GetSymbols(matchedSymbol.Type);
                        if (symbolType != null)
                        {
                            foreach (SymbolTable innerClass in symbolType.InnerClasses)
                            {
                                if (part == innerClass.Id)
                                {
                                    matchedSymbol = innerClass;
                                    partFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    // check for type reference
                    if (!partFound && !typeSearchDone && result.Count == 0)
                    {
                        TypedSymbol tempSymbol = matchedSymbol;
                        int tempIndex = i;

                        StringBuilder typeNameBuilder = new StringBuilder();
                        for (i = 0; i < parts.Length; i++)
                        {
                            if (i == 0)
                                typeNameBuilder.Append(parts[i]);
                            else
                                typeNameBuilder.AppendFormat(".{0}", parts[i]);

                            matchedSymbol = _language.GetSymbols(typeNameBuilder.ToString());
                            if (matchedSymbol != null)
                            {
                                partFound = true;
                                break;
                            }
                        }

                        if (!partFound)
                        {
                            matchedSymbol = tempSymbol;
                            i = tempIndex;
                        }

                        typeSearchDone = true;
                    }

                    // check for incomplete method
                    if (!partFound &&
                        includeIncompleteMethods &&
                        !methodSearchDone &&
                        i == parts.Length - 1)
                    {
                        parts[i] = String.Format("{0}()", parts[i]);
                        i--;
                        methodSearchDone = true;
                    }

                    // collect the matched symbol
                    if (partFound && matchedSymbol != null)
                        result.Add(matchedSymbol);
                    else if (matchedSymbol == null)
                        return null;
                }
            }

            if (result.Count == 0 || !partFound)
                return null;
            else
                return result.ToArray();
        }

        /// <summary>
        /// Get generic code completions.
        /// </summary>
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Generic code completions.</returns>
        public Symbol[] GetCodeCompletionsLetter(Stream text, string className, TextPosition position)
        {
            if (IsInToken(position, text, _tokensToIgnore))
                return new Symbol[0];

            string word = null;
            bool isOpenBracket = false;

            if (text.Position > 0)
            {
                // get the char directly before the insertion point
                text.Position = text.Position - 1;
                char prevChar = (char)text.ReadByte();
                if (Char.IsLetterOrDigit(prevChar) || prevChar == '_' || prevChar == '\'' || prevChar == '.')
                    return new Symbol[0];

                // get the word directly before the insertion point
                string[] parts = GetLineParts(text);
                StringBuilder wordBuilder = new StringBuilder();
                foreach (string part in parts)
                    wordBuilder.AppendFormat("{0}.", part);
                if (wordBuilder.Length > 0)
                    wordBuilder.Length--;
                word = wordBuilder.ToString();
            }

            // don't do code completions for text that:
            // immediately follows a type,
            // immediately follows certain keywords.
            if (!String.IsNullOrWhiteSpace(word))
            {
                if (_language.GetSymbols(word) != null)
                    return new Symbol[0];

                switch (word)
                {
                    case "abstract":
                    case "delete":
                    case "extends":
                    case "final":
                    case "global":
                    case "implements":
                    case "insert":
                    case "merge":
                    case "new":
                    case "override":
                    case "private":
                    case "protected":
                    case "public":
                    case "return":
                    case "static":
                    case "testmethod":
                    case "throw":
                    case "transient":
                    case "undelete":
                    case "update":
                    case "upsert":
                    case "virtual":
                    case "webservice":
                    case "with sharing":
                    case "without sharing":
                        break;

                    default:
                        if (_genericCompletions.Any(s => s.Id == word))
                            return new Symbol[0];
                        break;
                }
            }

            // if we get to this point then return symbols for completions
            List<Symbol> result = new List<Symbol>();
            result.AddRange(_genericCompletions);
            result.AddRange(_language.GetAllSymbols());

            // if the first non-whitespace char before the insertion point is a bracket add the SELECT and FIND keywords
            if (isOpenBracket)
            {
                result.Add(new Keyword("SELECT"));
                result.Add(new Keyword("FIND"));
            }

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
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the code completion.</param>
        /// <returns>Valid symbols that can be used for the code completion.</returns>
        public Symbol[] GetCodeCompletionsDot(Stream text, string className, TextPosition position)
        {
            if (IsInToken(position, text, _tokensToIgnore))
                return new Symbol[0];

            List<Symbol> result = new List<Symbol>();

            // get class definition
            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol == null)
                return result.ToArray();

            // get symbol matches
            Symbol[] matchedSymbols = MatchSymbols(text, className, position, false);
            if (matchedSymbols == null || matchedSymbols.Length == 0)
                return result.ToArray();

            // get the symbol definitions
            Symbol symbol = matchedSymbols[matchedSymbols.Length - 1];
            SymbolTable typeSymbol = null;
            bool isTypeReference = false;
            if (symbol is SymbolTable)
            {
                isTypeReference = true;
                typeSymbol = symbol as SymbolTable;
            }
            else
            {
                isTypeReference = false;
                if (symbol is TypedSymbol)
                    typeSymbol = _language.GetSymbols((symbol as TypedSymbol).Type);
            }

            if (typeSymbol == null)
                return result.ToArray();

            bool isExternal = (classSymbol.Id != symbol.Id);

            // build the results
            result.AddRange(GetFields(typeSymbol, isExternal));
            result.AddRange(GetProperties(typeSymbol, isExternal));
            result.AddRange(GetMethods(typeSymbol, isExternal));

            // filter out values
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] is ModifiedSymbol)
                {
                    SymbolModifier modifier = (result[i] as ModifiedSymbol).Modifier;
                    if ((modifier.HasFlag(SymbolModifier.Static) && !isTypeReference) ||
                        (!modifier.HasFlag(SymbolModifier.Static) && isTypeReference))
                    {
                        result.RemoveAt(i);
                        i--;
                    }
                }
            }

            // add inner classes
            if (isTypeReference)
            {
                foreach (SymbolTable innerClass in typeSymbol.InnerClasses)
                    if (innerClass.Modifier.HasFlag(SymbolModifier.Public))
                        result.Add(innerClass);
            }

            return result.OrderBy(s => s.Name).ToArray();
        }

        /// <summary>
        /// Get the methods that match for the current text.
        /// </summary>
        /// <param name="text">The text stream to process.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="position">The position in the class text for the method completions.</param>
        /// <returns>The methods that match for the current position.</returns>
        public Method[] GetMethodCompletions(Stream text, string className, TextPosition position)
        {
            List<Method> result = new List<Method>();

            // get class definition
            SymbolTable classSymbol = _language.GetSymbols(className);
            if (classSymbol == null)
                return result.ToArray();

            // get symbol matches
            Symbol[] matchedSymbols = MatchSymbols(text, className, position, true);
            if (matchedSymbols == null || matchedSymbols.Length == 0)
                return result.ToArray();

            // get the matched method
            Method method = matchedSymbols[matchedSymbols.Length - 1] as Method;
            if (method == null)
                return result.ToArray();

            // get the parent
            SymbolTable parent = null;
            bool isTypeReference = false;
            if (matchedSymbols.Length == 1)
            {
                parent = classSymbol;
                isTypeReference = true;
            }
            else
            {
                if (matchedSymbols[matchedSymbols.Length - 2] is TypedSymbol)
                    parent = _language.GetSymbols((matchedSymbols[matchedSymbols.Length - 2] as TypedSymbol).Type);
                isTypeReference = (matchedSymbols[matchedSymbols.Length - 2] is SymbolTable);
            }

            if (parent == null)
                return result.ToArray();

            bool isExternal = (classSymbol.Id != parent.Id);

            // build results
            HashSet<string> signatures = new HashSet<string>();
            SymbolTable currentClass = parent;
            while (currentClass != null)
            {
                foreach (Method m in currentClass.Methods)
                {
                    if (m.Id == method.Id && !signatures.Contains(m.Signature))
                    {
                        result.Add(m);
                        signatures.Add(m.Signature);
                    }
                }

                currentClass = _language.GetSymbols(currentClass.Extends);
            }

            // filter out methods based on visibility
            for (int i = 0; i < result.Count; i++)
            {

                if (result[i] is ModifiedSymbol)
                {
                    SymbolModifier modifier = (result[i] as ModifiedSymbol).Modifier;
                    if ((modifier.HasFlag(SymbolModifier.Static) && !isTypeReference) ||
                        (!modifier.HasFlag(SymbolModifier.Static) && isTypeReference))
                    {
                        result.RemoveAt(i);
                        i--;
                    }
                }
            }

            return result.ToArray();
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

            if (symbolTable.TableType == SymbolTableType.Interface)
                foreach (string interfaceName in symbolTable.Interfaces)
                    GetInterfaceMethods(_language.GetSymbols(interfaceName), result);

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
                if (((int)method.Modifier & (int)modifiers) > 0)
                    result.Add(method);
            }

            GetInheritedMethods(_language.GetSymbols(symbolTable.Extends), modifiers, result);
        }

        /// <summary>
        /// Recursive call that gets the methods for the given interface and all interfaces that it implements. 
        /// </summary>
        /// <param name="symbolTable">The symbol table to get the methods for.</param>
        /// <param name="result">The resulting methods are added to this list.</param>
        private void GetInterfaceMethods(SymbolTable symbolTable, List<Method> result)
        {
            if (symbolTable == null)
                return;

            foreach (Method method in symbolTable.Methods)
                result.Add(method);

            foreach (string interfaceName in symbolTable.Interfaces)
                GetInterfaceMethods(_language.GetSymbols(interfaceName), result);
        }

        #endregion
    }
}
