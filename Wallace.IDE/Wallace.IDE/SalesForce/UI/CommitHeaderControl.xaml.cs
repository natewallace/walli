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
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for CommitHeaderControl.xaml
    /// </summary>
    public partial class CommitHeaderControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Supports the Author property.
        /// </summary>
        private string _author;

        /// <summary>
        /// Supports the Sha property.
        /// </summary>
        private string _sha;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommitHeaderControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The comment on the commit.
        /// </summary>
        public string Comment
        {
            get { return textBlockComment.Text; }
            set { textBlockComment.Text = value; }
        }

        /// <summary>
        /// The author of the commit.
        /// </summary>
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                identiconUser.Identicon = Identicon.Get(value);
                textBlockAuthor.Text = String.Format("{0} - {1}", _author, _sha);
            }
        }

        /// <summary>
        /// The sha of the commit.
        /// </summary>
        public string Sha
        {
            get { return _sha; }
            set
            {
                _sha = value;
                textBlockAuthor.Text = String.Format("{0} - {1}", _author, _sha);
            }
        }

        /// <summary>
        /// The date of the commit.
        /// </summary>
        public DateTime? Date
        {
            get
            {
                if (textBlockDate.Tag == null)
                    return null;
                else
                    return (DateTime)textBlockDate.Tag;
            }
            set
            {
                if (value.HasValue)
                {
                    textBlockDate.Text = value.Value.ToString("ddd M/d/yyyy h:mm tt");
                    textBlockDate.Tag = value.Value;
                }
                else
                {
                    textBlockDate.Text = null;
                    textBlockDate.Tag = null;
                }
            }
        }

        #endregion
    }
}
