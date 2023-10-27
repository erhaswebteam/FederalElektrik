using Grand.Core;
using Grand.Core.Domain.Localization;
using Grand.Core.Plugins;
using Grand.Plugin.ContactFormAnswer.Controllers;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer
{
    public class AnswerPlugin : BasePlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public AnswerPlugin
            (
            ILocalizationService localizationService,
            IWebHelper webHelper,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext
            )
        {
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "ContactFormAnswerCom";
        }

        public Type GetControllerType()
        {
            return typeof(ContactFormAnswerController);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ContactFormAnswer/Configure";
        }
    }
}
