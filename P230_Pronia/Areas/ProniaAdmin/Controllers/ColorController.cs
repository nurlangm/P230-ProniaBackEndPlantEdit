using Microsoft.AspNetCore.Mvc;
using P230_Pronia.DAL;
using P230_Pronia.Entities;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ColorController : Controller
    {
        private readonly ProniaDbContext _context;

        public ColorController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Color> colors = _context.Colors.AsEnumerable();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Create")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Creates(Color newColor)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Name", "You cannot duplicate Color name");
                return View(newColor);
            }
            _context.Colors.Add(newColor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (color is null) return NotFound();
            return View(color);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Color edited)
        {
            if (id != edited.Id) return BadRequest();
            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (color is null) return NotFound();
            bool duplicate = _context.Colors.Any(c => c.Name == edited.Name);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate Color name");
                return View();
            }
            color.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var color = _context.Colors.FirstOrDefault(c => c.Id == id);

            if (color == null)
            {

                return NotFound();
            }


            return View(color);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var color = _context.Colors.FirstOrDefault(c => c.Id == id);

            if (color == null)
            {
                return NotFound();
            }

            _context.Colors.Remove(color);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
