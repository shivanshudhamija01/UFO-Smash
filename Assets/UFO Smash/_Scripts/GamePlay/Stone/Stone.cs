using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float maxTravelDistance = 15f;

    [SerializeField] private float minimumScale = 0.3f;
    private IAudioService audioService;
    private Vector3 spawnPosition;
    private bool hasDealtDamage = false;
    private void Awake()
    {
        if (audioService == null)
        {
            audioService = ServiceLocator.Get<IAudioService>();
        }
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    public void ResetStone()
    {
        hasDealtDamage = false;

        transform.localScale = Vector3.one;

        spawnPosition = transform.position;
    }
    private void Update()
    {
        float travelledDistance = Vector3.Distance(spawnPosition, transform.position);

        float t = Mathf.Clamp01(travelledDistance / maxTravelDistance);

        float scale = Mathf.Lerp(1f, minimumScale, t);

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
            if (hasDealtDamage) return;
            hasDealtDamage = true;
            audioService.SFX(SoundType.StoneHit);
        }
    }
    public bool HasAlreadyHitUFO() => hasDealtDamage;
    public int GetDamage()
    {
        return damage;
    }
    // Here i need to add the logic for returning to pool

}