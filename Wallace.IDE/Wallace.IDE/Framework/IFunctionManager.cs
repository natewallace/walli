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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// Interface to a function manager.
    /// </summary>
    public interface IFunctionManager
    {
        /// <summary>
        /// Add the given function to the root of the control.
        /// </summary>
        /// <param name="function">The function to add.</param>
        void AddFunction(IFunction function);

        /// <summary>
        /// Add the given function to the root of the control.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="position">The position where the function will be inserted.</param>
        void AddFunction(IFunction function, int position);

        /// <summary>
        /// Add the given function to the sub items of the function with the given id.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="parentId">The id of the parent to add the function to.</param>
        void AddFunction(IFunction function, string parentId);

        /// <summary>
        /// Add the given function to the sub items of the function with the given id.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="parentId">The id of the parent to add the function to.</param>
        /// <param name="position">The position where the function will be inserted.</param>
        void AddFunction(IFunction function, string parentId, int position);

        /// <summary>
        /// Remove the function from the control.
        /// </summary>
        /// <param name="function">The function to remove.</param>
        void RemoveFunction(IFunction function);

        /// <summary>
        /// Update all functions.
        /// </summary>
        void UpdateFunctions();
    }
}
