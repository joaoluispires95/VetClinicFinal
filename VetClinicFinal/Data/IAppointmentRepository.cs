using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Entities;

namespace VetClinic.Data
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Animal> ExistAnimal(int id);

        IEnumerable<SelectListItem> GetComboOwners();

        IEnumerable<SelectListItem> GetComboAnimals(int id);

        IEnumerable<SelectListItem> GetComboEmployees();

        Task<Appointment> GetByIdAppointmentAsync(int id);

        public IQueryable GetAllWithUsers();

        Task<Appointment> GetByClientIdAppointmentAsync(int id);

        Task<Appointment> GetByAppointmentIdAppointmentAsync(int id);
    }
}
