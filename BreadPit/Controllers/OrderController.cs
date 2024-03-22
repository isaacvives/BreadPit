using BreadPit.Data;
using BreadPit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BreadPit.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly BreadPitContext _context;

        public OrderController(BreadPitContext context)
        {
            _context = context;
        }


        // GET: /Order
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            var viewModel = new OrderViewModel
            {
                Products = products,
                OrderedItems = new Dictionary<int, int>()
            };

            return View(viewModel);
        }

        // POST: /Order
        [HttpPost]
        public IActionResult Index(OrderViewModel viewModel)
        {
            var order = new Order
            {
                CustomerId = User.Identity.Name,
                OrderPlaced = DateTime.Now,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var productId in viewModel.OrderedItems.Keys)
            {
                var quantity = viewModel.OrderedItems[productId];
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    if (quantity > 0)
                    {
                        var orderDetail = new OrderDetail
                        {
                            ProductId = productId,
                            Quantity = quantity
                        };
                        order.OrderDetails.Add(orderDetail);
                    }
                }
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
