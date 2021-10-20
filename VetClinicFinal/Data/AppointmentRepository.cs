using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Entities;

namespace VetClinic.Data
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Animal> ExistAnimal(int id)
        {
            return await _context.Set<Animal>().AsNoTracking().Include(p => p.Owner).FirstOrDefaultAsync(e => e.Id == id);
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Appointments.Include(p => p.Animal).Include(p => p.Doctor).Include(p => p.Owner);
        }

        public async Task<Appointment> GetByIdAppointmentAsync(int id)
        {
            return await _context.Set<Appointment>().AsNoTracking().Include(p => p.Animal).Include(p => p.Owner).FirstOrDefaultAsync(e => e.Id == id);
        }

        public IEnumerable<SelectListItem> GetComboAnimals(int id)
        {
            var animal = _context.Animals.Find(id);

            var list = new List<SelectListItem>();

            if (animal != null)
            {
                list = _context.Animals.Where(c => c.OwnerId == id).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }).ToList();


                list.Insert(0, new SelectListItem
                {
                    Text = "Select an animal",
                    Value = "0"
                });

            }

            return list;
        }

        public IEnumerable<SelectListItem> GetComboEmployees()
        {
            var list = _context.Employees.Where(c => c.Job == "Doctor").Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()

            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a Doctor",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboOwners()
        {
            var list = _context.Clients.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()

            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select an Owner",
                Value = "0"
            });

            return list;
        }

        public async Task<Appointment> GetByClientIdAppointmentAsync(int id)
        {
            return await _context.Set<Appointment>().Include(p => p.Animal).Include(p => p.Owner).FirstOrDefaultAsync(e => e.OwnerId == id);
        }

        public async Task<Appointment> GetByEmployeeIdAppointmentAsync(int id)
        {
            return await _context.Set<Appointment>().Include(p => p.Animal).Include(p => p.Owner).FirstOrDefaultAsync(e => e.DoctorId == id);
        }

        public Task<Appointment> GetByAppointmentIdAppointmentAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
