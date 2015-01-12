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
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Node;
using Wallace.IDE.SalesForce.UI;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Display properties for the current source file.
    /// </summary>
    public class PropertiesFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Gets the currently selected item or null if there isn't one.
        /// </summary>
        /// <returns>The currently selected item or null if there isn't one.</returns>
        private object GetSelectedItem()
        {
            if (App.Instance.SalesForceApp.CurrentProject != null &&
                App.Instance.Navigation.SelectedNodes.Count == 1)
            {
                INode node = App.Instance.Navigation.SelectedNodes[0];
                if (node is SourceFileNode)
                    return (node as SourceFileNode).SourceFile;
                if (node is DataObjectNode)
                    return (node as DataObjectNode).DataObject;
                if (node is DataObjectFieldNode)
                    return (node as DataObjectFieldNode).DataObjectField;

                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the header based on the currently selected class.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            object item = GetSelectedItem();

            if (item != null)
            {
                string name = String.Empty;
                if (item is SourceFile)
                    name = (item as SourceFile).Name;
                else if (item is SObjectTypePartial)
                    name = (item as SObjectTypePartial).Name;
                else if (item is SObjectFieldType)
                    name = (item as SObjectFieldType).Name;

                if (host == FunctionHost.Toolbar)
                {
                    presenter.Header = VisualHelper.CreateIconHeader(null, "Property.png");
                    presenter.ToolTip = String.Format("View {0} properties...", name);
                }
                else
                {
                    name = name.Replace("_", "__");
                    presenter.Header = String.Format("View {0} properties...", name);
                    presenter.Icon = VisualHelper.CreateIconHeader(null, "Property.png");
                }

                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
        }

        /// <summary>
        /// Show properties window.
        /// </summary>
        public override void Execute()
        {
            object item = GetSelectedItem();
            if (item is SourceFile)
            {
                SourceFile file = item as SourceFile;
                PropertiesWindow dlg = new PropertiesWindow();
                dlg.AddProperty("Name", file.Name);
                dlg.AddProperty("File", file.FileName);
                dlg.AddProperty("State", file.State.ToString());
                dlg.AddProperty("Changed by", file.ChangedBy == null ? String.Empty : file.ChangedBy.Name);
                dlg.AddProperty("Changed on", file.ChangedOn.ToLocalTime().ToString());
                dlg.AddProperty("Created by", file.CreatedBy == null ? String.Empty : file.CreatedBy.Name);                
                dlg.AddProperty("Created on", file.CreatedOn.ToLocalTime().ToString());

                if (!String.IsNullOrWhiteSpace(file.Id) && 
                    App.Instance.SalesForceApp.CurrentProject != null &&
                    App.Instance.SalesForceApp.CurrentProject.IsCheckoutEnabled)
                    dlg.AddProperty("Checked out by", file.CheckedOutBy == null ? String.Empty : file.CheckedOutBy.Name);

                App.ShowDialog(dlg);
            }
            else if (item is SObjectTypePartial)
            {
                SObjectTypePartial dataObject = item as SObjectTypePartial;
                PropertiesWindow dlg = new PropertiesWindow();
                dlg.AddProperty("Name", dataObject.Name);
                dlg.AddProperty("Label", dataObject.Label);
                dlg.AddProperty("Plural label", dataObject.LabelPlural);

                App.ShowDialog(dlg);
            }
            else if (item is SObjectFieldType)
            {
                SObjectFieldType dataField = item as SObjectFieldType;
                PropertiesWindow dlg = new PropertiesWindow();
                dlg.AddProperty("Name", dataField.Name);
                dlg.AddProperty("Label", dataField.Label);
                dlg.AddProperty("Type", dataField.FieldType.ToString());
                if (dataField.FieldType == FieldType.Reference && dataField.ReferenceTo != null)
                {
                    foreach (string referenceTo in dataField.ReferenceTo)
                        dlg.AddProperty("Reference to", referenceTo);
                }
                if (dataField.Calculated == true)
                    dlg.AddProperty("Formula", dataField.CalculatedFormula);
                dlg.AddProperty("Length", dataField.Length.ToString());

                App.ShowDialog(dlg);
            }
        }

        #endregion
    }
}
