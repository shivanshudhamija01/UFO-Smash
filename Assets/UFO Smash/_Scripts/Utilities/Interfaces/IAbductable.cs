using UnityEngine;

public interface IAbductable
{
    void BeginAbduction(Transform abductTarget);
    void CancelAbduction();
    Transform GetTransform();
}
