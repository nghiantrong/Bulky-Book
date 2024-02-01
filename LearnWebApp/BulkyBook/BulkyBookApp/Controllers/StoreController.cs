using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BulkyBookApp.Data;
using BulkyBookApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBookApp.Controllers
{
    [AllowAnonymous]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Store
        public async Task<IActionResult> Index(string searchString, string minPrice, string maxPrice)
        {
            var books = _context.Books.Select(b => b);

            if (!string.IsNullOrEmpty(searchString) )
            {
                books = books.Where(b => b.Title.Contains(searchString) 
                                    || b.Author.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(minPrice) )
            {
                var min = int.Parse(minPrice);
                books = books.Where(b => b.Price >= min);
            }

            if (!string.IsNullOrEmpty(maxPrice))
            {
                var max = int.Parse(maxPrice);
                books = books.Where(b => b.Price <= max);
            }

            return _context.Books != null ? 
                          View(await books.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Books'  is null.");
        }

        // GET: Store/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
