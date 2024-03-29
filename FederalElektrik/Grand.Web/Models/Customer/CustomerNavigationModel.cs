﻿using Grand.Framework.Mvc.Models;

namespace Grand.Web.Models.Customer
{
    public partial class CustomerNavigationModel : BaseGrandModel
    {
        public bool HideInfo { get; set; }
        public bool HideAddresses { get; set; }
        public bool HideOrders { get; set; }
        public bool HideBackInStockSubscriptions { get; set; }
        public bool HideReturnRequests { get; set; }
        public bool HideDownloadableProducts { get; set; }
        public bool HideRewardPoints { get; set; }
        public bool HideChangePassword { get; set; }
        public bool HideDeleteAccount { get; set; }
        public bool HideAvatar { get; set; }
        public bool HideAuctions { get; set; }
        public bool HideForumSubscriptions { get; set; }
        public bool ShowVendorInfo { get; set; }

        public bool HideMyRegisterDealer { get; set; }

        public CustomerNavigationEnum SelectedTab { get; set; }
    }

    public enum CustomerNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        Orders = 20,
        BackInStockSubscriptions = 30,
        ReturnRequests = 40,
        DownloadableProducts = 50,
        RewardPoints = 60,
        ChangePassword = 70,
        DeleteAccount = 75,
        Avatar = 80,
        ForumSubscriptions = 90,
        VendorInfo = 100,
        Auctions = 110,
        ProcessHistory = 120,
        ProductQuota = 130,
        PointTransfer = 140,
        Register = 150,
        MyRegisterDealer = 160
    }
}