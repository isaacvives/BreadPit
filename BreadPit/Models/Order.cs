using BreadPit.Areas.Identity.Data;

namespace BreadPit.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustumerId { get; set; }
        public BreadPitUser User { get; set; }
        public DateTime OrderPlaced { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
