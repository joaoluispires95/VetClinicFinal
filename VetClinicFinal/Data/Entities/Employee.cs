using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Entities
{
    public class Employee : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee's name")]
        public string Name { get; set; }

        [Required]
        public string Job { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
