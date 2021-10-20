using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data;
using VetClinic.Data.Entities;
using VetClinic.Helpers;
using VetClinicFinal.Helpers;

namespace VetClinic.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IAnimalRepository _animalRepository;

        public ClientsController(IClientRepository clientRepository, IUserHelper userHelper, IMailHelper mailHelper, IAnimalRepository animalRepository)
        {
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _animalRepository = animalRepository;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                client.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                await _clientRepository.CreateAsync(client);

                //Create User

                var user = await _userHelper.GetUserByEmailAsync(client.Email);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = client.Name,
                        Email = client.Email,
                        UserName = client.Email
                    };

                    var result = await _userHelper.AddUserAsync(user, "123456789");

                    await _userHelper.AddUserToRoleAsync(user, "Client");

                    Response response = _mailHelper.SendEmail(user.Email, "Your VetClinic Account",
                    $"Welcome {user.FirstName},<br/> " +
                    $"Your account has been created. Your credentials are:<br/>" +
                    $"Username: {user.Email}<br/>" +
                    $"Password: 123456789 <br/><br/>" +
                    $"Thank you." + "<br/><br/> VetClinic");
                }

                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientRepository.UpdateAsync(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clientRepository.ExistsAsync(client.Id))
                    {
                        return new NotFoundViewResult("ClientNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            var count = _animalRepository.GetAll().Count();

            for (int i = 1; i <= count; i++)
            {
                var animal = await _animalRepository.GetByIdAsync(i);

                if (animal.OwnerId == id)
                {
                    TempData["Message"] = "The client cannot be deleted because it has an animal!";

                    return RedirectToAction(nameof(Index));
                }
            }

            await _clientRepository.DeleteAsync(client);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ClientNotFound()
        {
            return View();
        }
    }
}
