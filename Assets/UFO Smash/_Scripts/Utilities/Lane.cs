using UnityEngine;

[System.Serializable]
public class Lane
{
    public Transform pointA;
    public Transform pointB;

    [HideInInspector]
    public int currentAnimals;
    public bool firstOrderTaken;
    public bool secondOrderTaken;
}