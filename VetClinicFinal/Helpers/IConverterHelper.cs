using System;
using VetClinic.Data.Entities;
using VetClinic.Models;

namespace VetClinicFinal.Helpers
{
    public interface IConverterHelper
    {
        Animal ToAnimal(AddAnimalViewModel model, Guid imageId, bool isNew);

        AddAnimalViewModel ToAnimalViewModel(Animal animal);
    }
}
