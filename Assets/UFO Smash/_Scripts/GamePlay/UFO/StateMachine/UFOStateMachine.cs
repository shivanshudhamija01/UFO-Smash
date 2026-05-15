using System;
using UnityEngine;

public class UFOStateMachine : BaseStateMachine<UFOController, UFOStates>
{
    public UFOStateMachine(UFOController controller) : base(controller) { }
    protected override void InitializeStates()
    {
        RegisterState(UFOStates.entry, new UFOEntry(controller));
        RegisterState(UFOStates.abduct, new UFOAbduct(controller));
        RegisterState(UFOStates.blast, new UFOBlast(controller));
        RegisterState(UFOStates.success, new UFOSuccess(controller));
    }

}
