using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserApp.Models;
using UserApp.Repository;
using UserApp.ViewModels;

namespace UserApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IUserRepository userRepository,
                IWebHostEnvironment webHostEnvironment)
        {
            this.userRepository = userRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<UserClass> user = await userRepository.GetUsers();
                UserCreateViewModel model = new UserCreateViewModel
                {
                    UserList = user,
                };
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Resource not available:";
                ViewBag.ErrorMessage = "Delaying in retrieving data from database. Please reload the site";
                return View("CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {

                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string upLoadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(upLoadFolder, uniqueFileName);
                        using (Stream stream = new FileStream(filePath, FileMode.Create)) 
                            await photo.CopyToAsync(stream);
                    }
                }

                UserClass user = new UserClass
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    PhotPath = uniqueFileName
                };
                await userRepository.AddUser(user);
                return RedirectToAction("Index", new { user.Id });
            }
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {

            UserClass user = await userRepository.GetUser(id.Value);
            if (user == null)
            {
                return View("Error", id.Value);
            }
            UserDetailsViewModel model = new UserDetailsViewModel
            {
                User = user,
                PageTitle = "DETAILS.cshtml"
            };

            model.FileType = Path.GetExtension(user.PhotPath);

            return View(model);
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine("wwwroot/Images", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            UserClass user = await userRepository.GetUser(id);

            UserEditViewModel model = new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                ExistingPhotoPath = user.PhotPath

                //ExistingPhotoPath = employee.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserClass user = await userRepository.GetUser(model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                //user.PhotPath = 
                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);

                    }
                    user.PhotPath = ProcessUploadedFile(model); //1
                }

               await userRepository.UpdateUser(user);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadedFile(UserEditViewModel model) //2
        {
            string uniqueFileName = null;
            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (IFormFile photo in model.Photos)
                {
                    string upLoadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(upLoadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                        photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            var user = await userRepository.GetUser(Id.Value);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await userRepository.DeleteUser(Id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = $"The Department cannot be deleted becouse of reference constraint of his/her Id in another database";
                return View("CustomError");
            }
        }

    }
}
