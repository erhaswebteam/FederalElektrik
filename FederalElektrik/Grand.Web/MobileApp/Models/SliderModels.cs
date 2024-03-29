﻿using Grand.Core.Configuration;

namespace Grand.Web.MobileApp.Models
{
    public class MobileSlider
    {
        public string PictureUrl { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class MobileSliderSettings : ISettings
    {
        public string Picture1Id { get; set; }
        public string Text1 { get; set; }
        public string Link1 { get; set; }

        public string Picture2Id { get; set; }
        public string Text2 { get; set; }
        public string Link2 { get; set; }

        public string Picture3Id { get; set; }
        public string Text3 { get; set; }
        public string Link3 { get; set; }

        public string Picture4Id { get; set; }
        public string Text4 { get; set; }
        public string Link4 { get; set; }

        public string Picture5Id { get; set; }
        public string Text5 { get; set; }
        public string Link5 { get; set; }
    }
}
