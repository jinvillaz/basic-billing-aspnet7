using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BasicBilling.API.Models
{
    public class BillRequest
    {
        [DefaultValue(202308)]
        [Required(ErrorMessage = "The 'period' field is required")]
        [Range(100001, 999912, ErrorMessage = "The 'period' field must be a number in YYYYMM format")]
        public int Period { get; set; }

        [DefaultValue(BillCategory.WATER)]
        [Required(ErrorMessage = "The 'category' field is required")]
        public string Category { get; set; }
    }
}
