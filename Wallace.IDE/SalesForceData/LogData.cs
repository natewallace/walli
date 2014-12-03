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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// A processed log file.
    /// </summary>
    public class LogData
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logContent">The log content to build this object from.</param>
        public LogData(string logContent)
        {            
            Units = new LogUnit[0];

            Stack<LogUnit> unitStack = new Stack<LogUnit>();
            Stack<List<LogUnit>> listStack = new Stack<List<LogUnit>>();
            List<LogUnit> units = new List<LogUnit>();

            if (String.IsNullOrWhiteSpace(logContent))
                return;

            using (StringReader reader = new StringReader(logContent))
            {
                string line = null;
                int lineNumber = -1;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    LogUnit unit = LogUnit.ParseLine(line, lineNumber);
                    if (unit != null)
                    {
                        if (unit.IsEventStart)
                        {
                            unitStack.Push(unit);
                            listStack.Push(units);
                            units = new List<LogUnit>();
                        }
                        else if (unit.IsEventEnd)
                        {
                            LogUnit parent = unitStack.Pop();
                            if (parent == null)
                                return;

                            LogUnit u = new LogUnit(
                                parent.TimeStamp,
                                parent.BaseEventType,
                                parent.EventDetail,
                                parent.BaseEventType,
                                false,
                                false,
                                parent.LineNumber,
                                units.ToArray());

                            units = listStack.Pop();
                            if (units == null)
                                return;

                            units.Add(u);
                        }
                        else
                        {
                            units.Add(unit);
                        }
                    }
                }
            }

            // clear out stack if there any left
            while (unitStack.Count > 0)
            {
                LogUnit parent = unitStack.Pop();
                if (parent == null)
                    return;

                LogUnit u = new LogUnit(
                    parent.TimeStamp,
                    parent.BaseEventType,
                    parent.EventDetail,
                    parent.BaseEventType,
                    false,
                    false,
                    parent.LineNumber,
                    units.ToArray());

                units = listStack.Pop();
                if (units == null)
                    return;

                units.Add(u);
            }

            Units = units.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The units that make up this log.
        /// </summary>
        public LogUnit[] Units { get; private set; }

        #endregion
    }
}
