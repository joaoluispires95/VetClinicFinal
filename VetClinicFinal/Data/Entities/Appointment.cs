using System;
using System.ComponentModel.DataAnnotations;

namespace VetClinic.Data.Entities
{
    public class Appointment : IEntity
    {
        [Key]
        public int Id { get; set; }

        public Client Owner { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public Animal Animal { get; set; }

        [Required]
        public int AnimalId { get; set; }

        public Employee Doctor { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public User User { get; set; }
    }
}
