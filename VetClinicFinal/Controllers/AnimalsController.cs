using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Entities;
using VetClinic.Helpers;
using VetClinic.Models;
using VetClinicFinal.Helpers;

namespace VetClinic.Controllers
{
    

    public class AnimalsController : Controller
    {

        private readonly IAnimalRepository _animalRepository;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IAppointmentRepository _appointmentRepository;

        public AnimalsController(IAnimalRepository animalRepository, IUserHelper userHelper, IClientRepository clientRepository, 
            IBlobHelper blobHelper, IConverterHelper converterHelper, IAppointmentRepository appointmentRepository)
        {
            _animalRepository = animalRepository;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _appointmentRepository = appointmentRepository;
        }


        [Authorize(Roles = "Employee,Client")]
        // GET: AnimalsController
        public IActionResult Index()
        {
            return View(_animalRepository.GetAll().Include(p => p.Owner));

        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AnimalsController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAnimalAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AnimalsController/Create
        public IActionResult Create()
        {
            var count = _clientRepository.GetAll().Count();

            if(count == 0)
            {
                TempData["Message"] = "You need to create an Owner first";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var model = new AddAnimalViewModel{
                    Owners = _animalRepository.GetComboOwners()
                };

                TempData["Message"] = "";

                return View(model);
            }
        }

        [Authorize(Roles = "Employee,Client")]
        // POST: AnimalsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "animals");
                }

                var animal = _converterHelper.ToAnimal(model, imageId, true);

                await _animalRepository.CreateAsync(animal);

                return RedirectToAction(nameof(Index));
            }

            return View(model);

        }

        [Authorize(Roles = "Employee,Client")]
        // GET: AnimalsController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAnimalAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }
            return View(animal);
        }

        [Authorize(Roles = "Employee,Client")]
        // POST: AnimalsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Animal animal)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = animal.ImageId;

                    if (animal.ImageFile != null && animal.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(animal.ImageFile, "animals");

                        animal.ImageId = imageId;
                    }

                    await _animalRepository.UpdateAsync(animal);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _animalRepository.ExistsAsync(animal.Id))
                    {
                        return new NotFoundViewResult("AnimalNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        [Authorize(Roles = "Employee")]
        // GET: AnimalsController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("Animal Not Found");
            }

            var veicule = await _animalRepository.GetByIdAnimalAsync(id.Value);

            if (veicule == null)
            {
                return new NotFoundViewResult("Animal Not Found");
            }

            return View(veicule);
        }

        [Authorize(Roles = "Employee")]
        // POST: AnimalsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _animalRepository.GetByIdAnimalAsync(id);

            var count = _appointmentRepository.GetAll().Count();

            for(int i = 1; i <= count; i++)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(i);

                if(appointment.AnimalId == id)
                {
                    TempData["Message"] = "The animal cannot be deleted because it has an appointment!"; 

                    return RedirectToAction(nameof(Index));
                }
            }

            await _animalRepository.DeleteAsync(animal);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AnimalNotFound()
        {
            return View();
        }
    }
}
