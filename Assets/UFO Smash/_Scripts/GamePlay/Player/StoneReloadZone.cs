using UnityEngine;
using System.Collections;

public class StoneReloadZone : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 0.5f;

    private Coroutine reloadCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player"))
            return;

        Aim aim = collision.GetComponent<Aim>();

        if (aim != null)
        {
            reloadCoroutine =
                StartCoroutine(ReloadStones(aim));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }

    IEnumerator ReloadStones(Aim aim)
    {
        while (aim.GetCurrentAmmo() < aim.GetMaxAmmo())
        {
            yield return new WaitForSeconds(reloadDelay);

            aim.ReloadToMax();
        }
    }
    //     IEnumerator ReloadStones(Aim aim)
    // {
    //     while (aim.GetCurrentAmmo() < aim.GetMaxAmmo())
    //     {
    //         yield return new WaitForSeconds(reloadDelay);

    //         aim.AddStone(1);
    //     }
    // }
}