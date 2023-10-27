using Microsoft.AspNetCore.Mvc;
using Grand.Web.Services;
using Grand.Core;
using Grand.Services.Orders;
using System.Linq;
using Grand.Services.Points;

namespace Grand.Web.ViewComponents
{
    public class SearchBoxViewComponent : ViewComponent
    {
        private readonly ICatalogWebService _catalogWebService;
        private readonly IWorkContext _workContext;
        private readonly IProcessService _processService;

        public SearchBoxViewComponent(ICatalogWebService catalogWebService, IWorkContext workContext, IProcessService processService)
        {
            this._catalogWebService = catalogWebService;
            this._workContext = workContext;
            this._processService = processService;
        }

        public IViewComponentResult Invoke()
        {
            var model = _catalogWebService.PrepareSearchBox();

            //var rewardPoints = _rewardPointsService.GetRewardPointsHistory(_workContext.CurrentCustomer.Id).OrderByDescending(x => x.CreatedOnUtc).FirstOrDefault();
            //if (rewardPoints != null)
            //    model.RewardPoints = rewardPoints.PointsBalance;
            //else
            //    model.RewardPoints = 0;

            decimal point = 0;
                try
                {
                    point = _processService.GetCustomerActualPoint(_workContext.CurrentCustomer.Username);
                }
                catch (System.Exception)
                {
                    point = 0;
                }

            model.RewardPoints = point <= 0 ? 0 : point;


            return View(model);
        }
    }
}