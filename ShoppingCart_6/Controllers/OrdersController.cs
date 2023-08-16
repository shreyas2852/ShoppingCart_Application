using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart_6.Data;
using ShoppingCart_6.Models;
using ShoppingCart_6.Services;
using ShoppingCart_6.ViewModel;

namespace ShoppingCart_6.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public OrdersController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }


        // GET: Orders
        [NoDirectAccess]
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("OrderPlacedMessage"))
            {
                ViewBag.OrderPlacedMessage = TempData["OrderPlacedMessage"] as string;
            }

            var appDbContext = _context.Orders.Include(o => o.Product).Include(o => o.Register);

            return View(await appDbContext.ToListAsync());

        }

        //GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
           
            var order = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Register)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var register = await _context.Registers.FindAsync(order.RegisterId);
            if(register == null)
            {
                return NotFound();
            }

            ViewData["username"] = register?.UserName;

            var Products = await _context.Products.FindAsync(order.ProductId);
            if (Products == null)
            {
                return NotFound();
            }

            ViewData["ProductName"] = Products?.Name;

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var cartItems = _cartService.GetCartItems().ToList();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["RegisterId"] = userId;

            int userType = _context.Registers.FirstOrDefault(item => item.Id == Convert.ToInt32(userId)).UserType;
            ViewData["UserType"] = userType;

            if (cartItems != null)
            {
                var productIds = cartItems[0].Id;
                ViewData["ProductIds"] = productIds;

                ViewData["CartItems"] = cartItems;
            }
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegisterId,ProductId,Quantity,Price,ShippingAddress,PaymentMethod,Status")] OrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                var orders = new Order
                {
                   Id = order.Id,
                   RegisterId = order.RegisterId,
                   ProductId = order.ProductId,
                   Quantity = order.Quantity,
                   Price =  order.Price,
                   ShippingAddress = order.ShippingAddress,
                   PaymentMethodType    = order.PaymentMethodType,
                   Status   = order.Status

                };

                _context.Add(orders);
                await _context.SaveChangesAsync();

                TempData["OrderPlacedMessage"] = "Your order is placed";
                return RedirectToAction(nameof(Index));
            }
           

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", order.ProductId);
            ViewData["RegisterId"] = new SelectList(_context.Registers, "Id", "Id", order.RegisterId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Register)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var register = await _context.Registers.FindAsync(order.RegisterId);
            if (register == null)
            {
                return NotFound();
            }

            ViewData["username"] = register?.UserName;

            var Products = await _context.Products.FindAsync(order.ProductId);
            if (Products == null)
            {
                return NotFound();
            }

            ViewData["ProductName"] = Products?.Name;
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult PlaceOrder()
        {
            var cartItems = _cartService.GetCartItems().ToList();
            return View(cartItems);
        }

        // GET: Orders/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Orders == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", order.ProductId);
        //    ViewData["RegisterId"] = new SelectList(_context.Registers, "Id", "Id", order.RegisterId);
        //    return View(order);
        //}

        // POST: Orders/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,Quantity,Price,ShippingAddress,PaymentMethod,Status")] OrderViewModel order)
        //{
        //    if (id != order.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var orders = new Order
        //            {
        //                Id = order.Id,
        //                RegisterId = order.RegisterId,
        //                ProductId = order.ProductId,
        //                Quantity = order.Quantity,
        //                Price = order.Price,
        //                ShippingAddress = order.ShippingAddress,
        //                PaymentMethodType = order.PaymentMethodType,
        //                Status = order.Status

        //            };
        //            _context.Update(orders);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", order.ProductId);
        //    ViewData["RegisterId"] = new SelectList(_context.Registers, "Id", "Id", order.RegisterId);
        //    return View(order);
        //}


    }
}
