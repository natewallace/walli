using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;
using Wallace.IDE.SalesForce.Document;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Remove a comment.
    /// </summary>
    public class RemoveCommentFunction : FunctionBase
    {
        #region Properties

        /// <summary>
        /// The current document.
        /// </summary>
        private IDocument CurrentDocument
        {
            get { return App.Instance.Content.ActiveDocument; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the header.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            if (host == FunctionHost.Toolbar)
            {
                presenter.Header = VisualHelper.CreateIconHeader(null, "RemoveComment.png");
                presenter.ToolTip = "Uncomment selected lines";
            }
            else
            {
                presenter.Header = "Uncomment selected lines";
                presenter.Icon = VisualHelper.CreateIconHeader(null, "RemoveComment.png");
            }
        }

        /// <summary>
        /// Set visibility.
        /// </summary>
        /// <param name="host">The type of host.</param>
        /// <param name="presenter">The presenter to use.</param>
        public override void Update(FunctionHost host, IFunctionPresenter presenter)
        {
            IsVisible = (CurrentDocument is ClassEditorDocument || CurrentDocument is TriggerEditorDocument);
        }

        /// <summary>
        /// Comment out selected lines.
        /// </summary>
        public override void Execute()
        {
            if (CurrentDocument is ClassEditorDocument)
                (CurrentDocument as ClassEditorDocument).CommentSelectedText(false);
            else if (CurrentDocument is TriggerEditorDocument)
                (CurrentDocument as TriggerEditorDocument).CommentSelectedText(false);
        }

        #endregion
    }
}
