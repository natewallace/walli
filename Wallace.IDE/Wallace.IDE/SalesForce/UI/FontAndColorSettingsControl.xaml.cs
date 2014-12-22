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
    public partial class FontAndColorSettingsControl : UserControl
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
        public FontAndColorSettingsControl()
        {
            InitializeComponent();

            SettingFontFamily = null;
            SettingFontSize = 0;
            SettingSymbols = null;
        }

        #endregion

        #region Properties

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
                textBlockForeground.Text = ColorSelectWindow.GetColorName(value);
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
                textBlockBackground.Text = ColorSelectWindow.GetColorName(value);
            }
        }

        /// <summary>
        /// The selection foreground color.
        /// </summary>
        public Color? SettingSelectionForeground
        {
            get
            {
                if (buttonSelectionForeground.Tag is Color)
                    return (Color)buttonSelectionForeground.Tag;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    buttonSelectionForeground.Tag = value;
                    borderSelectionForeground.Background = new SolidColorBrush(value.Value);
                    textBlockSelectionForeground.Text = ColorSelectWindow.GetColorName(value.Value);
                }
                else
                {
                    buttonSelectionForeground.Tag = null;
                    borderSelectionForeground.Background = null;
                    textBlockSelectionForeground.Text = "(none)";
                }
            }
        }

        /// <summary>
        /// The selection background color.
        /// </summary>
        public Color? SettingSelectionBackground
        {
            get
            {
                if (buttonSelectionBackground.Tag is Color)
                    return (Color)buttonSelectionBackground.Tag;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    buttonSelectionBackground.Tag = value;
                    borderSelectionBackground.Background = new SolidColorBrush(value.Value);
                    textBlockSelectionBackground.Text = ColorSelectWindow.GetColorName(value.Value);
                }
                else
                {
                    buttonSelectionBackground.Tag = null;
                    borderSelectionBackground.Background = null;
                    textBlockSelectionBackground.Text = "(none)";
                }
            }
        }

        /// <summary>
        /// The find result background color.
        /// </summary>
        public Color SettingFindResultBackground
        {
            get
            {
                if (buttonFindResultBackground.Tag is Color)
                    return (Color)buttonFindResultBackground.Tag;
                else
                    return Colors.Transparent;
            }
            set
            {
                if (value != null)
                {
                    buttonFindResultBackground.Tag = value;
                    borderFindResultBackground.Background = new SolidColorBrush(value);
                    textBlockFindResultBackground.Text = ColorSelectWindow.GetColorName(value);
                }
                else
                {
                    buttonFindResultBackground.Tag = null;
                    borderFindResultBackground.Background = null;
                    textBlockFindResultBackground.Text = ColorSelectWindow.GetColorName(value);
                }
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

        /// <summary>
        /// The themes a user can select from.
        /// </summary>
        public bool ShowThemes
        {
            get
            {
                return (buttonTheme.Visibility == Visibility.Visible);
            }
            set
            {
                if (value)
                {
                    textBlockTheme.Visibility = Visibility.Visible;
                    buttonTheme.Visibility = Visibility.Visible;
                }
                else
                {
                    textBlockTheme.Visibility = Visibility.Collapsed;
                    buttonTheme.Visibility = Visibility.Collapsed;
                }
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

        /// <summary>
        /// Raises the ThemeClick event.
        /// </summary>
        /// <param name="e">Arguments to pass with the event.</param>
        protected virtual void OnThemeClick(EventArgs e)
        {
            if (ThemeClick != null)
                ThemeClick(this, e);
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
                EditorSymbolSettings ess = SelectedSettingSymbol;
                if (ess != null)
                {
                    ColorSelectWindow dlg = new ColorSelectWindow();
                    dlg.AllowNoColor = true;
                    dlg.SelectedColor = ess.Foreground;
                    if (App.ShowDialog(dlg))
                    {
                        ess.Foreground = dlg.SelectedColor;
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
                EditorSymbolSettings ess = SelectedSettingSymbol;
                if (ess != null)
                {
                    ColorSelectWindow dlg = new ColorSelectWindow();
                    dlg.AllowNoColor = true;
                    dlg.SelectedColor = ess.Background;
                    if (App.ShowDialog(dlg))
                    {
                        ess.Background = dlg.SelectedColor;
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
                ColorSelectWindow dlg = new ColorSelectWindow();
                dlg.SelectedColor = SettingForeground;
                if (App.ShowDialog(dlg) && dlg.SelectedColor.HasValue)
                {
                    SettingForeground = dlg.SelectedColor.Value;
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
                ColorSelectWindow dlg = new ColorSelectWindow();
                dlg.SelectedColor = SettingBackground;
                if (App.ShowDialog(dlg) && dlg.SelectedColor.HasValue)
                {
                    SettingBackground = dlg.SelectedColor.Value;
                    UpdateView();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// The selection foreground for the editor.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSelectionForeground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorSelectWindow dlg = new ColorSelectWindow();
                dlg.AllowNoColor = true;
                dlg.SelectedColor = SettingSelectionForeground;
                if (App.ShowDialog(dlg))
                {
                    SettingSelectionForeground = dlg.SelectedColor;
                    UpdateView();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// The selection background for the editor.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonSelectionBackground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorSelectWindow dlg = new ColorSelectWindow();
                dlg.AllowNoColor = true;
                dlg.SelectedColor = SettingSelectionBackground;
                if (App.ShowDialog(dlg))
                {
                    SettingSelectionBackground = dlg.SelectedColor;
                    UpdateView();
                }
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// The find result background for the editor.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonFindResultBackground_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorSelectWindow dlg = new ColorSelectWindow();
                dlg.SelectedColor = SettingFindResultBackground;
                if (App.ShowDialog(dlg) && dlg.SelectedColor.HasValue)
                {
                    SettingFindResultBackground = dlg.SelectedColor.Value;
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

        /// <summary>
        /// Select a theme to apply.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnThemeClick(EventArgs.Empty);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the user clicks the theme button.
        /// </summary>
        public event EventHandler ThemeClick;

        #endregion
    }
}
