using UnityEngine;

public class AnimalTaken : BaseState<AnimalController>
{
    public AnimalTaken(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {

    }
    public override void OnExitState()
    {

    }
    public override void UpdateState()
    {

    }
    public override void FixedUpdateState()
    {

    }
}

// This is the state , when UFO is successful of taking the animal away , 
// At this, the player will lost the point and the animal is put back to the pool
// and also i have to update the services also