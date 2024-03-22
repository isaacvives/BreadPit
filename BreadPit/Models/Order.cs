using BreadPit.Areas.Identity.Data;

namespace BreadPit.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public BreadPitUser Customer { get; set; }
        public DateTime OrderPlaced { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
