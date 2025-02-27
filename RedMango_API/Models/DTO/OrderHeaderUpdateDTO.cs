using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RedMango_API.Models.DTO
{
    public class OrderHeaderUpdateDTO
    {
        public int OrderHeaderId { get; set; }
        public string PickUpName { get; set; }
        public string PickUpPhoneNumber { get; set; }
        public string PickUpEmail { get; set; }

        public DateTime OrderDate { get; set; }
        public string StripePaymentIntendId { get; set; }
        public string Status { get; set; }
    }
}
