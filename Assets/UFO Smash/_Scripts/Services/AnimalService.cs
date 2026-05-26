using System.Collections.Generic;
using UnityEngine;

public class AnimalService : IAnimalService
{
    private List<AnimalController> animalActiveInScene = new List<AnimalController>();
    public void AddAnimal(AnimalController animal)
    {
        animalActiveInScene.Add(animal);
    }


    public List<AnimalController> GetAnimalInScene()
    {
        if (animalActiveInScene != null)
        {
            return animalActiveInScene;
        }
        return null;
    }
    public int AnimalCountInScene()
    {
        if (animalActiveInScene != null)
        {
            return animalActiveInScene.Count;
        }
        return 0;
    }

    public void RemoveAnimal(AnimalController animal)
    {
        if (animalActiveInScene.Count > 0 && animalActiveInScene.Contains(animal))
        {
            animalActiveInScene.Remove(animal);
        }
    }
}
