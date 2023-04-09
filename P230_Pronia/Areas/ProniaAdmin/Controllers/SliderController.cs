using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.Utilities.Extensions;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(ProniaDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Slider>slider=_context.Sliders.AsEnumerable();
            return View(slider);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Slider newSlider)
        {
            if (newSlider==null)
            {
                ModelState.AddModelError("Image","Please choose image");
                return View();
            }
            if (!newSlider.Image.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Please choose image type file");
                return View();
            }
            if ((double)newSlider.Image.Length/1024/1024>1)
            {
                ModelState.AddModelError("Image", "Please size has to be max 1MB");
                return View();
            }






            string imagesFolderPath = Path.Combine(_env.WebRootPath,"assets","images");
            newSlider.ImagePath = await newSlider.Image.CreateImage(imagesFolderPath,"website-images");
            _context.Sliders.Add(newSlider);
            _context.SaveChanges();



            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var slider = _context.Sliders.FirstOrDefault(c => c.Id == id);

            if (slider == null)
            {

                return NotFound();
            }


            return View(slider);
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var slider = _context.Sliders.FirstOrDefault(c => c.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            _context.Sliders.Remove(slider);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(s=>s.Id==id);
            if (slider is null) return BadRequest();
            return View(slider);
        }

         
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Slider edited)
        {

            if (id != edited.Id) return BadRequest();
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(slider);

            _context.Entry(slider).CurrentValues.SetValues(edited);

            if (edited.Image is not null)
            {
                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                string filePath = Path.Combine(imagesFolderPath,"website-images",slider.ImagePath);
                FileUpload.DeleteImage(filePath);
                slider.ImagePath = await edited.Image.CreateImage(imagesFolderPath,"website-images");

            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
