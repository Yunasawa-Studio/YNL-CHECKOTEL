using System;
using System.Collections.Generic;

namespace YNL.Checkotel
{
    public class VoucherContainer
    {
        public Dictionary<UID, Voucher> Vouchers = new();           // Key is ID of Voucher
    }

    public class Voucher
    {
        public UID ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsPercentage { get; set; } = false;   
        public decimal Value { get; set; } = 0m;          
        public DateTime ExpirationDate { get; set; } = DateTime.MaxValue;
        public bool IsUsed { get; set; } = false;

        public bool IsValid()
        {
            return !IsUsed && DateTime.Now <= ExpirationDate;
        }

        public decimal ApplyDiscount(decimal originalPrice)
        {
            if (!IsValid()) return originalPrice;

            decimal discount = IsPercentage
                ? originalPrice * (Value / 100)
                : Value;

            return Math.Max(originalPrice - discount, 0);
        }
    }

}
