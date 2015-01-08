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
        #region Fields

        /// <summary>
        /// Events that are the start of a nested collection of log units.
        /// </summary>
        private static string[] START_EVENTS = new string[] 
        {
            "CODE_UNIT_STARTED",
            "CONSTRUCTOR_ENTRY",
            "CUMULATIVE_LIMIT_USAGE",
            "CUMULATIVE_PROFILING_BEGIN",
            "DML_BEGIN",
            "EXECUTION_STARTED",
            "FLOW_CREATE_INTERVIEW_BEGIN",
            "FLOW_ELEMENT_BEGIN",
            "FLOW_START_INTERVIEW_BEGIN",
            "FLOW_START_INTERVIEWS_BEGIN",
            "METHOD_ENTRY",
            "QUERY_MORE_BEGIN",
            "SOQL_EXECUTE_BEGIN",
            "SOSL_EXECUTE_BEGIN",
            "SYSTEM_CONSTRUCTOR_ENTRY",
            "SYSTEM_METHOD_ENTRY",
            "VARIABLE_SCOPE_BEGIN",
            "VF_DESERIALIZE_VIEWSTATE_BEGIN",
            "VF_EVALUATE_FORMULA_BEGIN",
            "VF_SERIALIZE_VIEWSTATE_BEGIN",
            "WF_CRITERIA_BEGIN",
            "WF_FLOW_ACTION_BEGIN",
            "WF_RULE_EVAL_BEGIN",
        };

        /// <summary>
        /// Events that are the end of a nested collection of log units.
        /// </summary>
        private static string[] END_EVENTS = new string[]
        {
            "CODE_UNIT_FINISHED",
            "CONSTRUCTOR_EXIT",
            "CUMULATIVE_LIMIT_USAGE_END",
            "CUMULATIVE_PROFILING_END",
            "DML_END",
            "EXECUTION_FINISHED",
            "FLOW_BULK_ELEMENT_END",
            "FLOW_CREATE_INTERVIEW_END",
            "FLOW_ELEMENT_END",
            "FLOW_START_INTERVIEW_END",
            "FLOW_START_INTERVIEWS_END",
            "METHOD_EXIT",
            "QUERY_MORE_END",
            "SOQL_EXECUTE_END",
            "SOSL_EXECUTE_END",
            "SYSTEM_CONSTRUCTOR_EXIT",
            "SYSTEM_METHOD_EXIT",
            "VARIABLE_SCOPE_END",
            "VF_DESERIALIZE_VIEWSTATE_END",
            "VF_EVALUATE_FORMULA_END",
            "VF_SERIALIZE_VIEWSTATE_END",
            "WF_CRITERIA_END",
            "WF_FLOW_ACTION_END",
            "WF_RULE_EVAL_END",
            "WF_RULE_NOT_EVALUATED"
        };

        #endregion

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
            bool isStart = START_EVENTS.Contains(eventType);
            bool isEnd = END_EVENTS.Contains(eventType);

            if (isStart && eventType != "WF_ACTION")
            {
                int lastUnderscore = eventType.LastIndexOf('_');
                if (lastUnderscore > 0)
                    baseEventType = eventType.Substring(0, lastUnderscore);
            }

            // get detail if there is any
            string eventDetail = String.Empty;
            switch (eventType)
            {
                case "DML_BEGIN":
                    eventDetail = FormatEventDetail(parts, 3, 4, 5);
                    break;

                case "WF_CRITERIA_BEGIN":
                    eventDetail = FormatEventDetail(parts, 3);
                    break;

                case "WF_FORMULA":
                    eventDetail = FormatEventDetail(parts, 2);
                    break;

                case "LIMIT_USAGE_FOR_NS":
                    eventDetail = FormatEventDetail(parts, 2);
                    break;

                default:
                    if (parts.Length > 2)
                        eventDetail = parts[parts.Length - 1];
                    break;
            }

            return new LogUnit(timestamp, eventType, eventDetail, baseEventType, isStart, isEnd, lineNumber);
        }

        /// <summary>
        /// Format the parts for display.
        /// </summary>
        /// <param name="parts">The parts to format from.</param>
        /// <param name="indices">The index of each part to use.</param>
        /// <returns>The formated parts.</returns>
        private static string FormatEventDetail(string[] parts, params int[] indices)
        {
            if (parts == null || indices == null || indices.Length == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (int index in indices)
            {
                if (index >= 0 && index < parts.Length)
                    sb.AppendFormat("{0} | ", parts[index]);
            }

            if (sb.Length >= 3)
                sb.Length = sb.Length - 3;

            return sb.ToString();
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
