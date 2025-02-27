using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RedMango_API.Models.DTO
{
    public class OrderHeaderCreateDTO
    {
        [Required]
        public string PickUpName { get; set; }
        [Required]
        public string PickUpPhoneNumber { get; set; }
        [Required]
        public string PickUpEmail { get; set; }

        public string ApplicationUserId { get; set; }
        public double OrderTotal { get; set; }

        public string StripePaymentIntendId { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }

        public IEnumerable<OrderDetailCreateDTO> OrderDetailDTO { get; set; }
    }
}
