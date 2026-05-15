using UnityEngine;

public class AnimalStateMachine : BaseStateMachine<AnimalController, AnimalState>
{
    public AnimalStateMachine(AnimalController controller) : base(controller) { }

    protected override void InitializeStates()
    {
        RegisterState(AnimalState.sleep, new AnimalSleep(controller));
        RegisterState(AnimalState.roam, new AnimalRoam(controller));
        RegisterState(AnimalState.panic, new AnimalPanic(controller));
        RegisterState(AnimalState.abducting, new AnimalAbduct(controller));
        RegisterState(AnimalState.taken, new AnimalTaken(controller));
        RegisterState(AnimalState.rescue, new AnimalRescue(controller));
    }
}


// i have write the base state , 
// base state machine,
// now i need to search for the controller and then lets see what will happen