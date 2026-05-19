using UnityEngine;

public interface IAbductable
{
    void BeginAbduction(Transform abductTarget, UFOController ufo);
    void CancelAbduction();
    Transform GetTransform();
}
