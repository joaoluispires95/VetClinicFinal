using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VetClinic.Data.Entities;

namespace VetClinic.Data
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        IEnumerable<SelectListItem> GetComboEmployees();

        Task<Employee> GetEmployeeByIdAsync(int id);
    }
}
