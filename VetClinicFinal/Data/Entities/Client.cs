using System;
using System.ComponentModel.DataAnnotations;

namespace VetClinic.Data.Entities
{
    public class Client : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client's name")]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public User User { get; internal set; }
    }
}
