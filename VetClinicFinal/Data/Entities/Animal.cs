using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Entities
{
    public class Animal : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Animal's name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Breed { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }
        
        public Client Owner { get; set; }

        [Required]
        [Display(Name = "Owner")]
        public int OwnerId { get; set; }

    
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://vetclinic.blob.core.windows.net/images/noimage.png" 
            : $"https://vetclinic.blob.core.windows.net/animals/{ImageId}";

        [Display(Name = "Image")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
