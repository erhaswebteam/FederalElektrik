using Grand.Core.Plugins;
using Grand.Framework.Menu;
using Grand.Services.Common;
using System;

namespace Grand.Plugin.ShoppingCard.ClearShoppingCards
{
    public class ClearShoppingCards : BasePlugin, IAdminMenuPlugin
    {
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            foreach (var item in rootNode.ChildNodes)
            {
                if (item.SystemName == "Customers")
                {
                    var Menu = new SiteMapNode();
                    Menu.Title = "Clear Shoppingcards";
                    Menu.Visible = true;
                    Menu.SystemName = "ClearShoppingCards";
                    Menu.ControllerName = "ClearShoppingCards";
                    Menu.ActionName = "Index";
                    Menu.RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary() { { "area", "admin" } };
                    Menu.IconClass = "fa fa-arrow-circle-o-right";
                    item.ChildNodes.Add(Menu);
                }
            }
        }
    }
}
