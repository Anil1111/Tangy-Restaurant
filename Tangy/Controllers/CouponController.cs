using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;

namespace Tangy.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Coupon
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupons.ToListAsync());
        }

        // GET: Coupon/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] picture = null;
                    using (var fileStream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            picture = memoryStream.ToArray();
                        }
                    }
                    coupon.Picture = picture;
                }

                _db.Coupons.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}