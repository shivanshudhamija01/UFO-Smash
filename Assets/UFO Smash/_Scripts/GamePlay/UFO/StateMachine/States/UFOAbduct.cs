using UnityEngine;

public class UFOAbduct : BaseState<UFOController>
{
    private bool isStoneHit;
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
// When the animal is about to reach the ufo center, then it will make the animal disappear and change the ufo state to ufo success, 
// and then ufo will run away 