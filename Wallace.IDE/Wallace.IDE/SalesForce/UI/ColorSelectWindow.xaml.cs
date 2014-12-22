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

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for ColorSelectWindow.xaml
    /// </summary>
    public partial class ColorSelectWindow : Window
    {
        #region Fields

        /// <summary>
        /// Holds the last selection.
        /// </summary>
        private object _previousSelection;

        /// <summary>
        /// A map of color hex value to it's color name.
        /// </summary>
        private static Dictionary<string, string> _colorNames;

        /// <summary>
        /// The no color option.
        /// </summary>
        private object _noColor;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ColorSelectWindow()
        {
            _colorNames = new Dictionary<string, string>();
            _colorNames.Add(Colors.AliceBlue.ToString(), "AliceBlue");
            _colorNames.Add(Colors.AntiqueWhite.ToString(), "AntiqueWhite");
            _colorNames.Add(Colors.Aqua.ToString(), "Aqua");
            _colorNames.Add(Colors.Aquamarine.ToString(), "Aquamarine");
            _colorNames.Add(Colors.Azure.ToString(), "Azure");
            _colorNames.Add(Colors.Beige.ToString(), "Beige");
            _colorNames.Add(Colors.Bisque.ToString(), "Bisque");
            _colorNames.Add(Colors.Black.ToString(), "Black");
            _colorNames.Add(Colors.BlanchedAlmond.ToString(), "BlanchedAlmond");
            _colorNames.Add(Colors.Blue.ToString(), "Blue");
            _colorNames.Add(Colors.BlueViolet.ToString(), "BlueViolet");
            _colorNames.Add(Colors.Brown.ToString(), "Brown");
            _colorNames.Add(Colors.BurlyWood.ToString(), "BurlyWood");
            _colorNames.Add(Colors.CadetBlue.ToString(), "CadetBlue");
            _colorNames.Add(Colors.Chartreuse.ToString(), "Chartreuse");
            _colorNames.Add(Colors.Chocolate.ToString(), "Chocolate");
            _colorNames.Add(Colors.Coral.ToString(), "Coral");
            _colorNames.Add(Colors.CornflowerBlue.ToString(), "CornflowerBlue");
            _colorNames.Add(Colors.Cornsilk.ToString(), "Cornsilk");
            _colorNames.Add(Colors.Crimson.ToString(), "Crimson");
            _colorNames.Add(Colors.DarkBlue.ToString(), "DarkBlue");
            _colorNames.Add(Colors.DarkCyan.ToString(), "DarkCyan");
            _colorNames.Add(Colors.DarkGoldenrod.ToString(), "DarkGoldenrod");
            _colorNames.Add(Colors.DarkGray.ToString(), "DarkGray");
            _colorNames.Add(Colors.DarkGreen.ToString(), "DarkGreen");
            _colorNames.Add(Colors.DarkKhaki.ToString(), "DarkKhaki");
            _colorNames.Add(Colors.DarkMagenta.ToString(), "DarkMagenta");
            _colorNames.Add(Colors.DarkOliveGreen.ToString(), "DarkOliveGreen");
            _colorNames.Add(Colors.DarkOrange.ToString(), "DarkOrange");
            _colorNames.Add(Colors.DarkOrchid.ToString(), "DarkOrchid");
            _colorNames.Add(Colors.DarkRed.ToString(), "DarkRed");
            _colorNames.Add(Colors.DarkSalmon.ToString(), "DarkSalmon");
            _colorNames.Add(Colors.DarkSeaGreen.ToString(), "DarkSeaGreen");
            _colorNames.Add(Colors.DarkSlateBlue.ToString(), "DarkSlateBlue");
            _colorNames.Add(Colors.DarkSlateGray.ToString(), "DarkSlateGray");
            _colorNames.Add(Colors.DarkTurquoise.ToString(), "DarkTurquoise");
            _colorNames.Add(Colors.DarkViolet.ToString(), "DarkViolet");
            _colorNames.Add(Colors.DeepPink.ToString(), "DeepPink");
            _colorNames.Add(Colors.DeepSkyBlue.ToString(), "DeepSkyBlue");
            _colorNames.Add(Colors.DimGray.ToString(), "DimGray");
            _colorNames.Add(Colors.DodgerBlue.ToString(), "DodgerBlue");
            _colorNames.Add(Colors.Firebrick.ToString(), "Firebrick");
            _colorNames.Add(Colors.FloralWhite.ToString(), "FloralWhite");
            _colorNames.Add(Colors.ForestGreen.ToString(), "ForestGreen");
            _colorNames.Add(Colors.Fuchsia.ToString(), "Fuchsia");
            _colorNames.Add(Colors.Gainsboro.ToString(), "Gainsboro");
            _colorNames.Add(Colors.GhostWhite.ToString(), "GhostWhite");
            _colorNames.Add(Colors.Gold.ToString(), "Gold");
            _colorNames.Add(Colors.Goldenrod.ToString(), "Goldenrod");
            _colorNames.Add(Colors.Gray.ToString(), "Gray");
            _colorNames.Add(Colors.Green.ToString(), "Green");
            _colorNames.Add(Colors.GreenYellow.ToString(), "GreenYellow");
            _colorNames.Add(Colors.Honeydew.ToString(), "Honeydew");
            _colorNames.Add(Colors.HotPink.ToString(), "HotPink");
            _colorNames.Add(Colors.IndianRed.ToString(), "IndianRed");
            _colorNames.Add(Colors.Indigo.ToString(), "Indigo");
            _colorNames.Add(Colors.Ivory.ToString(), "Ivory");
            _colorNames.Add(Colors.Khaki.ToString(), "Khaki");
            _colorNames.Add(Colors.Lavender.ToString(), "Lavender");
            _colorNames.Add(Colors.LavenderBlush.ToString(), "LavenderBlush");
            _colorNames.Add(Colors.LawnGreen.ToString(), "LawnGreen");
            _colorNames.Add(Colors.LemonChiffon.ToString(), "LemonChiffon");
            _colorNames.Add(Colors.LightBlue.ToString(), "LightBlue");
            _colorNames.Add(Colors.LightCoral.ToString(), "LightCoral");
            _colorNames.Add(Colors.LightCyan.ToString(), "LightCyan");
            _colorNames.Add(Colors.LightGoldenrodYellow.ToString(), "LightGoldenrodYellow");
            _colorNames.Add(Colors.LightGray.ToString(), "LightGray");
            _colorNames.Add(Colors.LightGreen.ToString(), "LightGreen");
            _colorNames.Add(Colors.LightPink.ToString(), "LightPink");
            _colorNames.Add(Colors.LightSalmon.ToString(), "LightSalmon");
            _colorNames.Add(Colors.LightSeaGreen.ToString(), "LightSeaGreen");
            _colorNames.Add(Colors.LightSkyBlue.ToString(), "LightSkyBlue");
            _colorNames.Add(Colors.LightSlateGray.ToString(), "LightSlateGray");
            _colorNames.Add(Colors.LightSteelBlue.ToString(), "LightSteelBlue");
            _colorNames.Add(Colors.LightYellow.ToString(), "LightYellow");
            _colorNames.Add(Colors.Lime.ToString(), "Lime");
            _colorNames.Add(Colors.LimeGreen.ToString(), "LimeGreen");
            _colorNames.Add(Colors.Linen.ToString(), "Linen");
            _colorNames.Add(Colors.Maroon.ToString(), "Maroon");
            _colorNames.Add(Colors.MediumAquamarine.ToString(), "MediumAquamarine");
            _colorNames.Add(Colors.MediumBlue.ToString(), "MediumBlue");
            _colorNames.Add(Colors.MediumOrchid.ToString(), "MediumOrchid");
            _colorNames.Add(Colors.MediumPurple.ToString(), "MediumPurple");
            _colorNames.Add(Colors.MediumSeaGreen.ToString(), "MediumSeaGreen");
            _colorNames.Add(Colors.MediumSlateBlue.ToString(), "MediumSlateBlue");
            _colorNames.Add(Colors.MediumSpringGreen.ToString(), "MediumSpringGreen");
            _colorNames.Add(Colors.MediumTurquoise.ToString(), "MediumTurquoise");
            _colorNames.Add(Colors.MediumVioletRed.ToString(), "MediumVioletRed");
            _colorNames.Add(Colors.MidnightBlue.ToString(), "MidnightBlue");
            _colorNames.Add(Colors.MintCream.ToString(), "MintCream");
            _colorNames.Add(Colors.MistyRose.ToString(), "MistyRose");
            _colorNames.Add(Colors.Moccasin.ToString(), "Moccasin");
            _colorNames.Add(Colors.NavajoWhite.ToString(), "NavajoWhite");
            _colorNames.Add(Colors.Navy.ToString(), "Navy");
            _colorNames.Add(Colors.OldLace.ToString(), "OldLace");
            _colorNames.Add(Colors.Olive.ToString(), "Olive");
            _colorNames.Add(Colors.OliveDrab.ToString(), "OliveDrab");
            _colorNames.Add(Colors.Orange.ToString(), "Orange");
            _colorNames.Add(Colors.OrangeRed.ToString(), "OrangeRed");
            _colorNames.Add(Colors.Orchid.ToString(), "Orchid");
            _colorNames.Add(Colors.PaleGoldenrod.ToString(), "PaleGoldenrod");
            _colorNames.Add(Colors.PaleGreen.ToString(), "PaleGreen");
            _colorNames.Add(Colors.PaleTurquoise.ToString(), "PaleTurquoise");
            _colorNames.Add(Colors.PaleVioletRed.ToString(), "PaleVioletRed");
            _colorNames.Add(Colors.PapayaWhip.ToString(), "PapayaWhip");
            _colorNames.Add(Colors.PeachPuff.ToString(), "PeachPuff");
            _colorNames.Add(Colors.Peru.ToString(), "Peru");
            _colorNames.Add(Colors.Pink.ToString(), "Pink");
            _colorNames.Add(Colors.Plum.ToString(), "Plum");
            _colorNames.Add(Colors.PowderBlue.ToString(), "PowderBlue");
            _colorNames.Add(Colors.Purple.ToString(), "Purple");
            _colorNames.Add(Colors.Red.ToString(), "Red");
            _colorNames.Add(Colors.RosyBrown.ToString(), "RosyBrown");
            _colorNames.Add(Colors.RoyalBlue.ToString(), "RoyalBlue");
            _colorNames.Add(Colors.SaddleBrown.ToString(), "SaddleBrown");
            _colorNames.Add(Colors.Salmon.ToString(), "Salmon");
            _colorNames.Add(Colors.SandyBrown.ToString(), "SandyBrown");
            _colorNames.Add(Colors.SeaGreen.ToString(), "SeaGreen");
            _colorNames.Add(Colors.SeaShell.ToString(), "SeaShell");
            _colorNames.Add(Colors.Sienna.ToString(), "Sienna");
            _colorNames.Add(Colors.Silver.ToString(), "Silver");
            _colorNames.Add(Colors.SkyBlue.ToString(), "SkyBlue");
            _colorNames.Add(Colors.SlateBlue.ToString(), "SlateBlue");
            _colorNames.Add(Colors.SlateGray.ToString(), "SlateGray");
            _colorNames.Add(Colors.Snow.ToString(), "Snow");
            _colorNames.Add(Colors.SpringGreen.ToString(), "SpringGreen");
            _colorNames.Add(Colors.SteelBlue.ToString(), "SteelBlue");
            _colorNames.Add(Colors.Tan.ToString(), "Tan");
            _colorNames.Add(Colors.Teal.ToString(), "Teal");
            _colorNames.Add(Colors.Thistle.ToString(), "Thistle");
            _colorNames.Add(Colors.Tomato.ToString(), "Tomato");
            _colorNames.Add(Colors.Transparent.ToString(), "Transparent");
            _colorNames.Add(Colors.Turquoise.ToString(), "Turquoise");
            _colorNames.Add(Colors.Violet.ToString(), "Violet");
            _colorNames.Add(Colors.Wheat.ToString(), "Wheat");
            _colorNames.Add(Colors.White.ToString(), "White");
            _colorNames.Add(Colors.WhiteSmoke.ToString(), "WhiteSmoke");
            _colorNames.Add(Colors.Yellow.ToString(), "Yellow");
            _colorNames.Add(Colors.YellowGreen.ToString(), "YellowGreen");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ColorSelectWindow()
        {
            InitializeComponent();

            _previousSelection = null;

            _noColor = CreateItemColor(Colors.Transparent);
            ((_noColor as StackPanel).Children[1] as TextBlock).Text = "(none)";
        }

        #endregion

        #region Properties

        /// <summary>
        /// The selected color.
        /// </summary>
        public Color? SelectedColor
        {
            get
            {
                return GetItemColor(comboBoxInput.SelectedItem);
            }
            set
            {
                if (value != null)
                {
                    bool found = false;
                    foreach (object item in comboBoxInput.Items)
                    {
                        if (item == panelCustomColor)
                            continue;

                        Color? c = GetItemColor(item);
                        if (c.HasValue && c.Value == value)
                        {
                            found = true;
                            comboBoxInput.SelectedItem = item;
                            break;
                        }
                    }

                    if (!found)
                    {
                        object item = CreateItemColor(value.Value);
                        comboBoxInput.Items.Add(item);
                        comboBoxInput.SelectedItem = item;
                    }
                }
                else
                {
                    if (AllowNoColor)
                        comboBoxInput.SelectedItem = _noColor;
                    else
                        comboBoxInput.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Enable the option to select null for a color.
        /// </summary>
        public bool AllowNoColor
        {
            get
            {
                return comboBoxInput.Items.Contains(_noColor);
            }
            set
            {
                if (value)
                {
                    if (!comboBoxInput.Items.Contains(_noColor))
                        comboBoxInput.Items.Insert(0, _noColor);
                }
                else
                {
                    comboBoxInput.Items.Remove(_noColor);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the name to display for the color.
        /// </summary>
        /// <param name="color">The color to get the name for.</param>
        /// <returns>The name for the color.</returns>
        public static string GetColorName(Color color)
        {
            string key = color.ToString();
            if (_colorNames.ContainsKey(key))
                return _colorNames[key];
            else
                return key;
        }

        /// <summary>
        /// Get the color for the given item that is in the combo box item.
        /// </summary>
        /// <param name="item">The combobox item to get the color for.</param>
        /// <returns>The color for the given item or null if it can't be read.</returns>
        private Color? GetItemColor(object item)
        {
            if (item == null || item == _noColor)
                return null;

            StackPanel panel = item as StackPanel;
            if (panel == null || panel.Children.Count == 0)
                return null;

            Border border = panel.Children[0] as Border;
            if (border == null)
                return null;

            SolidColorBrush brush = border.Background as SolidColorBrush;
            if (brush == null)
                return null;

            return brush.Color;
        }

        /// <summary>
        /// Create a combobox item for the given color.
        /// </summary>
        /// <param name="color">The color to create an item for.</param>
        /// <returns>The item for display in the combo box.</returns>
        private object CreateItemColor(Color color)
        {
            Border border = new Border();
            border.Height = 16;
            border.Width = 16;
            border.Background = new SolidColorBrush(color);

            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            textBlock.Text = color.ToString();

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(border);
            panel.Children.Add(textBlock);

            return panel;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Close the dialog with the selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Cancel the dialog.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedColor = null;
                DialogResult = false;
                Close();
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        /// <summary>
        /// Process a click on the custom color option.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void comboBoxInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxInput.SelectedItem == panelCustomColor)
                {
                    System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Color color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                        SelectedColor = color;
                    }
                    else
                    {
                        if (_previousSelection != panelCustomColor)
                            comboBoxInput.SelectedItem = _previousSelection;
                    }
                }
                else
                {
                    _previousSelection = comboBoxInput.SelectedItem;
                }

                buttonSelect.IsEnabled = (comboBoxInput.SelectedItem != null);
            }
            catch (Exception err)
            {
                App.HandleException(err);
            }
        }

        #endregion
    }
}
