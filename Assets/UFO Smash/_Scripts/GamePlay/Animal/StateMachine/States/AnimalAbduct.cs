using UnityEngine;

public class AnimalAbduct : BaseState<AnimalController>
{
    private Transform abductTarget;
    public AnimalAbduct(AnimalController controller) : base(controller)
    {
    }
    public override void OnEnterState()
    {
        abductTarget = controller.GetAbductTarget();
        Debug.Log("Animal entered into abduct state");
    }
    public override void UpdateState()
    {
        Debug.Log("Now i have to tilt a little and start raising my animal towards the ufo");
    }
    public override void OnExitState()
    {

    }
    public override void FixedUpdateState()
    {

    }

}

