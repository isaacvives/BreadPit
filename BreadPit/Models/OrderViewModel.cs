using System.Collections.Generic;
namespace BreadPit.Models
{
    public class OrderViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<int, int> OrderedItems { get; set; } // Key: SandwichID, Value: Quantity
    }
}
