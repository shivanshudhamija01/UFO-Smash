using UnityEngine;

public class UFOAbduct : BaseState<UFOController>
{
    public UFOAbduct(UFOController controller) : base(controller)
    {
    }
    public override void OnEnterState()
    {
        Debug.Log("Entered into the UFO abduct state");
    }
    public override void UpdateState()
    {

    }
    public override void OnExitState()
    {

    }
    public override void FixedUpdateState()
    {

    }
}
