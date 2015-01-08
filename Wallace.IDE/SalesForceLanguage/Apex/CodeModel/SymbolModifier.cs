/*
 * Copyright (c) 2015 Nathaniel Wallace
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

namespace SalesForceLanguage.Apex.CodeModel
{
    /// <summary>
    /// Possible visibility for a symbol.
    /// </summary>
    [Flags]
    public enum SymbolModifier
    {
        /// <summary>
        /// No modifiers.
        /// </summary>
        None = 0,

        /// <summary>
        /// Public.
        /// </summary>
        Public = 1,

        /// <summary>
        /// Protected.
        /// </summary>
	    Protected = 2,

        /// <summary>
        /// Privatey.
        /// </summary>
	    Private = 4,

        /// <summary>
        /// Static.
        /// </summary>
	    Static = 8,

        /// <summary>
        /// Global.
        /// </summary>
	    Global = 16,

        /// <summary>
        /// Override.
        /// </summary>
	    Override = 32,

        /// <summary>
        /// Virtual.
        /// </summary>
	    Virtual = 64,

        /// <summary>
        /// TestMethod.
        /// </summary>
	    TestMethod = 128,

        /// <summary>
        /// Transient.
        /// </summary>
	    Transient = 256,

        /// <summary>
        /// WithSharing.
        /// </summary>
	    WithSharing = 512,

        /// <summary>
        /// WithoutSharing.
        /// </summary>
	    WithoutSharing = 1024,

        /// <summary>
        /// WebService.
        /// </summary>
	    WebService = 2048,

        /// <summary>
        /// Final.
        /// </summary>
	    Final = 4096,

        /// <summary>
        /// Abstract.
        /// </summary>
        Abstract = 8192
    }
}
