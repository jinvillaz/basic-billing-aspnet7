using System.ComponentModel;
using System.Runtime.Serialization;

namespace BasicBilling.API.Models
{
    public class Bill
    {
        [DefaultValue(1)]
        public int Id { get; set; }

        [DefaultValue(100)]
        public int ClientId { get; set; }

        [DefaultValue(202308)]
        public int Period { get; set; }

        [DefaultValue(100.2)]
        public decimal Amount { get; set; }
        public BillState State { get; set; }
        public BillCategory Category { get; set; }
    }

    public enum BillState
    {
        Pending = 1,
        Paid = 2,
    }

    public enum BillCategory
    {
        WATER = 1,
        ELECTRICITY = 2,
        GAS = 3,
        INTERNET = 4,
        TELEPHONY = 5,
    }
}
