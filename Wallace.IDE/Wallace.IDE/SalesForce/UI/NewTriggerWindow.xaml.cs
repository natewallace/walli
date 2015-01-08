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
using System.Windows;
using SalesForceData;

namespace Wallace.IDE.SalesForce.UI
{
    /// <summary>
    /// Interaction logic for NewTriggerWindow.xaml
    /// </summary>
    public partial class NewTriggerWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewTriggerWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the trigger.
        /// </summary>
        public string TriggerName
        {
            get { return textBoxName.Text.Trim(); }
            set { textBoxName.Text = value; }
        }

        /// <summary>
        /// The selected object to create the trigger on.
        /// </summary>
        public object TriggerObject
        {
            get { return comboBoxObject.SelectedItem; }
            set { comboBoxObject.SelectedItem = value; }
        }

        /// <summary>
        /// The available trigger objects to select from.
        /// </summary>
        public object[] TriggerObjects
        {
            get { return comboBoxObject.ItemsSource as object[]; }
            set { comboBoxObject.ItemsSource = value; }
        }

        /// <summary>
        /// The checked trigger events.
        /// </summary>
        public TriggerEvents TriggerEvents
        {
            get
            {
                TriggerEvents result = SalesForceData.TriggerEvents.None;
                if (checkBoxBeforeInsert.IsChecked.Value)
                    result |= TriggerEvents.BeforeInsert;
                if (checkBoxAfterInsert.IsChecked.Value)
                    result |= TriggerEvents.AfterInsert;
                if (checkBoxBeforeUpdate.IsChecked.Value)
                    result |= TriggerEvents.BeforeUpdate;
                if (checkBoxAfterUpdate.IsChecked.Value)
                    result |= TriggerEvents.AfterUpdate;
                if (checkBoxBeforeDelete.IsChecked.Value)
                    result |= TriggerEvents.BeforeDelete;
                if (checkBoxAfterDelete.IsChecked.Value)
                    result |= TriggerEvents.AfterDelete;
                if (checkBoxAfterUndelete.IsChecked.Value)
                    result |= TriggerEvents.AfterUndelete;

                return result;
            }
            set
            {
                checkBoxBeforeInsert.IsChecked = value.HasFlag(TriggerEvents.BeforeInsert);
                checkBoxAfterInsert.IsChecked = value.HasFlag(TriggerEvents.AfterInsert);
                checkBoxBeforeUpdate.IsChecked = value.HasFlag(TriggerEvents.BeforeUpdate);
                checkBoxAfterUpdate.IsChecked = value.HasFlag(TriggerEvents.AfterUpdate);
                checkBoxBeforeDelete.IsChecked = value.HasFlag(TriggerEvents.BeforeDelete);
                checkBoxAfterDelete.IsChecked = value.HasFlag(TriggerEvents.AfterDelete);
                checkBoxAfterUndelete.IsChecked = value.HasFlag(TriggerEvents.AfterUndelete);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Set the dialog result to true and close.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCreate_Click(object sender, RoutedEventArgs e)
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
        /// Set the dialog result to false and close.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
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
