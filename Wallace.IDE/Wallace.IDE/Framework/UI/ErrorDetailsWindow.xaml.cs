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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for ErrorDetailsWindow.xaml
    /// </summary>
    public partial class ErrorDetailsWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ErrorDetailsWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The error details displayed.
        /// </summary>
        public Exception Error
        {
            get
            {
                return textBoxDetails.Tag as Exception;
            }
            set
            {
                textBoxDetails.Text = String.Empty;
                textBoxDetails.Tag = value;

                if (value != null)
                {
                    StringBuilder sb = new StringBuilder();
                    Exception e = value;
                    while (e != null)
                    {
                        sb.AppendFormat("Message: {0}", e.Message);
                        sb.AppendLine();
                        sb.AppendLine("Stack trace:");
                        sb.AppendLine(e.StackTrace);
                        sb.AppendLine();

                        e = e.InnerException;
                    }

                    textBoxDetails.Text = sb.ToString();
                }
            }
        }

        #endregion
    }
}
