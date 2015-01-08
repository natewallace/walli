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

namespace SalesForceData
{
    /// <summary>
    /// The different types of trigger events.
    /// </summary>
    [Flags]
    public enum TriggerEvents
    {
        /// <summary>
        /// No trigger event.
        /// </summary>
        None = 0,

        /// <summary>
        /// Before insert.
        /// </summary>
        BeforeInsert = 1,

        /// <summary>
        /// After insert.
        /// </summary>
        AfterInsert = 2,

        /// <summary>
        /// Before update.
        /// </summary>
        BeforeUpdate = 4,

        /// <summary>
        /// After update.
        /// </summary>
        AfterUpdate = 8,

        /// <summary>
        /// Before delete.
        /// </summary>
        BeforeDelete = 16,

        /// <summary>
        /// After delete.
        /// </summary>
        AfterDelete = 32,

        /// <summary>
        /// After undelete.
        /// </summary>
        AfterUndelete = 64
    }
}
