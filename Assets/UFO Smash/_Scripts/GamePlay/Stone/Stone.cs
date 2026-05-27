using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    private bool hasDealtDamage = false;
    public void ResetStone()
    {
        hasDealtDamage = false;
    }

    // Replace / wrap your existing OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StonePool.Instance.Return(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("UFO"))
        {
            Debug.Log("Yup entered into the UFO collider");
            if (hasDealtDamage) return;
            hasDealtDamage = true;
        }
    }
    public bool HasAlreadyHitUFO() => hasDealtDamage;
    public int GetDamage()
    {
        return damage;
    }
}