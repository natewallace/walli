using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wallace.IDE.Framework.UI
{
    /// <summary>
    /// Dependency properties for the ChromeTab style.
    /// </summary>
    public class ChromeTab
    {
        #region Properties

        /// <summary>
        /// When set to false the overflow button will not be displayed.
        /// </summary>
        public static readonly DependencyProperty ShowOverflowButtonProperty = DependencyProperty.RegisterAttached(
            "ShowOverflowButton",
            typeof(bool),
            typeof(ChromeTab),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// When set to false the close button will not be displayed.
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.RegisterAttached(
            "ShowCloseButton",
            typeof(bool),
            typeof(ChromeTab),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// When set to false the tab strip will not be displayed.
        /// </summary>
        public static readonly DependencyProperty ShowTabStripProperty = DependencyProperty.RegisterAttached(
            "ShowTabStrip",
            typeof(bool),
            typeof(ChromeTab),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Methods

        /// <summary>
        /// Get the ShowOverflowButton value.
        /// </summary>
        /// <param name="target">The object to get the value for.</param>
        /// <returns>The requested value.</returns>
        public static bool GetShowOverflowButton(DependencyObject target)
        {
            return (bool)target.GetValue(ShowOverflowButtonProperty);
        }

        /// <summary>
        /// Set the ShowOverflowButton value.
        /// </summary>
        /// <param name="target">The object to set the value for.</param>
        /// <param name="value">The value to set.</param>
        public static void SetShowOverflowButton(DependencyObject target, bool value)
        {
            target.SetValue(ShowOverflowButtonProperty, value);
        }

        /// <summary>
        /// Get the ShowCloseButton value.
        /// </summary>
        /// <param name="target">The object to get the value for.</param>
        /// <returns>The requested value.</returns>
        public static bool GetShowCloseButton(DependencyObject target)
        {
            return (bool)target.GetValue(ShowCloseButtonProperty);
        }

        /// <summary>
        /// Set the ShowCloseButton value.
        /// </summary>
        /// <param name="target">The object to set the value for.</param>
        /// <param name="value">The value to set.</param>
        public static void SetShowCloseButton(DependencyObject target, bool value)
        {
            target.SetValue(ShowCloseButtonProperty, value);
        }

        /// <summary>
        /// Get the ShowTabStrip value.
        /// </summary>
        /// <param name="target">The object to get the value for.</param>
        /// <returns>The requested value.</returns>
        public static bool GetShowTabStrip(DependencyObject target)
        {
            return (bool)target.GetValue(ShowTabStripProperty);
        }

        /// <summary>
        /// Set the ShowTabStrip value.
        /// </summary>
        /// <param name="target">The object to set the value for.</param>
        /// <param name="value">The value to set.</param>
        public static void SetShowTabStrip(DependencyObject target, bool value)
        {
            target.SetValue(ShowTabStripProperty, value);
        }

        #endregion
    }
}
