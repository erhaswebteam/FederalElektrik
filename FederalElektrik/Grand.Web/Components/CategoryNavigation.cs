using Microsoft.AspNetCore.Mvc;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;

namespace Grand.Web.ViewComponents
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogWebService _catalogWebService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryNavigationViewComponent(ICatalogWebService catalogWebService, IHttpContextAccessor httpContextAccessor)
        {
            this._catalogWebService = catalogWebService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke(string currentCategoryId, string currentProductId)
        {
            if (string.IsNullOrEmpty(currentCategoryId))
            {
                var catid = _httpContextAccessor.HttpContext.Request.Query["categoryId"].Count > 0 ? _httpContextAccessor.HttpContext.Request.Query["categoryId"][0].ToString() : "";
                currentCategoryId = catid;
            }
            var model = _catalogWebService.PrepareCategoryNavigation(currentCategoryId, currentProductId);
            return View(model);
        }
    }
}