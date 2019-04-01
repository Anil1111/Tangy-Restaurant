using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;

namespace Tangy.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db) => _db = db;

        // GET SubCategory
        public async Task<IActionResult> Index()
        {
            var subCategories = _db.SubCategories.Include(s => s.Category);

            return View(await subCategories.ToListAsync());
        }
    }
}