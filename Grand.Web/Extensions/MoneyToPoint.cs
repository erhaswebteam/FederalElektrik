using Grand.Core.Infrastructure;
using Grand.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.Extensions
{
    public class MoneyToPoint
    {
        public static string CalculatePoint(decimal money)
        {
            var _settingService = EngineContext.Current.Resolve<ISettingService>();
            var setting = _settingService.GetSetting("payment.moneypointfactor");
            decimal result = money / decimal.Parse(setting.Value);
            return result.ToString("F");
        }

    }
}
