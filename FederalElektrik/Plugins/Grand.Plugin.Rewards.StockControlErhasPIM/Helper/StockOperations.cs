using Amazon.Runtime.Internal.Util;
using Grand.Core;
using Grand.Core.Domain.Localization;
using Grand.Core.Plugins;
using Grand.Plugin.Rewards.StockControlErhasPIM.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Stores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Grand.Plugin.Rewards.StockControlErhasPIM.Helper
{
    public class StockOperations : BasePlugin
    {
        private readonly ISettingService _settingService;

        public StockOperations(
            ISettingService settingService)
        {
            this._settingService = settingService;
        }

        /// <summary>
        /// Kullanıcı adı ve şifreye göre kullanıcı bilgilerini getiren fonksiyon...
        /// </summary>
        /// <returns></returns>
        public int GetStockQuantityWithSku(string sku)
        {
            int result = 0;

            try
            {
                var _stockOperationsSettings = _settingService.LoadSetting<StockOperationsSettings>();

                string address = _stockOperationsSettings.StockControlErhasPIMGetSkuQuantityAddress;
                string token = _stockOperationsSettings.StockControlErhasPIMToken;

                StockQuantityRequest model = new StockQuantityRequest();
                model.token = token;
                model.Sku = sku;
                
                string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                try
                {
                    result = SendRequest<int>(address, request);
                }
                catch (Exception ex)
                {
                    return -999;
                }
            }
            catch (Exception ex)
            {
                return -888;
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
            request.Method = "GET";
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
    }
}
