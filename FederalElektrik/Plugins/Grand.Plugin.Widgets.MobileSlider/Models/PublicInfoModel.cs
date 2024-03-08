using Grand.Framework.Mvc.Models;
using System.Collections.Generic;

namespace Grand.Plugin.Widgets.MobileSlider.Models
{
    public class PublicInfoModel : BaseGrandModel
    {
        public PublicInfoModel()
        {
            Slide = new List<MobileSlider>();
        }
        public IList<MobileSlider> Slide { get; set; }

        public class MobileSlider
        {
            public string PictureUrl { get; set; }
            public string Text { get; set; }
            public string Link { get; set; }
            public string CssClass { get; set; }

        }
    }
}