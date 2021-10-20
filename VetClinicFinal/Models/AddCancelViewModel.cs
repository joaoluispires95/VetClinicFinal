using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Entities;

namespace VetClinicFinal.Models
{
    public class AddCancelViewModel : Employee
    {
        public IEnumerable<SelectListItem> Employees { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select a doctor")]
        public int DoctorId { get; set; }
    }
}
