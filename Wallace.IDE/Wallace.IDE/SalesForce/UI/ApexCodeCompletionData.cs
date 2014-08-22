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

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using SalesForceLanguage.Apex.CodeModel;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Apex code completion item.
    /// </summary>
    public class ApexCodeCompletionData : ICompletionData
    {
        #region Fields

        /// <summary>
        /// The symbol used for the code completion.
        /// </summary>
        private Symbol _symbol;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol used for the code completion.</param>
        public ApexCodeCompletionData(Symbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            _symbol = symbol;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the name of the icon file to use for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to get the icon file name for.</param>
        /// <returns>The file name for the given symbol or null if it is not defined.</returns>
        public static string GetIconFileName(Symbol symbol)
        {
            ModifiedSymbol modifiedSymbol = symbol as ModifiedSymbol;

            if (symbol is Field)
            {
                if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Final))
                {
                    if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Private))
                        return "ConstantPrivate.png";
                    else if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Protected))
                        return "ConstantProtected.png";
                    else
                        return "Constant.png";
                }
                else
                {
                    if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Private))
                        return "FieldPrivate.png";
                    else if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Protected))
                        return "FieldProtected.png";
                    else
                        return "Field.png";
                }
            }
            else if (symbol is Property)
            {
                if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Private))
                    return "PropertyPrivate.png";
                else
                    return "Property.png";
            }
            else if (symbol is Method || symbol is Constructor)
            {
                if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Protected))
                    return "MethodProtected.png";
                else if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Private))
                    return "MethodPrivate.png";
                else
                    return "Method.png";
            }
            else if (symbol is SymbolTable)
            {
                SymbolTable st = symbol as SymbolTable;

                switch (st.TableType)
                {
                    case SymbolTableType.Interface:
                        return "interface.png";

                    case SymbolTableType.Class:
                        switch (st.Type.ToLower())
                        {
                            case "system.integer":
                            case "system.blob":
                            case "system.date":
                            case "system.datetime":
                            case "system.long":
                            case "system.id":
                            case "system.boolean":
                            case "system.decimal":
                            case "system.double":
                                return "Structure.png";

                            default:
                                if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Private))
                                    return "ClassPrivate.png";
                                else if (modifiedSymbol.Modifier.HasFlag(SymbolModifier.Protected))
                                    return "ClassProtected.png";
                                else
                                    return "Class.png";
                        }

                    case SymbolTableType.SObject:
                        return "Object.png";

                    default:
                        return null;
                }
            }
            else if (symbol is Keyword)
            {
                switch (symbol.Name.ToLower())
                {
                    case "true":
                    case "false":
                    case "null":
                        return "Literal.png";

                    default:
                        return "Keyword.png";
                }
            }
            else
                return null;

            
        }

        #endregion

        #region ICompletionData Members

        /// <summary>
        /// Do the completion.
        /// </summary>
        /// <param name="textArea">The text area to use.</param>
        /// <param name="completionSegment">The completion segment.</param>
        /// <param name="insertionRequestEventArgs">Insertion request arguments.</param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (_symbol is Keyword && (_symbol.Id == "select" || _symbol.Id == "find"))
            {
                textArea.Document.Replace(completionSegment, Text + " ]");
                textArea.Caret.Offset = textArea.Caret.Offset - 1;
            }
            else
            {
                textArea.Document.Replace(completionSegment, Text);
            }
        }

        /// <summary>
        /// Returns the text.
        /// </summary>
        public object Content
        {
            get { return Text; }
        }

        /// <summary>
        /// The type of symbol.
        /// </summary>
        public object Description
        {
            get { return null; }
        }

        /// <summary>
        /// The image for the symbol.
        /// </summary>
        public ImageSource Image
        {
            get 
            {
                return VisualHelper.LoadBitmap(GetIconFileName(_symbol));
            }
        }

        /// <summary>
        /// Not used.  Returns 1.
        /// </summary>
        public double Priority
        {
            get { return 1; }
        }

        /// <summary>
        /// The symbol name.
        /// </summary>
        public string Text
        {
            get { return _symbol.Name; }
        }

        #endregion
    }
}
