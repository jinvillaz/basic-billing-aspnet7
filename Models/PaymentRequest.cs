using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BasicBilling.API.Models
{
    public class PaymentRequest
    {
        [DefaultValue(100)]
        [Required(ErrorMessage = "The 'clientId' field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The 'clientId' field must be a number and greater than 0")]
        public int ClientId { get; set; }

        [DefaultValue(202308)]
        [Required(ErrorMessage = "The 'period' field is required")]
        [Range(100001, 999912, ErrorMessage = "The 'period' field must be a number in YYYYMM format")]

        public int Period { get; set; }

        [DefaultValue(BillCategory.WATER)]
        [Required(ErrorMessage = "The 'category' field is required")]

        public string Category { get; set; }
    }
}
