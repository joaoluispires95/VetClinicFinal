using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Entities;
using VetClinic.Helpers;
using VetClinicFinal.Helpers;

namespace VetClinic.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IAppointmentRepository _appointmentRepository;

        public EmployeesController(IEmployeeRepository employeeRepository, IUserHelper userHelper, IMailHelper mailHelper, IAppointmentRepository appointmentRepository)
        {
            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _appointmentRepository = appointmentRepository;
        }


        [Authorize(Roles = "Admin")]
        // GET: Employes
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAll().OrderBy(p => p.Name));
        }

        [Authorize(Roles = "Admin")]
        // GET: Employes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employes/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Employes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeRepository.CreateAsync(employee);

                //Create User

                var user = await _userHelper.GetUserByEmailAsync(employee.Email);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = employee.Name,
                        Email = employee.Email,
                        UserName = employee.Email
                    };

                    var result = await _userHelper.AddUserAsync(user, "123456789");

                    if(employee.Job == "Admin")
                        await _userHelper.AddUserToRoleAsync(user, "Admin");
                    else
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                    var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                    var link = this.Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(user.Email, "Your VetClinic Account",
                    $"Welcome {user.FirstName},<br/> " +
                    $"Your account has been created. Your credentials are:<br/>" +
                    $"Username: {user.Email}<br/>" +
                    $"Password: Change password click in this link:<br/>" +
                    $"<a href = \"{link}\">Reset Password</a><br/><br/>" +
                    $"Thank you for your preference." + "<br/><br/> VetClinic");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }
            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // POST: Employes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeRepository.ExistsAsync(employee.Id))
                    {
                        return new NotFoundViewResult("EmployeNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeNotFound");
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // POST: Employes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            var count = _appointmentRepository.GetAll().Count();

            for (int i = 1; i <= count; i++)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(i);

                if (appointment.DoctorId == id)
                {
                    TempData["Message"] = "The employee cannot be deleted because it has an appointment!";

                    return RedirectToAction(nameof(Index));
                }
            }

            await _employeeRepository.DeleteAsync(employee);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EmployeNotFound()
        {
            return View();
        }
    }
}
