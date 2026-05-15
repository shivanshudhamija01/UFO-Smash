using UnityEngine;

public class AnimalRescue : BaseState<AnimalController>
{
    public AnimalRescue(AnimalController controller) : base(controller)
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

// in rescue state, i will change the animation to happy state, and 
// when the animal touches the ground, i will change the state back to roaming 
