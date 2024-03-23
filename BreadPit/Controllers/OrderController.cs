using BreadPit.Data;
using BreadPit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BreadPit.Controllers
{
    public class OrderController : Controller
    {
        private readonly BreadPitContext _context;

        public OrderController(BreadPitContext context)
        {
            _context = context;
        }


        // GET: /Order
        [Authorize]
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
        [Authorize]
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

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult SimpleOverview()
        {
            var orders = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).ToList();

            var orderedItems = new Dictionary<int, int>();

            foreach (var order in orders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (!orderedItems.ContainsKey(detail.ProductId))
                    {
                        orderedItems.Add(detail.ProductId, detail.Quantity);
                    }
                    else
                    {
                        orderedItems[detail.ProductId] += detail.Quantity;
                    }
                }
            }

            var orderViewModel = new OrderViewModel
            {
                Products = orderedItems.Select(kvp =>
                    new Product { Id = kvp.Key, Name = _context.Products.Find(kvp.Key).Name }).ToList(),
                OrderedItems = orderedItems
            };

            return View(orderViewModel);
        }
    }
}
