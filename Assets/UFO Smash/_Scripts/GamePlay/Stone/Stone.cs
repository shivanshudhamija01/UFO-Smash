using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float maxTravelDistance = 15f;

    [SerializeField] private float minimumScale = 0.3f;

private Vector3 spawnPosition;
    private bool hasDealtDamage = false;
    public void ResetStone()
    {
        hasDealtDamage = false;
        // Here i will reset the scale of the stone 
        transform.localScale = Vector3.one;
    }
    private void Update()
    {
        float travelledDistance = Vector3.Distance(spawnPosition,transform.position);

        float t = Mathf.Clamp01(travelledDistance /maxTravelDistance);

        float scale = Mathf.Lerp(1f,minimumScale,t);

        transform.localScale = Vector3.one * scale;
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