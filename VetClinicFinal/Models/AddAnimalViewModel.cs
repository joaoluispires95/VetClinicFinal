
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetClinic.Data.Entities;

namespace VetClinic.Models
{
    public class AddAnimalViewModel : Animal
    {
        public IEnumerable<SelectListItem> Owners { get; set; }
    }
}
