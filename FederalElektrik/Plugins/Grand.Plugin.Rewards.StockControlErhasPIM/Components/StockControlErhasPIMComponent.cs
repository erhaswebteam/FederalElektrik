using Grand.Core;
using Grand.Plugin.Rewards.StockControlErhasPIM.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Plugin.Rewards.StockControlErhasPIM.Components
{
    class StockControlErhasPIMComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public StockControlErhasPIMComponent(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            ILanguageService languageService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._languageService = languageService;
        }

        public IViewComponentResult Invoke()
        {
            var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<StockOperationsSettings>(_storeContext.CurrentStore.Id);

            var model = new StockControlErhasPIMModel
            {
                DescriptionText = checkMoneyOrderPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id)
            };

            return View("~/Plugins/Rewards.StockControlErhasPIM/Views/PaymentInfo.cshtml", model);
        }
    }
}
