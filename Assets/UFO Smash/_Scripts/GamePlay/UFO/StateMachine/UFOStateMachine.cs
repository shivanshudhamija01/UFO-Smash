using System;
using UnityEngine;

public class UFOStateMachine : BaseStateMachine<UFOController, UFOStates>
{
    public UFOStateMachine(UFOController controller) : base(controller) { }
    protected override void InitializeStates()
    {
        RegisterState(UFOStates.spline, new UFOSpline(controller));
        RegisterState(UFOStates.hover, new UFOHover(controller));
        RegisterState(UFOStates.abduct, new UFOAbduct(controller));
        RegisterState(UFOStates.blast, new UFOBlast(controller));
        RegisterState(UFOStates.success, new UFOSuccess(controller));
    }

}
