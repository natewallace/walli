﻿/*
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

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for CodeCoverageControl.xaml
    /// </summary>
    public partial class CodeCoverageControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CodeCoverageControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The code coverage displayed.
        /// </summary>
        public CodeCoverage[] CodeCoverage
        {
            get { return dataGridFiles.ItemsSource as CodeCoverage[]; }
            set { dataGridFiles.ItemsSource = value; }
        }

        /// <summary>
        /// The organization wide coverage percent.
        /// </summary>
        public int OrganizationWideCoveragePercent
        {
            get { return Convert.ToInt32(textBlockOrgCoverage.Text); }
            set 
            { 
                textBlockOrgCoverage.Text = value.ToString();
                if (value < 75)
                {
                    textBlockOrgCoverage.Foreground = new SolidColorBrush(Color.FromRgb(116, 0, 0));
                    textBlockOrgCoverage.Background = new SolidColorBrush(Color.FromRgb(255, 221, 221));
                }
                else
                {
                    textBlockOrgCoverage.Foreground = Brushes.Black;
                    textBlockOrgCoverage.Background = null;
                }
            }
        }

        #endregion
    }
}
