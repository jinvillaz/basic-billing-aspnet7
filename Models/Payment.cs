using System.ComponentModel;

namespace BasicBilling.API.Models
{
    public class Payment
    {
        [DefaultValue(1)]
        public int Id { get; set; }

        [DefaultValue(100)]
        public int ClientId { get; set; }
        
        [DefaultValue(1)]
        public int BillId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
