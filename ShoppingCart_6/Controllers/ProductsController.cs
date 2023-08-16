using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart_6.Data;
using ShoppingCart_6.Models;
using ShoppingCart_6.Services;
using ShoppingCart_6.ViewModel;

namespace ShoppingCart_6.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICartService _cartService;

        public ProductsController(AppDbContext context, IWebHostEnvironment hostingEnvironment, ICartService cartService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _cartService = cartService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userType = _context.Registers.FirstOrDefault(item => item.Id == Convert.ToInt32(userId)).UserType;
            ViewData["UserType"] = userType;

            return _context.Products != null ? 
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Products'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,ImageFile")] ProductViewModel product)
        {
            var imageUrl = "";
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var filePath = Path.Combine(imagePath, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }
                    
                    imageUrl = imageName;
                }
                var products = new Product
                {
                    Description = product.Description,
                    ImageUrl = imageUrl,
                    Name = product.Name,
                    Price = product.Price,
                    Id = product.Id
                };
                _context.Add(products);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Orders");
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,ImageFile")] ProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var imageUrl = "";
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var filePath = Path.Combine(imagePath, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = imageName;
                }

                try
                {
                    var products = new Product
                    {
                        Description = product.Description,
                        ImageUrl = imageUrl,
                        Name = product.Name,
                        Price = product.Price,
                        Id = product.Id
                    };
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(string productId)
        {

            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest("Product ID is required.");
            }

            int parsedProductId;
            if (!int.TryParse(productId, out parsedProductId))
            {
                return BadRequest("Invalid Product ID format.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == parsedProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _cartService.AddToCart(product);
             return Json(product);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return Ok();
        }
        
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
