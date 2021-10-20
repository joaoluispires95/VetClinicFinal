using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Entities;

namespace VetClinic.Data
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        IEnumerable<SelectListItem> GetComboOwners();

        Task<Animal> GetByIdAnimalAsync(int id);
    }
}
