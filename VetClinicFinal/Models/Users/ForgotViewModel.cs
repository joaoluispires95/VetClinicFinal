using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinicFinal.Models.Users
{
    public class ForgotViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
