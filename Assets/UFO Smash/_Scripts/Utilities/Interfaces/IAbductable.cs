using UnityEngine;

public interface IAbductable
{
    void BeginAbduction(Transform abductTarget, UFOController ufo);
    void ReleaseFromAbduction();
    Transform GetTransform();
}
// I think here i need to do the modification , the animal is continue in roaming state, till the ufo is in movement , so either i have to remove the  animal from the service when it is about to reach at the end point or i have to stop it earlier
