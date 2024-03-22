using BreadPit.Models;
using Microsoft.EntityFrameworkCore;

namespace BreadPit.Data
{
    public class OrderContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
