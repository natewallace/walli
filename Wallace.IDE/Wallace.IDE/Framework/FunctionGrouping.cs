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

namespace Wallace.IDE.Framework
{
    /// <summary>
    /// This class is used to group other functions together.
    /// </summary>
    public class FunctionGrouping : FunctionBase
    {
        #region Fields

        /// <summary>
        /// Holds text that will be displayed.
        /// </summary>
        private string _text;

        /// <summary>
        /// When true, pad the text on the left.
        /// </summary>
        private bool _padIcon;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The id for this function.</param>
        /// <param name="text">Text that will be displayed for this function.</param>
        public FunctionGrouping(string id, string text)
        {
            Id = id;
            _text = text;
            _padIcon = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The id for this function.</param>
        /// <param name="text">Text that will be displayed for this function.</param>
        /// <param name="padIcon">If true the text will be padded on the left by the same width as an icon.</param>
        public FunctionGrouping(string id, string text, bool padIcon)
        {
            Id = id;
            _text = text;
            _padIcon = padIcon;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set header displayed.
        /// </summary>
        /// <param name="host">The type of the host.</param>
        /// <param name="presenter">Presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (_padIcon)
            {
                presenter.Header = _text;
                presenter.Icon = VisualHelper.CreateIconHeader(null, "Empty.png");
            }
            else
            {
                presenter.Header = _text;
            }
        }

        #endregion
    }
}
