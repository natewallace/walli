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

using ICSharpCode.AvalonEdit.Highlighting;
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
    /// Interaction logic for EditorSettingsControl.xaml
    /// </summary>
    public partial class EditorSettingsControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Supports the SettingFontFamily property.
        /// </summary>
        private FontFamily _settingFontFamily;

        /// <summary>
        /// Supports the SettingFontSize property.
        /// </summary>
        private double _settingFontSize;

        /// <summary>
        /// Supports the SettingSymbols property.
        /// </summary>
        private IEnumerable<EditorSymbolSettings> _settingSymbols;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditorSettingsControl()
        {
            InitializeComponent();

            SettingFontFamily = null;
            SettingFontSize = 0;
            SettingSymbols = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The header entered by the user.
        /// </summary>
        public string SettingHeader
        {
            get { return textBoxHeader.Text; }
            set { textBoxHeader.Text = value; }
        }

        /// <summary>
        /// Show/Hide the header input controls.
        /// </summary>
        public bool ShowSettingHeader
        {
            get { return textBoxHeader.Visibility == Visibility.Visible; }
            set
            {
                textBoxHeader.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                textBlockHeader.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The font selected by the user.
        /// </summary>
        public FontFamily SettingFontFamily
        {
            get
            {
                return _settingFontFamily;
            }
            set
            {
                _settingFontFamily = value;
                UpdateView();
            }
        }

        /// <summary>
        /// The size of the font in points.
        /// </summary>
        public double SettingFontSize
        {
            get
            {
                return _settingFontSize;
            }
            set
            {
                _settingFontSize = value;
                UpdateView();
            }
        }

        /// <summary>
        /// The foreground color.
        /// </summary>
        public Color SettingForeground
        {
            get
            {
                if (buttonForeground.Tag is Color)
                    return (Color)buttonForeground.Tag;
                else
                    return Colors.Transparent;
            }
            set
            {
                buttonForeground.Tag = value;
                borderForeground.Background = new SolidColorBrush(value);
                textBlockForeground.Text = value.ToString();
            }
        }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color SettingBackground
        {
            get
            {
                if (buttonBackground.Tag is Color)
                    return (Color)buttonBackground.Tag;
                else
                    return Colors.Transparent;
            }
            set
            {
                buttonBackground.Tag = value;
                borderBackground.Background = new SolidColorBrush(value);
                textBlockBackground.Text = value.ToString();
            }
        }

        /// <summary>
        /// The symbol colors.
        /// </summary>
        public IEnumerable<EditorSymbolSettings> SettingSymbols
        {
            get
            {
                return _settingSymbols;
            }
            set
            {
                _settingSymbols = value;
                listBoxSymbols.Items.Clear();
                if (_settingSymbols != null)
                {
                    foreach (EditorSymbolSettings ess in _settingSymbols)
                        listBoxSymbols.Items.Add(ess.Name);
                }

                UpdateView();
            }
        }

        /// <summary>
        /// Get the currently selected symbol or null if there isn't one.
        /// </summary>
        private EditorSymbolSettings SelectedSettingSymbol
        {
            get
            {
                if (SettingSymbols == null)
                    return null;

                string symbolName = listBoxSymbols.SelectedItem as string;
                if (symbolName == null)
                    return null;

                foreach (EditorSymbolSettings ess in SettingSymbols)
                    if (ess.Name == symbolName)
                        return ess;

                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the view to display the current state.
        /// </summary>
        private void UpdateView()
        {
            if (SettingFontFamily != null)
                buttonFont.Content = String.Format("{0}, {1:0}pt", SettingFontFamily, SettingFontSize);
            else
                buttonFont.Content = String.Format("none, {0:0}pt", SettingFontSize);

            borderSymbolForeground.Background = null;
            textBlockSymbolForeground.Text = null;

            borderSymbolBackground.Background = null;
            textBlockSymbolBackground.Text = null;

            checkBoxSymbolBold.IsChecked = false;
            checkBoxSymbolItalic.IsChecked = false;

            EditorSymbolSettings ess = SelectedSettingSymbol;
            if (ess != null)
            {
                if (ess.Foreground.HasValue)
                    borderSymbolForeground.Background = new SolidColorBrush(ess.Foreground.Value);
                textBlockSymbolForeground.Text = ess.ForegroundAsString;

                if (ess.Background.HasValue)
                    borderSymbolBackground.Background = new SolidColorBrush(ess.Background.Value);
                textBlockSymbolBackground.Text = ess.BackgroundAsString;

                checkBoxSymbolBold.IsChecked = ess.IsBold;
                checkBoxSymbolItalic.IsChecked = ess.IsItalic;

                textBlockSymbolForegroundTitle.Visibility = Visibility.Visible;
                buttonSymbolForeground.Visibility = Visibility.Visible;
                textBlockSymbolBackgroundTitle.Visibility = Visibility.Visible;
                buttonSymbolBackground.Visibility = Visibility.Visible;
                stackPanelSymbolBold.Visibility = Visibility.Visible;
                stackPanelSymbolItalic.Visibility = Visibility.Visible;
            }
            else
            {
                textBlockSymbolForegroundTitle.Visibility = Visibility.Hidden;
                buttonSymbolForeground.Visibility = Visibility.Hidden;
                textBlockSymbolBackgroundTitle.Visibility = Visibility.Hidden;
                buttonSymbolBackground.Visibility = Visibility.Hidden;
                stackPanelSymbolBold.Visibility = Visibility.Hidden;
                stackPanelSymbolItalic.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Show the font selection dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonFont_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FontDialog dlg = new System.Windows.Forms.FontDialog();
                dlg.ShowEffects = false;
                dlg.AllowScriptChange = false;
                if (SettingFontFamily != null)
                    dlg.Font = new System.Drawing.Font(SettingFontFamily.Source, (float)SettingFontSize);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SettingFontFamily = new FontFamily(dlg.Font.Name);
                    SettingFontSize = dlg.Font.SizeInPoints;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Show the color selection dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSymbolForeground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();                
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    EditorSymbolSettings ess = SelectedSettingSymbol;
                    if (ess != null)
                    {
                        Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                        ess.Foreground = color;
                        UpdateView();
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Show the color selection dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSymbolBackground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    EditorSymbolSettings ess = SelectedSettingSymbol;
                    if (ess != null)
                    {
                        Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                        ess.Background = color;
                        UpdateView();
                    }
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// The foreground for the editor.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonForeground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    SettingForeground = color;
                    UpdateView();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// The background for the editor.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonBackground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    SettingBackground = color;
                    UpdateView();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the view.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listBoxSymbols_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateView();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the symbol.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void checkBoxSymbolBold_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                EditorSymbolSettings ess = SelectedSettingSymbol;
                if (ess != null)
                {
                    ess.IsBold = checkBoxSymbolBold.IsChecked.HasValue && checkBoxSymbolBold.IsChecked.Value;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Update the symbol.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void checkBoxSymbolItalic_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                EditorSymbolSettings ess = SelectedSettingSymbol;
                if (ess != null)
                {
                    ess.IsItalic = checkBoxSymbolItalic.IsChecked.HasValue && checkBoxSymbolItalic.IsChecked.Value;
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
