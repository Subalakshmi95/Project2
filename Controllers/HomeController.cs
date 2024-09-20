using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Project2.Data;
using Project2.Models;
using System.Diagnostics;

namespace Project2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly int _pageSize = 4;

        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            if (booking.Photo != null && booking.Photo.Length > 0)
            {
                try
                {
                    // Get the filename and create a new unique name with GUID
                    var fileName = Path.GetFileName(booking.Photo.FileName);
                    var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";

                    // Get the path to the wwwroot folder
                    var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    // Define the folder for saving images inside wwwroot
                    var imageFolder = Path.Combine(wwwRootPath, "Images");

                    // Ensure the Images directory exists
                    Directory.CreateDirectory(imageFolder);

                    // Combine the path for the new file
                    var filePath = Path.Combine(imageFolder, newFileName);

                    // Save the image to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        booking.Photo.CopyTo(fileStream);
                    }

                    // Set the PhotoPath to be relative to the wwwroot for use in the view
                    booking.PhotoPath = $"/Images/{newFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading the photo."+ex.Message);
                    return View(booking);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please upload an image.");
                return View(booking);
            }

            try
            {
                _appDbContext.Bookings.Add(booking);
                _appDbContext.SaveChanges();
                TempData["Msg"] = "Booking created successfully.";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error saving changes."+ex.Message;
                return View(booking);
            }
        }


        [HttpGet]
       public IActionResult DisplayList(int pageNumber=1)
        {
            var TotalBookings=_appDbContext.Bookings.Count();
            var bookings = _appDbContext.Bookings.Skip((pageNumber - 1) * _pageSize).Take(_pageSize).ToList();
            var totalPages = (int)Math.Ceiling(TotalBookings /(double) _pageSize);
            var ViewModel = new BookingPageView()
            {
                Bookings = bookings,
                TotalPages = totalPages,
                CurrentPage = pageNumber
            };
            return View(ViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _appDbContext.Bookings.Find(Id);
            if (Id == 0)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }
            _appDbContext.Bookings.Update(booking);
            _appDbContext.SaveChanges();
            TempData["Msg"] = "Updated successfully";
            return RedirectToAction("DisplayList");
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var data = _appDbContext.Bookings.Find(Id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Delete(Booking booking, int Id)
        {
            var data = _appDbContext.Bookings.Find(Id);
            if (data == null)
            {
                return NotFound();
            }
            _appDbContext.Bookings.Remove(data);
            _appDbContext.SaveChanges();
            TempData["Msg"] = "Deleted successfully";
            return RedirectToAction("DisplayList");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
