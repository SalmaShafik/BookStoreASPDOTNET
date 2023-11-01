using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1Psight.Data;
using WebApplication1Psight.Models;

namespace WebApplication1Psight.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBcontext _db;

        //dependency injection
        public CategoryController(AppDBcontext db) //a constructor for the CategoryController class, which takes an instance of the AppDBcontext class as a parameter. The constructor initializes a private field _db with the provided db object.
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);

        }
        //public IActionResult Index(int? id)
        //{ //retrieve all records in categories table
        //    IEnumerable<Category> objCategoryList= _db.Categories.Include(c=>c.Products);
        //    if (id!=null)
        //        objCategoryList= _db.Categories.Where(c => c.Id == id).Include(c => c.Products);

        //    return View(objCategoryList);
        //}

        //GET
        public IActionResult Create()
        { //get category
            return View();
        }


        ////POST
        [HttpPost]
        [ValidateAntiForgeryToken] //injects key for validation
        public IActionResult Create(Category obj)
        { //post category

            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The display order cannot exactly match the name ");
            //}
            if (ModelState.IsValid) //check if input follows validation rules applied on model
            { //Server side validation in controller level
                _db.Categories.Add(obj);
                _db.SaveChanges(); //saves all changes made on DB
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index"); //Go back to index
            }
            return View(obj); //if not return view with the object only 
        }

        //GET
        public IActionResult Edit(int?id)
        { //edit category
            if(id==null || id == 0)
            {
                return NotFound();
            }
            //3 ways to retrieve data from database using id
            var categoryFromDb = _db.Categories.Find(id);
            //var categoryFromDbFirst = _db.Categories.FirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] //injects key for validation
        public IActionResult Edit(Category obj)
        { //edit category

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The display order cannot exactly match the name ");
            }
            if (ModelState.IsValid) //check if input follows validation rules applied on model
            { //Server side validation in controller level
                _db.Categories.Update(obj);
                _db.SaveChanges(); //saves all changes made on DB
                TempData["success"] = "Category upated successfully";

                return RedirectToAction("Index"); //Go back to index
            }
            return View(obj); //if not return view with the object only 
        }
        public IActionResult Delete(int? id)
        {  //retrieve id then delete it 
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //3 ways to retrieve data from database using id
            var categoryFromDb = _db.Categories.Find(id);
            //var categoryFromDbFirst = _db.Categories.FirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //injects key for validation
        public IActionResult DeletePOST (int? id)
        { // delete category
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
                _db.Categories.Remove(obj);
                _db.SaveChanges(); //saves all changes made on DB
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index"); //Go back to index
            
        }


    }

}
