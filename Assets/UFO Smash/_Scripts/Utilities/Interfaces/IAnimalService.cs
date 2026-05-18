using System.Collections.Generic;
using UnityEngine;

public interface IAnimalService
{
    public void AddAnimal(AnimalController animal);
    public List<AnimalController> GetAnimalInScene();
    public void RemoveAnimal(AnimalController animal);
    public int AnimalCountInScene();
}
