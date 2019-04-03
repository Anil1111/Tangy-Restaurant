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
                    _db.Coupons.Add(coupon);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(coupon);
        }

        // GET: Coupon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.ID == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        // POST: Coupon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.ID)
                return NotFound();

            var couponFromDB = await _db.Coupons.Where(c => c.ID == id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
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
                        couponFromDB.Picture = picture;
                    }
                }

                couponFromDB.Name = coupon.Name;
                couponFromDB.Discount = coupon.Discount;
                couponFromDB.MinimumAmount = coupon.MinimumAmount;
                couponFromDB.CouponType = coupon.CouponType;
                couponFromDB.IsActive = coupon.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Coupon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.ID == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        // POST: Coupon/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.ID == id);

            if (coupon != null)
            {
                _db.Coupons.Remove(coupon);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}