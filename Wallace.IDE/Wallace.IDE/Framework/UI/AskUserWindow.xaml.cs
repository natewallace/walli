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
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Interaction logic for AskUserWindow.xaml
    /// </summary>
    public partial class AskUserWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructors.
        /// </summary>
        public AskUserWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The image displayed.
        /// </summary>
        public MessageBoxImage Image
        {
            set
            {
                switch (value)
                {
                    case MessageBoxImage.Error:
                        imageIcon.Source = VisualHelper.LoadBitmap("Error32.png");
                        break;

                    case MessageBoxImage.Information:
                        imageIcon.Source = VisualHelper.LoadBitmap("Information32.png");
                        break;

                    case MessageBoxImage.Warning:
                        imageIcon.Source = VisualHelper.LoadBitmap("Warning32.png");
                        break;

                    case MessageBoxImage.Question:
                        imageIcon.Source = VisualHelper.LoadBitmap("Question32.png");
                        break;

                    case MessageBoxImage.None:
                    default:
                        imageIcon.Source = null;
                        break;
                }
            }
        }

        /// <summary>
        /// The text message displayed.
        /// </summary>
        public string Message
        {
            get { return textBlockMessage.Text; }
            set { textBlockMessage.Text = value; }
        }

        /// <summary>
        /// The possible answers the user can choose from.
        /// </summary>
        public IEnumerable<string> Answers
        {
            set
            {
                foreach (Button b in stackPanelButtons.Children)
                    b.Click -= button_Click;

                stackPanelButtons.Children.Clear();

                if (value != null)
                {
                    foreach (string answer in value)
                    {
                        Button button = new Button();
                        button.Content = answer;
                        button.Margin = new Thickness(5);
                        button.Padding = new Thickness(10, 5, 10, 5);
                        button.Click += button_Click;

                        stackPanelButtons.Children.Add(button);
                    }
                }
            }
        }

        /// <summary>
        /// The answer chosen by the user or null if they didn't choose.
        /// </summary>
        public string Answer { get; private set; }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Set the answer and close the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b = sender as Button;
                Answer = b.Content as string;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
