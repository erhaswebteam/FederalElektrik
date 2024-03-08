using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Grand.Services.Messages
{
    public class PostAPIData
    {
        public bool SendSMS(string url, SendSMS_RequestModel model)
        {
            string request = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return SendRequest<bool>(url, request);
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
    }
}
