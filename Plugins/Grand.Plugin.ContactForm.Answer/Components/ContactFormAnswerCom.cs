using Grand.Core;
using Grand.Plugin.ContactFormAnswer.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Components
{
    public class ContactFormAnswerCom : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public ContactFormAnswerCom(IWorkContext workContext,
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
            var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<AnswerSettings>(_storeContext.CurrentStore.Id);

            var model = new ContactFormAnswerInfoModel
            {
                DescriptionText = checkMoneyOrderPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id)
            };

            return View("~/Plugins/Rewards.RewardOperations/Views/PaymentInfo.cshtml", model);
        }
    }
}
