using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Entities;
using VetClinic.Helpers;

namespace VetClinic.Data
{
    public class SeedDB
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDB(DataContext dataContext, IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public UserManager<User> userManager { get; }

        public async Task SeedAsync()
        {
            await _dataContext.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Client");

            var user = await _userHelper.GetUserByEmailAsync("joao.luis.pires@formandos.cinel.pt");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "João",
                    LastName = "Pires",
                    Email = "joao.luis.pires@formandos.cinel.pt",
                    UserName = "joao.luis.pires@formandos.cinel.pt",
                    PhoneNumber = "911767893"
                };

                var result = await _userHelper.AddUserAsync(user, "123456789");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_dataContext.Employees.Any())
            {
                AddEmployee("Joao Pires", "joao.luis.pires@formandos.cinel.pt", "Admin");

                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddEmployee(string name, string email, string job)
        {
            _dataContext.Employees.Add(new Employee
            {
                Name = name,
                Email = email,
                Job = "Admin",
            });
        }
    }
}
