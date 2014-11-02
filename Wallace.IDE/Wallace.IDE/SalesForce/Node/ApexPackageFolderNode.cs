using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.Node
{
    /// <summary>
    /// Holds nodes that are part of a managed package.
    /// </summary>
    public class ApexPackageFolderNode : NodeBase
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="namespacePrefix">NamespacePrefix.</param>
        public ApexPackageFolderNode(string namespacePrefix)
        {
            if (String.IsNullOrWhiteSpace(namespacePrefix))
                throw new ArgumentException("namespacePrefix can't be null or whitespace.", "namespacePrefix");

            NamespacePrefix = namespacePrefix;
            PackageNodes = new List<INode>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The namespace for the package.
        /// </summary>
        public string NamespacePrefix { get; private set; }

        /// <summary>
        /// The nodes that make up this package.
        /// </summary>
        public IList<INode> PackageNodes { get; private set; }

        /// <summary>
        /// The text that represents this node.
        /// </summary>
        public override string Text
        {
            get
            {
                return NamespacePrefix;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        public override void Init()
        {
            Presenter.Header = VisualHelper.CreateIconHeader(NamespacePrefix, "FolderClosed.png");
            Presenter.ExpandedHeader = VisualHelper.CreateIconHeader(NamespacePrefix, "FolderOpen.png");
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
        /// Set the _isClassesLoaded flag.
        /// </summary>
        /// <returns>The children apex classes that were read in.</returns>
        public override INode[] GetChildren()
        {
            return PackageNodes.OrderBy(n => n.Text).ToArray();
        }

        /// <summary>
        /// Remove the given file from the folder.
        /// </summary>
        /// <param name="sourceFile">The source file to remove.</param>
        public void RemoveSourceFile(SourceFile sourceFile)
        {
            for (int i = 0; i < Presenter.Nodes.Count; i++)
            {
                if (Presenter.Nodes[i] is SourceFileNode && (Presenter.Nodes[i] as SourceFileNode).SourceFile.Equals(sourceFile))
                {
                    Presenter.Nodes.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion
    }
}
