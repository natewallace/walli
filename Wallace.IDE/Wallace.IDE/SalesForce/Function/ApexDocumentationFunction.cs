using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.IDE.Framework;

namespace Wallace.IDE.SalesForce.Function
{
    /// <summary>
    /// Launches the apex documentation site.
    /// </summary>
    public class ApexDocumentationFunction : FunctionBase
    {
        #region Methods

        /// <summary>
        /// Set header displayed.
        /// </summary>
        /// <param name="host">The type of the host.</param>
        /// <param name="presenter">Presenter to use.</param>
        public override void Init(FunctionHost host, IFunctionPresenter presenter)
        {
            presenter.Header = VisualHelper.CreateIconHeader("Apex documentation...", null);
        }

        /// <summary>
        /// Display apex documentation site.
        /// </summary>
        public override void Execute()
        {
            try
            {
                using (App.Wait("Opening apex documentation..."))
                    System.Diagnostics.Process.Start("http://www.salesforce.com/us/developer/docs/apexcode/index_Left.htm");
            }
            catch
            {
            }
        }

        #endregion
    }
}
