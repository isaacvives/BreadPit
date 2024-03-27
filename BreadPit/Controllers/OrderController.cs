using BreadPit.Areas.Identity.Data;
using BreadPit.Data;
using BreadPit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BreadPit.Controllers
{
    public class OrderController : Controller
    {
        private readonly BreadPitContext _context;
        private readonly UserManager<BreadPitUser> _userManager;


        public OrderController(BreadPitContext context, UserManager<BreadPitUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: /Order
        [Authorize(Roles = "Admin, Manager, User")]
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
        [Authorize(Roles = "Admin, Manager, User")]
        public IActionResult Index(OrderViewModel viewModel)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var order = new Order
            {
                CustomerId = currentUser.Id,
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PersonalOverviewList()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            var orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderedItems = order.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(od => od.Quantity));

                var totalPrice = order.OrderDetails.Sum(od => od.Quantity * od.Product.Price);

                var orderViewModel = new OrderViewModel
                {
                    Products = order.OrderDetails.Select(od => od.Product).ToList(),
                    Users = new List<BreadPitUser> { order.Customer },
                    OrderedItems = orderedItems,
                    TotalPrice = totalPrice,
                    OrderId = order.Id
                };

                orderViewModels.Add(orderViewModel);
            }

            return View(orderViewModels);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("PersonalOverviewList");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderedItems = order.OrderDetails.ToDictionary(od => od.ProductId, od => od.Quantity);

            var orderViewModel = new OrderViewModel
            {
                Products = order.OrderDetails.Select(od => od.Product).ToList(),
                OrderedItems = orderedItems,
                TotalPrice = order.OrderDetails.Sum(od => od.Quantity * od.Product.Price),
                OrderId = order.Id
            };

            return View(orderViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditOrder(int id, OrderViewModel viewModel)
        {
            if (id != viewModel.OrderId)
            {
                return BadRequest();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var detail in order.OrderDetails)
            {
                if (viewModel.OrderedItems.TryGetValue(detail.ProductId, out var quantity))
                {
                    detail.Quantity = quantity;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("PersonalOverviewList");
        }

    }
}
