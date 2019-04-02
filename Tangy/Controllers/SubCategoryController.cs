using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.SubCategoryViewModels;

namespace Tangy.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db) => _db = db;

        // GET: SubCategory
        public async Task<IActionResult> Index()
        {
            var subCategories = _db.SubCategories.Include(s => s.Category);

            return View(await subCategories.ToListAsync());
        }

        // GET: SubCategory/Create
        public IActionResult Create()
        {
            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Categories.ToList(),
                SubCategory = new SubCategory(),
                SubCategoryList = _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
            };

            return View(model);
        }

        // POST: SubCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategories.Where(s => s.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExist = _db.SubCategories.Where(s => s.Name == model.SubCategory.Name && s.CategoryID == model.SubCategory.CategoryID).Count();

                if (doesSubCategoryExist > 0 && model.IsNew)
                {
                    //error
                    StatusMessage = "Error: Sub Category Name already exists";
                }
                else
                {
                    if (doesSubCategoryExist == 0 && !model.IsNew)
                    {
                        //error
                        StatusMessage = "Error: Sub Category does not exist";
                    }
                    else
                    {
                        if(doesSubCatAndCatExist > 0)
                        {
                            //error
                            StatusMessage = "Error: Category and Sub Category combination already exists";
                        }
                        else
                        {
                            _db.Add(model.SubCategory);
                            await _db.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            var modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Categories.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }
    }
}