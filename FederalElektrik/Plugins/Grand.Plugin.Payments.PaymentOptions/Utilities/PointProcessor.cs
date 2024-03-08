using Grand.Core;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Core.Plugins;
using Grand.Plugin.Payments.PaymentOptions.Models;
using Grand.Services.Catalog;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Orders;
using Grand.Services.Stores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Grand.Plugin.Payments.PaymentOptions.Utilities
{
    public class PointProcessor : BasePlugin
    {
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IRewardPointsService _rewardPointsService;
        private readonly IStoreContext _storeContext;

        public PointProcessor(IProductService productService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IRewardPointsService rewardPointsService,
            IStoreContext storeContext)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._rewardPointsService = rewardPointsService;
            this._storeContext = storeContext;
        }

        public bool SendPointService(Order order, decimal orderPoint,Grand.Core.Domain.Customers.Customer cus)
        {
            bool isSuccess = false;
            if (order != null)
            {
                List<OrderItemPointModel> orderItemPoint = new List<OrderItemPointModel>();
                orderItemPoint.Add(new OrderItemPointModel
                {
                    OrderItemPoint = orderPoint,
                    SupplierProcessId = order.OrderGuid.ToString(),
                    SecondaryProcessId = order.OrderGuid.ToString(),
                    Message = string.Format("[{0}] nolu sipariş için düşülen puan", order.OrderGuid)
                });

                PointSpendingModel pointSpendingModel = new PointSpendingModel
                {
                    FirstProcessId = order.OrderGuid.ToString(),
                    OrderPoint = orderPoint,
                    Username = cus.Username,
                    OrderItems = orderItemPoint
                };

                RewardUserOperations rewardUserOperations = new RewardUserOperations(_localizationService, _localizationSettings, _logger, _settingService, _storeService, _workContext);
                ProcessResponseModel result = rewardUserOperations.SetSpendingPoint(pointSpendingModel);
                if (!result.Success)
                {
                    //processPaymentResult.AddError("Üzgünüz, bakiye işlemi gerçekleştirilemedi! Lütfen bakiyenizi kontrol edin veya daha sonra tekrar deneyin.");
                }
                else
                {
                    var pointBalanceTotal = _rewardPointsService.GetRewardPointsBalance(cus.Id, _storeContext.CurrentStore.Id);
                    if (pointBalanceTotal > 0)
                        _rewardPointsService.RemoveOldPointAndAddNewPoints(cus.Id, (pointBalanceTotal - pointSpendingModel.OrderPoint), _storeContext.CurrentStore.Id, string.Empty);

                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        public bool CancelTransection(Order order, decimal orderPoint, Grand.Core.Domain.Customers.Customer cus)
        {
            bool isSuccess = false;
            if (order != null)
            {
                List<OrderItemPointModel> orderItemPoint = new List<OrderItemPointModel>();
                orderItemPoint.Add(new OrderItemPointModel
                {
                    OrderItemPoint = orderPoint,
                    SupplierProcessId = order.OrderGuid.ToString(),
                    SecondaryProcessId = order.OrderGuid.ToString(),
                    Message = string.Format("[{0}] nolu sipariş için geri düşülen puan", order.OrderGuid)
                });

                PointSpendingModel pointSpendingModel = new PointSpendingModel
                {
                    FirstProcessId = order.OrderGuid.ToString(),
                    OrderPoint = orderPoint,
                    Username = cus.Username,
                    OrderItems = orderItemPoint
                };

                RewardUserOperations rewardUserOperations = new RewardUserOperations(_localizationService, _localizationSettings, _logger, _settingService, _storeService, _workContext);
                ProcessResponseModel result = rewardUserOperations.CancelTransaction(pointSpendingModel);
                if (!result.Success)
                {
                    //processPaymentResult.AddError("Üzgünüz, bakiye işlemi gerçekleştirilemedi! Lütfen bakiyenizi kontrol edin veya daha sonra tekrar deneyin.");
                }
                else
                {
                    var pointBalanceTotal = _rewardPointsService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                    if (pointBalanceTotal > 0)
                        _rewardPointsService.RemoveOldPointAndAddNewPoints(_workContext.CurrentCustomer.Id, (pointBalanceTotal + pointSpendingModel.OrderPoint), _storeContext.CurrentStore.Id, string.Empty);

                    isSuccess = true;
                }
            }
            return isSuccess;
        }

       
    }
}
