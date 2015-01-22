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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wallace.IDE.SalesForce.Framework;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for Identicon.xaml
    /// </summary>
    public partial class IdenticonControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Supports the Identicon property.
        /// </summary>
        private Identicon _identicon;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public IdenticonControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The identicon displayed.
        /// </summary>
        public Identicon Identicon
        {
            get
            {
                return _identicon;
            }
            set
            {
                _identicon = value;
                if (value != null)
                {
                    SolidColorBrush brush = new SolidColorBrush(value.Color);

                    grid_0_0.Background = value.Bits[0] ? brush : Brushes.White;
                    grid_0_1.Background = value.Bits[1] ? brush : Brushes.White;
                    grid_0_2.Background = value.Bits[2] ? brush : Brushes.White;
                    grid_0_3.Background = value.Bits[3] ? brush : Brushes.White;
                    grid_0_4.Background = value.Bits[4] ? brush : Brushes.White;

                    grid_1_0.Background = value.Bits[5] ? brush : Brushes.White;
                    grid_1_1.Background = value.Bits[6] ? brush : Brushes.White;
                    grid_1_2.Background = value.Bits[7] ? brush : Brushes.White;
                    grid_1_3.Background = value.Bits[8] ? brush : Brushes.White;
                    grid_1_4.Background = value.Bits[9] ? brush : Brushes.White;

                    grid_2_0.Background = value.Bits[10] ? brush : Brushes.White;
                    grid_2_1.Background = value.Bits[11] ? brush : Brushes.White;
                    grid_2_2.Background = value.Bits[12] ? brush : Brushes.White;
                    grid_2_3.Background = value.Bits[13] ? brush : Brushes.White;
                    grid_2_4.Background = value.Bits[14] ? brush : Brushes.White;

                    grid_3_0.Background = value.Bits[15] ? brush : Brushes.White;
                    grid_3_1.Background = value.Bits[16] ? brush : Brushes.White;
                    grid_3_2.Background = value.Bits[17] ? brush : Brushes.White;
                    grid_3_3.Background = value.Bits[18] ? brush : Brushes.White;
                    grid_3_4.Background = value.Bits[19] ? brush : Brushes.White;

                    grid_4_0.Background = value.Bits[20] ? brush : Brushes.White;
                    grid_4_1.Background = value.Bits[21] ? brush : Brushes.White;
                    grid_4_2.Background = value.Bits[22] ? brush : Brushes.White;
                    grid_4_3.Background = value.Bits[23] ? brush : Brushes.White;
                    grid_4_4.Background = value.Bits[24] ? brush : Brushes.White;
                }
                else
                {
                    grid_0_0.Background = Brushes.White;
                    grid_0_1.Background = Brushes.White;
                    grid_0_2.Background = Brushes.White;
                    grid_0_3.Background = Brushes.White;
                    grid_0_4.Background = Brushes.White;

                    grid_1_0.Background = Brushes.White;
                    grid_1_1.Background = Brushes.White;
                    grid_1_2.Background = Brushes.White;
                    grid_1_3.Background = Brushes.White;
                    grid_1_4.Background = Brushes.White;

                    grid_2_0.Background = Brushes.White;
                    grid_2_1.Background = Brushes.White;
                    grid_2_2.Background = Brushes.White;
                    grid_2_3.Background = Brushes.White;
                    grid_2_4.Background = Brushes.White;

                    grid_3_0.Background = Brushes.White;
                    grid_3_1.Background = Brushes.White;
                    grid_3_2.Background = Brushes.White;
                    grid_3_3.Background = Brushes.White;
                    grid_3_4.Background = Brushes.White;

                    grid_4_0.Background = Brushes.White;
                    grid_4_1.Background = Brushes.White;
                    grid_4_2.Background = Brushes.White;
                    grid_4_3.Background = Brushes.White;
                    grid_4_4.Background = Brushes.White;
                }
            }
        }

        #endregion
    }
}
