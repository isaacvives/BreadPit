using BreadPit.Areas.Identity.Data;
using System.Collections.Generic;
namespace BreadPit.Models
{
    public class OrderViewModel
    {
        public List<Product> Products { get; set; }
        public List<BreadPitUser> Users { get; set; }
        public Dictionary<int, int> OrderedItems { get; set; } // Key: ProductID, Value: Quantity
        public double TotalPrice { get; set; }
        public int OrderId { get; set; }
    }
}
