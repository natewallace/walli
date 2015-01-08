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
using System.Linq;
using System.Windows;
using SalesForceData;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Framework;
using Wallace.IDE.SalesForce.Function;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Displays schema for data objects.
    /// </summary>
    public class DataObjectNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="dataObject">DataObject.</param>
        public DataObjectNode(Project project, SObjectTypePartial dataObject)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (dataObject == null)
                throw new ArgumentNullException("dataObject");

            Project = project;
            DataObject = dataObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The project this node represents.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The data object being shown.
        /// </summary>
        public SObjectTypePartial DataObject { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return (DataObject == null) ? String.Empty : DataObject.Name;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(DataObject.Name, "Object.png");
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        /// <returns>true.</returns>
        public override bool HasChildren()
        {
            return true;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The resulting children.</returns>
        public override INode[] GetChildren()
        {
            List<INode> nodes = new List<INode>();

            SObjectType fullObject = Project.Client.DataDescribeObjectType(DataObject);
            foreach (SObjectFieldType field in fullObject.Fields.OrderBy(f => f.Name))
                nodes.Add(new DataObjectFieldNode(Project, field));

            Project.Language.UpdateSymbols(
                Project.ConvertToSymbolTable(fullObject),
                true,
                true);

            return nodes.ToArray();
        }

        /// <summary>
        /// Allow text to be dragged.
        /// </summary>
        public override void DragStart()
        {
            Presenter.DoDragDrop(DataObject.Name, DragDropEffects.Copy);
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
            return (DataObject.CompareTo(entity) == 0);
        }

        #endregion
    }
}
