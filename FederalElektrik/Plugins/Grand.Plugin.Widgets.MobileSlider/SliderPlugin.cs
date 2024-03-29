using System;
using System.Collections.Generic;
using System.IO;
using Grand.Core;
using Grand.Core.Plugins;
using Grand.Services.Cms;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Media;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Widgets.MobileSlider
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class SliderPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public SliderPlugin(IPictureService pictureService, 
            ISettingService settingService, IWebHelper webHelper)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_top" };
        }

        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //pictures
            var sampleImagesPath = CommonHelper.MapPath("~/Plugins/Widgets.MobileSlider/Content/slider/sample-images/");

            //settings
            var settings = new MobileSliderSettings
            {
                Picture1Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner1.jpg"), "image/pjpeg", "banner_1").Id,
                Text1 = "Sample text 1",
                Link1 = _webHelper.GetStoreLocation(false),
                Picture2Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner2.jpg"), "image/pjpeg", "banner_2").Id,
                Text2 = "Sample text 2",
                Link2 = _webHelper.GetStoreLocation(false),
            };
            _settingService.SaveSetting(settings);


            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture1", "Picture 1");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture2", "Picture 2");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture3", "Picture 3");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture4", "Picture 4");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture5", "Picture 5");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture.Hint", "Upload picture.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Text", "Comment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Link", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MobileSlider.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<MobileSliderSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture1");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture2");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture3");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture4");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture5");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Picture.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Text");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Text.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Link");
            this.DeletePluginLocaleResource("Plugins.Widgets.MobileSlider.Link.Hint");
            
            base.Uninstall();
        }

        public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "Grand.Plugin.Widgets.MobileSlider";
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/WidgetsMobileSlider/Configure";
        }
    }
}