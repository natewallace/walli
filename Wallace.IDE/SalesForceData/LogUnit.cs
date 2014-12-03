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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForceData
{
    /// <summary>
    /// A unit of code within a transaction.
    /// </summary>
    public class LogUnit
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timeStamp">TimeStamp.</param>
        /// <param name="eventType">EventType.</param>
        /// <param name="eventDetail">EventDetail.</param>
        /// <param name="baseEventType">BaseEventType.</param>
        /// <param name="isEventStart">IsEventStart.</param>
        /// <param name="isEventEnd">IsEventEnd.</param>
        /// <param name="lineNumber">LineNumber.</param>
        public LogUnit(
            DateTime timeStamp, 
            string eventType, 
            string eventDetail,
            string baseEventType,
            bool isEventStart,
            bool isEventEnd,
            int lineNumber)
        {
            TimeStamp = timeStamp;
            EventType = eventType;
            EventDetail = eventDetail;
            BaseEventType = baseEventType;
            IsEventStart = isEventStart;
            IsEventEnd = isEventEnd;
            LineNumber = lineNumber;
            Units = new LogUnit[0];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timeStamp">TimeStamp.</param>
        /// <param name="eventType">EventType.</param>
        /// <param name="eventDetail">EventDetail.</param>
        /// <param name="baseEventType">BaseEventType.</param>
        /// <param name="isEventStart">IsEventStart.</param>
        /// <param name="isEventEnd">IsEventEnd.</param>
        /// <param name="lineNumber">LineNumber.</param>
        /// <param name="units">Units.</param>
        public LogUnit(
            DateTime timeStamp,
            string eventType,
            string eventDetail,
            string baseEventType,
            bool isEventStart,
            bool isEventEnd,
            int lineNumber,
            LogUnit[] units)
        {
            TimeStamp = timeStamp;
            EventType = eventType;
            EventDetail = eventDetail;
            BaseEventType = baseEventType;
            IsEventStart = isEventStart;
            IsEventEnd = isEventEnd;
            LineNumber = lineNumber;
            Units = units ?? new LogUnit[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// The time this unit was executed.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// The event for this code unit.
        /// </summary>
        public string EventType { get; private set; }

        /// <summary>
        /// The event detail for this code unit.
        /// </summary>
        public string EventDetail { get; private set; }

        /// <summary>
        /// The base name of the event type.
        /// </summary>
        public string BaseEventType { get; private set; }

        /// <summary>
        /// Indicates this is the start of an event.
        /// </summary>
        public bool IsEventStart { get; private set; }

        /// <summary>
        /// Indicates this is the end of an event.
        /// </summary>
        public bool IsEventEnd { get; private set; }

        /// <summary>
        /// Units that are contained within this log unit.
        /// </summary>
        public LogUnit[] Units { get; private set; }

        /// <summary>
        /// The line number that this unit starts on.  zero based.
        /// </summary>
        public int LineNumber { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the given line and create a corresponding log unit.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <param name="lineNumber">The number of the line.</param>
        /// <returns>The corresponding log unit or null if the line is not for a code unit.</returns>
        public static LogUnit ParseLine(string line, int lineNumber)
        {
            if (String.IsNullOrWhiteSpace(line))
                return null;

            // check for metric entries
            if (line.StartsWith("  Number of ", StringComparison.CurrentCultureIgnoreCase))
            {
                line = line.Substring(12);
                int index = line.IndexOf(':');
                if (index > 0)
                {
                    string et = line.Substring(0, index);
                    string ed = line.Substring(index);

                    return new LogUnit(DateTime.MinValue, et, ed, et, false, false, lineNumber);
                }
                else
                {
                    return new LogUnit(DateTime.MinValue, line, String.Empty, line, false, false, lineNumber);
                }

            }
            else if (line.StartsWith("  Maximum "))
            {
                line = line.Substring(10);
                int index = line.IndexOf(':');
                if (index > 0)
                {
                    string et = line.Substring(0, index);
                    string ed = line.Substring(index);

                    return new LogUnit(DateTime.MinValue, et, ed, et, false, false, lineNumber);
                }
                else
                {
                    return new LogUnit(DateTime.MinValue, line, String.Empty, line, false, false, lineNumber);
                }
            }

            string[] parts = line.Split(new char[] { '|' });
            if (parts.Length < 2)
                return null;

            // get timestamp
            string[] timeStampParts = parts[0].Split(new char[] { ' ' });
            if (timeStampParts.Length < 1)
                return null;

            DateTime timestamp = DateTime.MinValue;
            if (!DateTime.TryParseExact(
                String.Format("01 01 0001 {0}", timeStampParts[0]),
                "MM dd yyyy HH:mm:ss.fff",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out timestamp))
                return null;

            // get event type
            string eventType = parts[1];

            // get base event type and start/end flags
            string baseEventType = eventType;
            bool isStart = (eventType == "CUMULATIVE_LIMIT_USAGE");
            bool isEnd = false;
            int lastUnderscore = eventType.LastIndexOf('_');
            if (lastUnderscore > 0)
            {
                baseEventType = eventType.Substring(0, lastUnderscore);
                string lastWord = eventType.Substring(lastUnderscore + 1).ToUpper();
                switch (lastWord)
                {
                    case "STARTED":
                    case "START":
                    case "ENTRY":
                    case "BEGIN":
                        isStart = true;
                        break;

                    case "FINISHED":
                    case "FINISH":
                    case "EXIT":
                    case "END":
                        isEnd = true;
                        break;

                    default:
                        break;
                }
            }

            // get detail if there is any
            string eventDetail = String.Empty;
            if (eventType == "DML_BEGIN" && parts.Length > 3)
            {
                eventDetail = String.Format(
                    "{0}  {1}  {2}", 
                    parts[parts.Length - 3],
                    parts[parts.Length - 2],
                    parts[parts.Length - 1]);
            }
            else if (parts.Length > 2)
            {
                eventDetail = parts[parts.Length - 1];
            }

            return new LogUnit(timestamp, eventType, eventDetail, baseEventType, isStart, isEnd, lineNumber);
        }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(EventDetail))
                return String.Format("{0} : {1}", EventType, EventDetail);
            else
                return EventType;
        }

        #endregion
    }
}
