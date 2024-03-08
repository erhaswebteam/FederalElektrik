using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Tax;
using Grand.Core.Plugins;
using Grand.Framework.Security.Captcha;
using Grand.Plugin.Payments.PaymentOptions.Models;
using Grand.Services.Authentication;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Events;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Stores;
using Grand.Services.Tax;
using Grand.Web.Models.Customer;
using Grand.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Grand.Plugin.Payments.PaymentOptions.Utilities
{
    public class RewardUserOperations : BasePlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public RewardUserOperations(
            ILocalizationService localizationService,
            LocalizationSettings localizationSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._localizationSettings = localizationSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        /// <summary>
        /// Kullanıcı adı ve şifreye göre kullanıcı bilgilerini getiren fonksiyon...
        /// </summary>
        /// <returns></returns>
        public GetCustomerResponseModel GetCustomer(string Username, string Password)
        {
            GetCustomerResponseModel result = new GetCustomerResponseModel();

            try
            {
                var _rewardOperationsSettings = _settingService.LoadSetting<PaymentOptionSettings>();

                string address = _rewardOperationsSettings.AddressGetUser;
                string projectPreChar = _rewardOperationsSettings.ProjectPreChars;
                string token = _rewardOperationsSettings.ApiToken;

                UserDataRequestModel model = new UserDataRequestModel
                {
                    Token = token,
                    ProjectPreChars = projectPreChar,
                    Username = Username,
                    Password = Password
                };

                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                try
                {
                    result = SendRequest<GetCustomerResponseModel>(address, request);
                }
                catch (Exception ex)
                {
                    result.ErrorCode = -999;
                    result.Success = false;
                    result.Description = ex.Message;
                }
            }
            catch (Exception  ex)
            {
                result.ErrorCode = -888;
                result.Success = false;
                result.Description = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// Puan harcatma işlemi...
        /// </summary>
        /// <returns></returns>
        public ProcessResponseModel SetSpendingPoint(PointSpendingModel model)
        {
            ProcessResponseModel result = new ProcessResponseModel();

            try
            {
                var _rewardOperationsSettings = _settingService.LoadSetting<PaymentOptionSettings>();

                string address = _rewardOperationsSettings.AddressPointSpending;
                string projectPreChar = _rewardOperationsSettings.ProjectPreChars;
                string token = _rewardOperationsSettings.ApiToken;

                if (model != null)
                {
                    model.Token = token;
                    model.ProjectPreChars = projectPreChar;
                }

                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                try
                {
                    result = SendRequest<ProcessResponseModel>(address, request);
                }
                catch (Exception ex)
                {
                    result.ErrorCode = -999;
                    result.Success = false;
                    result.Description = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.ErrorCode = -888;
                result.Success = false;
                result.Description = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// API Post işlemlerini yürüten fonksiyonum...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public T SendRequest<T>(string address, string dataObject)
        {
            var request = (HttpWebRequest)WebRequest.Create("" + address);
            request.Method = "POST";
            var encoding = new UTF8Encoding();

            var byteArray = encoding.GetBytes(dataObject);
            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            T data;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                string text = sr.ReadToEnd();
                data = JsonConvert.DeserializeObject<T>(text);
            }

            return data;
        }

        /// <summary>
        /// Kullanıcı adı ve şifreye göre kullanıcı bilgilerini getiren fonksiyon...
        /// </summary>
        /// <returns></returns>
        public GetAllPointResponseModel GetAllPointByCustomer(string Username)
        {
            GetAllPointResponseModel result = new GetAllPointResponseModel();

            try
            {
                var _rewardOperationsSettings = _settingService.LoadSetting<PaymentOptionSettings>();

                string address = _rewardOperationsSettings.AddressAllPointsByUser;
                string projectPreChar = _rewardOperationsSettings.ProjectPreChars;
                string token = _rewardOperationsSettings.ApiToken;

                UserDataRequestModel model = new UserDataRequestModel
                {
                    Token = token,
                    ProjectPreChars = projectPreChar,
                    Username = Username,
                    Password = "zorunludegil"
                };

                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                try
                {
                    result = SendRequest<GetAllPointResponseModel>(address, request);
                }
                catch (Exception ex)
                {
                    result.ErrorCode = -999;
                    result.Success = false;
                    result.Description = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.ErrorCode = -888;
                result.Success = false;
                result.Description = ex.Message;
            }

            return result;
        }

        public ProcessResponseModel CancelTransaction(PointSpendingModel model)
        {
            ProcessResponseModel result = new ProcessResponseModel();

            try
            {
                var _rewardOperationsSettings = _settingService.LoadSetting<PaymentOptionSettings>();

                string address = "http://www.erhaspointbank.com/api/pointgateway/PointCancelTransaction/";
                string projectPreChar = _rewardOperationsSettings.ProjectPreChars;
                string token = _rewardOperationsSettings.ApiToken;

                if (model != null)
                {
                    model.Token = token;
                    model.ProjectPreChars = projectPreChar;
                }

                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                try
                {
                    result = SendRequest<ProcessResponseModel>(address, request);
                }
                catch (Exception ex)
                {
                    result.ErrorCode = -999;
                    result.Success = false;
                    result.Description = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.ErrorCode = -888;
                result.Success = false;
                result.Description = ex.Message;
            }

            return result;
        }
    }
}
