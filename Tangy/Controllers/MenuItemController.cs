using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.MenuItemViewModels;
using Tangy.Utility;

namespace Tangy.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Categories = _db.Categories.ToList(),
                MenuItem = new MenuItem()
            };
        }

        // GET: MenuItem
        public async Task<IActionResult> Index()
        {
            var menuItems = _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await menuItems.ToListAsync());
        }

        // GET: MenuItem/Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        // POST: MenuItem/Create
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryID = int.Parse(Request.Form["SubCategoryID"].ToString());

            if (!ModelState.IsValid)
                return View(MenuItemVM);

            _db.MenuItems.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItems.Find(MenuItemVM.MenuItem.ID);

            if (files[0] != null && files[0].Length > 0)
            {
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf('.'), files[0].FileName.Length - files[0].FileName.LastIndexOf('.'));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.ID + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.ID + extension;
            }
            else
            {
                //when user does not upload an image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.ID + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.ID + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public JsonResult GetSubCategory(int categoryID)
        {
            var subCategoryList = new List<SubCategory>();
            subCategoryList = (from subCategory in _db.SubCategories
                               where subCategory.CategoryID == categoryID
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "ID", "Name"));
        }

        // GET: MenuItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ID == id);
            MenuItemVM.SubCategories = _db.SubCategories.Where(s => s.CategoryID == MenuItemVM.MenuItem.CategoryID).ToList();

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        // POST: MenuItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemVM.MenuItem.SubCategoryID = int.Parse(Request.Form["SubCategoryID"].ToString());

            if (id != MenuItemVM.MenuItem.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _db.MenuItems.Where(m => m.ID == MenuItemVM.MenuItem.ID).FirstOrDefault();

                    if (files[0].Length > 0 && files[0] != null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");

                        var newExtension = files[0].FileName.Substring(files[0].FileName.LastIndexOf('.'), files[0].FileName.Length - files[0].FileName.LastIndexOf('.'));
                        var oldExtension = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf('.'), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf('.'));

                        if (System.IO.File.Exists(Path.Combine(uploads, MenuItemVM.MenuItem.ID + oldExtension)))
                            System.IO.File.Delete(Path.Combine(uploads, MenuItemVM.MenuItem.ID + oldExtension));

                        using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.ID + newExtension), FileMode.Create))
                            files[0].CopyTo(filestream);

                        MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.ID + newExtension;
                    }

                    if (MenuItemVM.MenuItem.Image != null)
                        menuItemFromDb.Image = MenuItemVM.MenuItem.Image;

                    menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                    menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                    menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                    menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;
                    menuItemFromDb.CategoryID = MenuItemVM.MenuItem.CategoryID;
                    menuItemFromDb.SubCategoryID = MenuItemVM.MenuItem.SubCategoryID;

                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategories = _db.SubCategories.Where(s => s.CategoryID == MenuItemVM.MenuItem.CategoryID).ToList();
            return View(MenuItemVM);
        }

        // GET: MenuItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ID == id);

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }
    }
}