using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.Utilities.Extensions;
using P230_Pronia.ViewModels;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class PlantsController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PlantsController(ProniaDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Plant> model = _context.Plants.Include(p => p.PlantImages)
                                                        .Include(p => p.PlantSizeColors).ThenInclude(p => p.Size)
                                                        .Include(p => p.PlantSizeColors).ThenInclude(p => p.Color)
                                                        .AsNoTracking().AsEnumerable();






            //        IEnumerable<Plant> model = _context.Plants
            //.Include(p => p.PlantImages)
            //.Include(p => p.PlantSizeColors).ThenInclude(psc => psc.Size)
            //.Include(p => p.PlantSizeColors).ThenInclude(psc => psc.Color)
            //.Select(p => new Plant
            //{
            //    PlantSizeColors = p.PlantSizeColors.Select(psc => new PlantSizeColor
            //    {

            //        Size = psc.Size,
            //        Color = psc.Color,
            //        Quantity = psc.Quantity
            //    }).ToList()
            //})
            //.AsNoTracking()
            //.AsEnumerable();




            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Color=_context.Colors.AsEnumerable();
            ViewBag.Size = _context.Sizes.AsEnumerable();
            


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlantVM newPlant)
        {



            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Color = _context.Colors.AsEnumerable();
            ViewBag.Size = _context.Sizes.AsEnumerable();



            TempData["InvalidImages"] = string.Empty;
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!newPlant.HoverPhoto.IsValidFile("image/") || !newPlant.MainPhoto.IsValidFile("image/"))
            {
                ModelState.AddModelError(string.Empty, "Please choose image file");
                return View();
            }
            if (!newPlant.HoverPhoto.IsValidLength(1) || !newPlant.MainPhoto.IsValidLength(1))
            {
                ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 1MB");
                return View();
            }

            Plant plant = new()
            {
                Name = newPlant.Name,
                Desc = newPlant.Desc,
                Price = newPlant.Price,
                SKU = newPlant.SKU,                
                PlantDeliveryInformationId = newPlant.PlantDeliveryInformationId,


            };
            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            foreach (var image in newPlant.Images)
            {
                if (!image.IsValidFile("image/") || !image.IsValidLength(1))
                {
                    TempData["InvalidImages"] += image.FileName;
                    continue;
                }
                PlantImage plantImage = new()
                {
                    IsMain = false,
                    Path = await image.CreateImage(imageFolderPath, "website-images")
                };
                plant.PlantImages.Add(plantImage);
            }





            PlantImage main = new()
            {
                IsMain = true,
                Path = await newPlant.MainPhoto.CreateImage(imageFolderPath, "website-images")
            };
            plant.PlantImages.Add(main);
            PlantImage hover = new()
            {
                IsMain = null,
                Path = await newPlant.HoverPhoto.CreateImage(imageFolderPath, "website-images")
            };
            plant.PlantImages.Add(hover);

            foreach (int id in newPlant.CategoryIds)
            {
                PlantCategory category = new()
                {
                    CategoryId = id
                };
                plant.PlantCategories.Add(category);
            }
            foreach (int id in newPlant.TagIds)
            {
                PlantTag tag = new()
                {
                    TagId = id
                };
                plant.PlantTags.Add(tag);
            }




            plant.PlantSizeColors.RemoveAll(psc => newPlant.ColorIds.Contains(psc.ColorId) || newPlant.SizeIds.Contains(psc.SizeId));

            foreach (var colorId in newPlant.ColorIds)
            {
                foreach (var sizeId in newPlant.SizeIds)
                {

                    var newPlantSizeColor = new PlantSizeColor
                    {
                        ColorId = colorId,
                        SizeId = sizeId,
                        Quantity=newPlant.Quantity
                    };
                    plant.PlantSizeColors.Add(newPlantSizeColor);
                }
            }







            _context.Plants.Add(plant);
            _context.SaveChanges();
            return RedirectToAction("Index", "Plants");
            
        }


        public IActionResult Edit(int id)
        {
            if (id == 0) return BadRequest();
            PlantVM? model = EditedPlant(id);

            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Color = _context.Colors.AsEnumerable();
            ViewBag.Size = _context.Sizes.AsEnumerable();
            if (model is null) return BadRequest();
            _context.SaveChanges();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PlantVM edited)
        {
            ViewBag.Informations = _context.PlantDeliveryInformation.AsEnumerable();
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Tags = _context.Tags.AsEnumerable();
            ViewBag.Color = _context.Colors.AsEnumerable();
            ViewBag.Size = _context.Sizes.AsEnumerable();
            PlantVM? model = EditedPlant(id);

            Plant? plant = await _context.Plants.Include(p => p.PlantImages).FirstOrDefaultAsync(p => p.Id == id);
            if (plant is null) return BadRequest();

            IEnumerable<string> removables = plant.PlantImages.Where(p => !edited.ImageIds.Contains(p.Id)).Select(i => i.Path).AsEnumerable();
            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            foreach (string removable in removables)
            {
                string path = Path.Combine(imageFolderPath, "website-images", removable);
                await Console.Out.WriteLineAsync(path);
                Console.WriteLine(FileUpload.DeleteImage(path));
            }

            //TODO  You have to control validation: FileType and FileLength
            if (edited.MainPhoto is not null)
            {
                await AdjustPlantPhotos(edited.MainPhoto, plant, true);
            }
            else if (edited.HoverPhoto is not null)
            {
                await AdjustPlantPhotos(edited.HoverPhoto, plant, null);
            }

            plant.PlantImages.RemoveAll(p => !edited.ImageIds.Contains(p.Id));
            if (edited.Images is not null)
            {
                foreach (var item in edited.Images)
                {
                    if (!item.IsValidFile("image/") || !item.IsValidLength(1))
                    {
                        TempData["InvalidImages"] += item.FileName;
                        continue;
                    }
                    PlantImage plantImage = new()
                    {
                        IsMain = false,
                        Path = await item.CreateImage(imageFolderPath, "website-images")
                    };
                    plant.PlantImages.Add(plantImage);
                }
            }

            if (edited.TagIds is null)
            {
                edited.TagIds = model.TagIds;
            }

            else
            {
                foreach (var item in plant.PlantTags)
                {
                    _context.Remove(item);
                }
                foreach (var item in edited.TagIds)
                {
                    PlantTag plantTag = new()
                    {
                        PlantId = edited.Id,
                        TagId = item
                    };
                    _context.Add(plantTag);
                }
            }

            if (edited.CategoryIds is null)
            {
                edited.CategoryIds = model.CategoryIds;
            }

            else
            {
                foreach (var item in plant.PlantCategories)
                {
                    _context.Remove(item);
                }
                foreach (var item in edited.CategoryIds)
                {
                    PlantCategory plantCategory = new()
                    {
                        PlantId = edited.Id,
                        CategoryId = item
                    };
                    _context.Add(plantCategory);
                }
            }




            plant.Name = edited.Name;
            plant.Price = edited.Price;
            plant.Desc = edited.Desc;
            plant.SKU = edited.SKU;
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        private PlantVM? EditedPlant(int id)
        {
            PlantVM? model = _context.Plants.Include(p => p.PlantCategories)
                                            .Include(p => p.PlantTags)
                                            .Include(p => p.PlantImages)
                                            .Include(p=>p.PlantSizeColors)
                                            .Select(p =>
                                                new PlantVM
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    SKU = p.SKU,
                                                    Desc = p.Desc,
                                                    Price = p.Price,
                                                    DiscountPrice = p.Price,        
                                                    PlantDeliveryInformationId = p.PlantDeliveryInformationId,
                                                    ColorIds=p.PlantSizeColors.Select(c => c.ColorId).ToList(),//
                                                    SizeIds = p.PlantSizeColors.Select(c => c.SizeId).ToList(),//
                                                    CategoryIds = p.PlantCategories.Select(pc => pc.CategoryId).ToList(),
                                                    TagIds = p.PlantTags.Select(pc => pc.TagId).ToList(),
                                                    SpecificImages = p.PlantImages.Select(p => new PlantImage
                                                    {
                                                        Id = p.Id,
                                                        Path = p.Path,
                                                        IsMain = p.IsMain
                                                    }).ToList()
                                                })
                                                .FirstOrDefault(p => p.Id == id);
            return model;
        }
        private async Task AdjustPlantPhotos(IFormFile image, Plant plant, bool? isMain)
        {
            string photoPath = plant.PlantImages.FirstOrDefault(p => p.IsMain == isMain).Path;
            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            string filePath = Path.Combine(imagesFolderPath, "website-images", photoPath);
            FileUpload.DeleteImage(filePath);
            plant.PlantImages.FirstOrDefault(p => p.IsMain == isMain).Path = await image.CreateImage(imagesFolderPath, "website-images");
        }


        public IActionResult Search(string data)
        {
            List<Plant> plant = _context.Plants.Where(p => p.Name.Contains(data)).ToList();
            return Json(plant);
        }
        public IActionResult Delete(int id)
        {
            var plant = _context.Plants.FirstOrDefault(c => c.Id == id);

            if (plant == null)
            {

                return NotFound();
            }


            return View(plant);
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var plant = _context.Plants.FirstOrDefault(c => c.Id == id);

            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}