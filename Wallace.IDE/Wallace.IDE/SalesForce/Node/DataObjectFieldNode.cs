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
using System.Text;
using System.Windows;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// A node for a data object field.
    /// </summary>
    public class DataObjectFieldNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="dataObjectField">DataObjectField.</param>
        public DataObjectFieldNode(Project project, SObjectFieldType dataObjectField)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (dataObjectField == null)
                throw new ArgumentNullException("dataObjectField");

            Project = project;
            DataObjectField = dataObjectField;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The data object field being shown.
        /// </summary>
        public SObjectFieldType DataObjectField { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return (DataObjectField == null) ? String.Empty : DataObjectField.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(DataObjectField.Name, "ObjectField.png");
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        /// <returns>true.</returns>
        public override bool HasChildren()
        {
            return false;
        }

        /// <summary>
        /// Allow text to be dragged.
        /// </summary>
        public override void DragStart()
        {
            StringBuilder sb = new StringBuilder();
            foreach (INode node in Presenter.NodeManager.SelectedNodes)
                if (node is DataObjectFieldNode)
                    sb.AppendFormat("{0}, ", (node as DataObjectFieldNode).DataObjectField.Name);

            if (sb.Length > 2)
            {
                sb.Length -= 2;
                Presenter.DoDragDrop(sb.ToString(), DragDropEffects.Copy);
            }
        }

        /// <summary>
        /// Get the functions for this node.
        /// </summary>
        /// <returns>The functions for this node.</returns>
        public override IFunction[] GetContextFunctions()
        {
            return new IFunction[]
            {
                App.Instance.GetFunction<PropertiesFunction>()
            };
        }

        /// <summary>
        /// If this node represents the given entity this method should return true.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>true if this node represents the given entity.</returns>
        public override bool RepresentsEntity(object entity)
        {
            return (DataObjectField.CompareTo(entity) == 0);
        }

        #endregion
    }
}
