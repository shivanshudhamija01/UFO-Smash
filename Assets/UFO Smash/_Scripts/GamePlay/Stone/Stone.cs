using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

    public int GetDamage()
    {
        return damage;
    }
}