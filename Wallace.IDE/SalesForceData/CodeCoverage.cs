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
    /// A code coverage entry for a given file.
    /// </summary>
    public class CodeCoverage
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="coverage">The object to build this object from.</param>
        internal CodeCoverage(SourceFile file, SalesForceData.SalesForceAPI.Tooling.ApexCodeCoverageAggregate coverage)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            File = file;

            if (coverage.Coverage != null)
            {
                LinesCovered = coverage.Coverage.coveredLines ?? new int[0];
                LinesUncovered = coverage.Coverage.uncoveredLines ?? new int[0];
            }
            else
            {
                LinesCovered = new int[0];
                LinesUncovered = new int[0];
            }

            // set percentage
            decimal totalLines = LinesCovered.Length + LinesUncovered.Length;
            if (totalLines == 0)
                CoveragePercent = 0;
            else
                CoveragePercent = (int)(((decimal)LinesCovered.Length / totalLines) * 100m);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The file the code coverage is for.
        /// </summary>
        public SourceFile File { get; private set; }

        /// <summary>
        /// The total lines covered.
        /// </summary>
        public int[] LinesCovered { get; private set; }

        /// <summary>
        /// The number of lines covered.
        /// </summary>
        public int NumberOfLinesCovered
        {
            get { return LinesCovered.Length; }
        }

        /// <summary>
        /// The total lines uncovered.
        /// </summary>
        public int[] LinesUncovered { get; private set; }

        /// <summary>
        /// The number of lines uncovered.
        /// </summary>
        public int NumberOfLinesUncovered
        {
            get { return LinesUncovered.Length; }
        }

        /// <summary>
        /// The percentage of code covered.
        /// </summary>
        public int CoveragePercent { get; private set; }

        #endregion
    }
}
