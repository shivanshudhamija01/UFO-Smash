using UnityEngine;

public class AnimalPanic : BaseState<AnimalController>
{
    public AnimalPanic(AnimalController controller) : base(controller)
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
// When the UFO lock the animal , then needs to change the state to tht panic state.
// In panic state, the animal stop moving and its animation will change
