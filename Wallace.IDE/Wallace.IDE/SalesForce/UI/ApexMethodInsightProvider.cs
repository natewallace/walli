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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;
using SalesForceLanguage.Apex.CodeModel;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Used to display insight tooltip for method parameters.
    /// </summary>
    public class ApexMethodInsightProvider : IOverloadProvider
    {
        #region Fields

        /// <summary>
        /// The methods for the insight window.
        /// </summary>
        private Method[] _methods;

        /// <summary>
        /// Supports the SelectedIndex property.
        /// </summary>
        private int _selectedIndex;

        /// <summary>
        /// Supports the CurrentIndexText property.
        /// </summary>
        private string _currentIndexText;

        /// <summary>
        /// Supports the CurrentHeader property.
        /// </summary>
        private object _currentHeader;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methods">The methods to display insight for.</param>
        public ApexMethodInsightProvider(Method[] methods)
        {
            _methods = methods ?? new Method[0];
            SelectedIndex = 0;
        }

        #endregion

        #region IOverloadProvider Members

        /// <summary>
        /// The number of methods.
        /// </summary>
        public int Count
        {
            get { return _methods.Length; }
        }

        /// <summary>
        /// Text to display for the current index.
        /// </summary>
        public string CurrentIndexText
        {
            get 
            { 
                return _currentIndexText; 
            }
            set
            {
                _currentIndexText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentIndexText"));
            }
        }

        /// <summary>
        /// The header to display.
        /// </summary>
        public object CurrentHeader
        {
            get
            {
                return _currentHeader;
            }
            set
            {
                _currentHeader = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentHeader"));
            }
        }

        /// <summary>
        /// Not currently used.  Would be used for documentation in the future.
        /// </summary>
        public object CurrentContent
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// The index of the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                CurrentIndexText = String.Format("{0} of {1}", SelectedIndex + 1, Count);
                CurrentHeader = _methods[SelectedIndex].ToStringWithReturnType();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Not used.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
