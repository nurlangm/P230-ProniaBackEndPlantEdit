using Microsoft.AspNetCore.Mvc;
using P230_Pronia.DAL;
using P230_Pronia.Entities;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        private readonly ProniaDbContext _context;

        public SizeController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Size> Sizes = _context.Sizes.AsEnumerable();
            return View(Sizes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Create")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Creates(Size newSize)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Name", "You cannot duplicate Size name");
                return View(newSize);
            }
            _context.Sizes.Add(newSize);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Size Size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            if (Size is null) return NotFound();
            return View(Size);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Size edited)
        {
            if (id != edited.Id) return BadRequest();
            Size Size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            if (Size is null) return NotFound();
            bool duplicate = _context.Sizes.Any(c => c.Name == edited.Name);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate Size name");
                return View();
            }
            Size.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var Size = _context.Sizes.FirstOrDefault(c => c.Id == id);

            if (Size == null)
            {

                return NotFound();
            }


            return View(Size);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var Size = _context.Sizes.FirstOrDefault(c => c.Id == id);

            if (Size == null)
            {
                return NotFound();
            }

            _context.Sizes.Remove(Size);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
