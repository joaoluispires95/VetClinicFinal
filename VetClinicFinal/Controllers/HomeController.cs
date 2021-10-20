using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Entities;
using VetClinic.Models;
using VetClinicFinal.Helpers;
using VetClinicFinal.Models;

namespace VetClinicFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(ILogger<HomeController> logger, IClientRepository clientRepository, IMailHelper mailHelper, IAppointmentRepository appointmentRepository, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _clientRepository = clientRepository;
            _mailHelper = mailHelper;
            _appointmentRepository = appointmentRepository;
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Communication()
        {
            return View();
        }

        public IActionResult UC()
        {
            return View();
        }

        public IActionResult AR()
        {
            return View();
        }

        public IActionResult AC()
        {
            AddCancelViewModel model = new AddCancelViewModel();

            model.Employees = _employeeRepository.GetComboEmployees();

            return View(model);
        }

        public async Task<IActionResult> EnviaMailUC()
        {
            var count = _clientRepository.GetAll().Count();

            for (int i = 1; i <= count; i++)
            {
                var client = await _clientRepository.GetByIdAsync(i);


                _mailHelper.SendEmail(client.Email, "VetClinic Information",
                "Hello, we would like to inform you that today we we'll be closed");
            }
            return RedirectToAction(nameof(Communication));
        }

        public async Task<IActionResult> EnviaMailAR()
        {
            var count = _clientRepository.GetAll().Count();

            for (int i = 1; i <= count; i++)
            {
                var client = await _clientRepository.GetByIdAsync(i);

                var appointment = await _appointmentRepository.GetByClientIdAppointmentAsync(i);

                var aux = (DateTime.Parse(DateTime.Now.ToShortDateString()).Subtract(DateTime.Parse(appointment.Time.ToShortDateString()))).Days;

                if (aux <= 1)
                {
                    _mailHelper.SendEmail(client.Email, "VetClinic Information",
                    "Hello, we would like to inform you about your appointment tommorow. <br />" + 
                    $"Pet: {appointment.Animal.Name} <br />" +
                    $"Time: {appointment.Time}");
                }
            }
            return RedirectToAction(nameof(Communication));
        }

        [HttpPost]
        public async Task<IActionResult> EnviaMailAC(AddCancelViewModel model)
        {
            var count = _appointmentRepository.GetAll().Count();

            for (int i = 1; i <= count; i++)
            {
                var appointment = await _appointmentRepository.GetByIdAppointmentAsync(i);

                var employee = await _employeeRepository.GetEmployeeByIdAsync(appointment.DoctorId);

                if(employee.Id == appointment.DoctorId)
                {
                    _mailHelper.SendEmail(appointment.Owner.Email, "VetClinic Information",
                    $"Hello, we would like to inform you that your appointments with Dr.{employee.Name} have been canceled");
                }
            }
            return RedirectToAction(nameof(Communication));
        }
    }
}
