using System.Collections.Generic;
using UnityEngine;

public class AnimalService : IAnimalService
{
    private List<AnimalController> animalActiveInScene = new List<AnimalController>();
    public void AddAnimal(AnimalController animal)
    {
        animalActiveInScene.Add(animal);
        // Debug.Log("Animal added to the scene is : " + animal.gameObject.name + " " + " the count of list is : " + animalActiveInScene.Count);
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
            // Debug.Log("Animal added to the scene is : " + animal.gameObject.name + " " + " the count of list is : " + animalActiveInScene.Count);
        }
        else
        {
            // Debug.LogWarning("Either list is empty or not contains any animal that you are trying to remove");
        }
    }
}
