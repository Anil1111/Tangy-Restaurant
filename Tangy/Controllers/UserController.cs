using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models.AccountModels;
using Tangy.Utility;

namespace Tangy.Controllers
{
    //[Authorize(Roles = SD.AdminEndUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userList = _db.Users.Where(u => u.Id != currentUser.Id).ToList();

            return View(userList);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUser user)
        {
            if (user.Id != id)
                return BadRequest();

            var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;

            if (_db.Users.Where(u => u.UserName == user.UserName).Count() == 0)
                dbUser.UserName = user.UserName;

            if (_db.Users.Where(u => u.Email == user.Email).Count() == 0)
                dbUser.Email = user.Email;

            dbUser.PhoneNumber = user.PhoneNumber;
            dbUser.LockoutEnd = user.LockoutEnd;
            dbUser.LockoutReason = user.LockoutReason;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id, AppUser user)
        {
            if (id == null)
                return NotFound();

            if (user.LockoutEnd < DateTimeOffset.Now)
                return View(user);

            var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest();

            dbUser.LockoutReason = user.LockoutReason;

            if (user.LockoutEnd == null)
                return View(dbUser);

            dbUser.LockoutEnd = user.LockoutEnd;
            await _db.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Edit), routeValues: new { id });
        }

        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest();

            user.LockoutEnd = null;
            user.LockoutReason = null;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}