using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1Psight.Data;
using WebApplication1Psight.Models;
using WebApplication1Psight.ViewModel;
using PagedList;

namespace WebApplication1Psight.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDBcontext _context;

        public ProductsController(AppDBcontext context)
        {
            _context = context;
        }
        public ActionResult Index(int? categoryId, int? page)
        {
            int pageSize = 10; // Number of products to display per page
            int pageNumber = (page ?? 1);

            // Retrieve categories for dropdown list
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            // Query products based on selected category (if categoryId is provided)
            var query = _context.Products.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Calculate the number of products to skip based on the current page and page size
            int productsToSkip = (pageNumber - 1) * pageSize;

            // Retrieve only the necessary products from the database using Skip and Take
            var products = query.OrderBy(p => p.Id) // Order by a unique property like Id to ensure consistent ordering
                                .Skip(productsToSkip)
                                .Take(pageSize)
                                .ToList();

            // Get the total count of products (for pagination)
            var totalProducts = query.Count();

            // Create a StaticPagedList instance for pagination
            var pagedProducts = new StaticPagedList<Product>(products, pageNumber, pageSize, totalProducts);

            return View(pagedProducts);
        }
        //For the dropdownLIST of categories
        //public ActionResult Index(int? categoryId)
        //{
        //    var categories = _context.Categories.ToList();
        //    ViewBag.Categories = new SelectList(categories, "Id", "Name");

        //    IQueryable<Product> products = _context.Products.Include(p => p.Category);


        //    if (categoryId.HasValue)
        //    {
        //        products = products.Where(p => p.CategoryId == categoryId.Value);
        //    }

        //    return View(products.ToList());

        //}
        //GET Categories
        private List<Category> GetCategories()
        {
            // Retrieve categories from your database or any data source
            List<Category> categories = _context.Categories.ToList();
            if (categories == null)
            {
                categories = new List<Category>(); // Initialize an empty list to avoid null reference
            }

            return categories;
         
       
        }
        //GET: Product
      

        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        ////POST
        //[HttpPost]
        //[ValidateAntiForgeryToken] //injects key for validation
        //public IActionResult Create(Product obj)
        //{ 
        //    if (ModelState.IsValid) //check if input follows validation rules applied on model
        //    { //Server side validation in controller level
        //        _context.Products.Add(obj);
        //        _context.SaveChanges(); //saves all changes made on DB
        //        TempData["success"] = "Product created successfully";
        //        return RedirectToAction("Index"); //Go back to index
        //    }
        //    return View(obj); //if not return view with the object only 
        //}


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                    .Include(p => p.Category) // Eager load the Category navigation property

                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        //GET
        public IActionResult Edit(int? id)
        { //edit product
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            if (id == null || id == 0)
            {
                return NotFound();
            }
            //3 ways to retrieve data from database using id
            var productFromDb = _context.Products.Find(id);
            //var categoryFromDbFirst = _db.Categories.FirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] //injects key for validation
        public IActionResult Edit(Product obj)
        { //edit product
           
            //if (obj.Name == obj.Id.ToString())
            //{
            //    ModelState.AddModelError("name", "The display order cannot exactly match the name ");
            //}

            if (ModelState.IsValid) //check if input follows validation rules applied on model
            { //Server side validation in controller level
                _context.Products.Update(obj);
                _context.SaveChanges(); //saves all changes made on DB
                TempData["success"] = "Product upated successfully";

                return RedirectToAction("Index"); //Go back to index
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", obj.CategoryId);

            return View(obj); //if not return view with the object only 
        }

        //DELETE PRODUCT
        public IActionResult Delete(int? id)
        {  //retrieve id then delete it 
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //3 ways to retrieve data from database using id
            var productFromDb = _context.Products.Find(id);
            //var categoryFromDbFirst = _db.Categories.FirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //injects key for validation
        public IActionResult DeletePOST(int? id)
        { // delete product
            var obj = _context.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _context.Products.Remove(obj);
            _context.SaveChanges(); //saves all changes made on DB
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index"); //Go back to index

        }
        ////Pagination
        //public IActionResult Index2(int? page)
        //{
        //    int pageSize = 10; // Number of items per page
        //    int pageNumber = (page ?? 1); // If no page number is specified, default to page 1

        //    var data = _context.(); // Replace with your data retrieval logic

        //    var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        //    return View(paginatedData);
        //}
    }
}

        //public ActionResult Index()
        //{
        //    var categories = _context.Categories.ToList();
        //    ViewBag.Categories = new SelectList(categories, "Id", "Name");
        //    return View();
        //}

        //public ActionResult ProductsByCategory(int categoryId)
        //{
        //    var category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == categoryId);
        //    return View(category.Products);
        //}
    

    //GET Product by Category.ID

    //public ActionResult ListByCategory(int categoryId)
    //    {
    //        var products = _context.Products.Where(p => p.CategoryId == categoryId).ToList();
    //        return View(products);
    //    }
    //    // GET: Products

    //    public async Task<IActionResult> Index()
    //    {
    //          return _context.Products != null ? 
    //                      View(await _context.Products.ToListAsync()) :
    //                      Problem("Entity set 'AppDBcontext.Products'  is null.");
    //    }
    //    [HttpPost]
    //    public async Task<IActionResult> Index(int categoryId)
    //    {
    //        return _context.Products != null ?
    //                    View(await _context.Products.Where(p=>p.CategoryId==categoryId).ToListAsync()) :
    //                    Problem("Entity set 'AppDBcontext.Products'  is null.");
    //    }








    // GET: Products/Edit/5
    //public async Task<IActionResult> Edit(int? id)
    //{
    //    if (id == null || _context.Products == null)
    //    {
    //        return NotFound();
    //    }

    //    var product = await _context.Products.FindAsync(id);
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(product);
    //}

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Quantity,CategoryRefId")] Product product)
    //{
    //    if (id != product.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(product);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!ProductExists(product.Id))
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
    //    return View(product);
    //}

    //// GET: Products/Delete/5
    //public async Task<IActionResult> Delete(int? id)
    //{
    //    if (id == null || _context.Products == null)
    //    {
    //        return NotFound();
    //    }

    //    var product = await _context.Products
    //        .FirstOrDefaultAsync(m => m.Id == id);
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(product);
    //}

    //// POST: Products/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int id)
    //{
    //    if (_context.Products == null)
    //    {
    //        return Problem("Entity set 'AppDBcontext.Products'  is null.");
    //    }
    //    var product = await _context.Products.FindAsync(id);
    //    if (product != null)
    //    {
    //        _context.Products.Remove(product);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    //private bool ProductExists(int id)
    //{
    //  return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
    //}

