using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Plugin.Widgets.MobileSlider.Infrastructure.Cache;
using Grand.Plugin.Widgets.MobileSlider.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Media;
using Grand.Services.Stores;
using System.Linq;

namespace Grand.Plugin.Widgets.MobileSlider.ViewComponents
{
    [ViewComponent(Name = "Grand.Plugin.Widgets.MobileSlider")]
    public class MobileSliderViewComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;

        public MobileSliderViewComponent(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
        }

        protected string GetPictureUrl(string pictureId)
        {

            string cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
                if (url == null)
                    url = "";

                return url;
            });
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            return null;
        }
    }
}