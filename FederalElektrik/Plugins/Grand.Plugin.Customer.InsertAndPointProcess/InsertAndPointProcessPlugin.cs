using Grand.Core;
using Grand.Core.Plugins;
using Grand.Framework.Menu;
using System;
using System.Linq;

namespace Grand.Plugin.Customer.InsertAndPointProcess
{
    public class InsertAndPointProcessPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Constructors

        public InsertAndPointProcessPlugin(
            IWorkContext workContext
            )
        {
            this._workContext = workContext;
        }

        #endregion

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            bool isAdmin = _workContext.CurrentCustomer.CustomerRoles.Any(x => x.SystemName == "Administrators");
            if (isAdmin)
            {
                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Customers");
                if (pluginNode != null)
                {
                    rootNode.ChildNodes.Add(new SiteMapNode()
                    {
                        SystemName = "Customer.InsertAndPointProcess",
                        Title = "Kullanıcı ve Puan Yükleme",
                        Visible = true,
                        IconClass = "fa fa-dot-circle-o",
                        ControllerName = "InsertAndPoint",
                        ActionName = "Index",
                    });
                }
            }
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
