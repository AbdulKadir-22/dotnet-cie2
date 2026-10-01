using App131.Extensions;
using App131.Models;
using App131.Services;
using Microsoft.AspNetCore.Mvc;

namespace App131.Controllers
{
    public class HomeController : Controller
    {
        private readonly DoctorService _doctorService;

        public HomeController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index(string? specialization)
        {
            var doctors = _doctorService.GetDoctors();

            // State Management: Session for Selected Specialization
            if (!string.IsNullOrEmpty(specialization))
            {
                HttpContext.Session.SetString("SelectedSpecialization", specialization);
            }
            ViewBag.SelectedSpecialization = HttpContext.Session.GetString("SelectedSpecialization");

            // State Management: Cookie for Patient Name (if submitted or stored)
            if (Request.Cookies.ContainsKey("PatientNameCookie"))
            {
                ViewBag.PatientNameCookie = Request.Cookies["PatientNameCookie"];
            }

            // Asynchronous Web Request (JSONPlaceholder posts)
            var posts = await _doctorService.GetExternalPostsAsync();
            ViewBag.Posts = posts;

            return View(doctors);
        }

        // State Management: Response Caching for 30 seconds
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public IActionResult Details(int id)
        {
            var doctor = _doctorService.GetDoctors().FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null) return NotFound();

            // Calculate total consultation charge via Extension Method
            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            return View(doctor);
        }

        [HttpGet]
        public IActionResult BookAppointment(int doctorId, string patientName)
        {
            var doctor = _doctorService.GetDoctors().FirstOrDefault(d => d.DoctorId == doctorId);
            if (doctor == null) return NotFound();

            // Store Patient Name in Cookie if provided
            if (!string.IsNullOrEmpty(patientName))
            {
                CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
                Response.Cookies.Append("PatientNameCookie", patientName, options);
            }

            var model = new AppointmentViewModel
            {
                DoctorId = doctor.DoctorId,
                DoctorName = doctor.DoctorName,
                PatientName = patientName ?? Request.Cookies["PatientNameCookie"] ?? string.Empty,
                AppointmentDate = DateTime.Now.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult BookAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save patient name cookie
                Response.Cookies.Append("PatientNameCookie", model.PatientName, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
                TempData["SuccessMessage"] = $"Appointment successfully booked with {model.DoctorName} for {model.PatientName}!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}