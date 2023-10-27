using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Rewards.RewardOperations.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal ExpendablePointTotal { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserDataRequestModel
    {
        public string ProjectPreChars { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class PointSpendingModel
    {
        public string Token { get; set; }
        public string ProjectPreChars { get; set; }
        public string Username { get; set; }
        public string FirstProcessId { get; set; }
        public decimal OrderPoint { get; set; }
        public List<OrderItemPointModel> OrderItems { get; set; }
    }

    public class OrderItemPointModel
    {
        public string SecondaryProcessId { get; set; }
        public decimal OrderItemPoint { get; set; }
        public string SupplierProcessId { get; set; }
        public string Message { get; set; }
    }

    public class ProcessResponseModel
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
    }

    public class GeneralValues
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityValue { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GetCustomerResponseModel
    {
        public GetCustomerResponseModel()
        {
            Success = false;
        }

        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public Customer Customer { get; set; }
        public List<GeneralValues> GeneralValues { get; set; }
    }

    public class SetUserPasswordReq
    {
        public string Token { get; set; }
        public string ProjectPreChars { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SetUserPasswordRes
    {
        public SetUserPasswordRes()
        {
            Success = false;
            ErrorCode = -999;
            Description = "unknown data";
        }

        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
    }



    public class GetAllPointResponseModel
    {
        public GetAllPointResponseModel()
        {
            Success = false;
            ErrorCode = 999;
            Description = "unknown data";
            TransactionId = string.Empty;
        }

        public class PointDetail
        {
            public string ProcessTypeName { get; set; }
            public string PointTypeName { get; set; }
            public decimal Point { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ExpiredDate { get; set; }
            public string Message { get; set; }
        }

        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public Customer Customer { get; set; }
        public List<PointDetail> PointDetails { get; set; }
    }
}
