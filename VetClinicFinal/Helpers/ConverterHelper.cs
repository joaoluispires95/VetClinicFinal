using System;
using VetClinic.Data.Entities;
using VetClinic.Models;

namespace VetClinicFinal.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Animal ToAnimal(AddAnimalViewModel model, Guid imageId, bool isNew)
        {
            return new Animal
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                DateOfBirth = model.DateOfBirth,
                Breed = model.Breed,
                ImageId = imageId,
                Owner = model.Owner,
                OwnerId = model.OwnerId
            };
        }

        public AddAnimalViewModel ToAnimalViewModel(Animal animal)
        {
            return new AddAnimalViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                DateOfBirth = animal.DateOfBirth,
                Breed = animal.Breed,
                ImageId = animal.ImageId,
                Owner = animal.Owner,
                OwnerId = animal.OwnerId
            };
        }
    }
}
