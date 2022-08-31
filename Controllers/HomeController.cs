using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("products")]
        public IActionResult Index()
        {
            ViewBag.AllProducts = _context.Products.ToList();
            return View();
        }

        [HttpPost("products/create")]
        public IActionResult AddProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AllProducts = _context.Products.ToList();
            return View("Index");
        }

        [HttpGet("products/{productId}")]
        public IActionResult OneProduct(int productId)
        {
            Product OneProduct = _context.Products.Include(a => a.AssociatedWithin).ThenInclude(c => c.Category).FirstOrDefault(p => p.ProductId == productId);
            ViewBag.AllCategories = _context.Categories.ToList();
            return View(OneProduct);
        }

        [HttpGet("categories/{categoryId}")]
        public IActionResult OneCategory(int categoryId)
        {
            Category OneCategory = _context.Categories.Include(a => a.ProductsWithin).ThenInclude(p => p.Product).FirstOrDefault(c => c.CategoryId == categoryId);
            ViewBag.AllProducts = _context.Products.ToList();
            return View(OneCategory);
        }

        [HttpPost("categories/create")]
        public IActionResult AddCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(newCategory);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            ViewBag.AllCategories = _context.Categories.ToList();
            return View("Categories");
        }

        [HttpPost("addCategoryTo")]
        public IActionResult AddCategoryTo(Association newAssociation)
        {
            if (newAssociation.CategoryId == 0)
            {
                Product OneProduct = _context.Products.Include(a => a.AssociatedWithin).ThenInclude(c => c.Category).FirstOrDefault(p => p.ProductId == newAssociation.ProductId);
                ViewBag.AllCategories = _context.Categories.ToList();
                return View("OneProduct", OneProduct);
            }
            _context.Associations.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/products/{newAssociation.ProductId}");
        }

        [HttpPost("addProductTo")]
        public IActionResult AddProductTo(Association newAssociation)
        {
            if (newAssociation.ProductId == 0)
            {
                Category OneCategory = _context.Categories.Include(a => a.ProductsWithin).ThenInclude(p => p.Product).FirstOrDefault(c => c.CategoryId == newAssociation.CategoryId);
                ViewBag.AllProducts = _context.Products.ToList();
                return View("OneCategory", OneCategory);
            }
            _context.Associations.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/categories/{newAssociation.CategoryId}");
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            ViewBag.AllCategories = _context.Categories.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
