using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Entities;
using VetClinic.Helpers;

namespace VetClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserHelper _userHelper;
        private readonly IAnimalRepository _animalRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IUserHelper userHelper, IAnimalRepository animalRepository)
        {
            _appointmentRepository = appointmentRepository;
            _userHelper = userHelper;
            _animalRepository = animalRepository;
        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AppointmentsController
        public IActionResult Index()
        {
            return View(_appointmentRepository.GetAll().Include(a => a.Animal).Include(a => a.Owner).Include(b => b.Doctor));
        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AppointmentsController/Create
        public ActionResult Create()
        {
            var count = _animalRepository.GetAll().Count();

            if (count == 0)
            {
                TempData["Message"] = "You need to create an Animal first!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Owners = _appointmentRepository.GetComboOwners();

                ViewBag.Employees = _appointmentRepository.GetComboEmployees();

                ViewBag.Animals = _appointmentRepository.GetComboAnimals(0);

                return View();
            }
        }

        public JsonResult LoadAnimals(int id)
        {
            var animals = _appointmentRepository.GetComboAnimals(id);
            return Json(animals);
        }

        [Authorize(Roles = "Employee,Client")]
        // POST: AppointmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            
                if (ModelState.IsValid)
                {
                    appointment.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _appointmentRepository.CreateAsync(appointment);

                    return RedirectToAction(nameof(Index));

                }
                return View(appointment);
            
        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AppointmentsController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var appointment = await _appointmentRepository.GetByIdAppointmentAsync(id.Value);

            if (appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            return View(appointment);
        }

        [Authorize(Roles = "Employee,Client")]
        // POST: AppointmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    
                    await _appointmentRepository.UpdateAsync(appointment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _appointmentRepository.ExistsAsync(appointment.Id))
                    {
                        return new NotFoundViewResult("AppointmentNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AppointmentsController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var appointment = await _appointmentRepository.GetByIdAppointmentAsync(id.Value);

            if (appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            return View(appointment);
        }

        [Authorize(Roles = "Employee,Client")]
        // POST: AppointmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            await _appointmentRepository.DeleteAsync(appointment);

            return RedirectToAction(nameof(Index));
        }
    }
}
