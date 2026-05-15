using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine<TController, TStateEnum>
    where TController : class
    where TStateEnum : struct, Enum
{
    protected TController controller;
    protected BaseState<TController> currentState;
    protected TStateEnum currentStateType;
    protected TStateEnum previousStateType;
    protected Dictionary<TStateEnum, BaseState<TController>> stateRegistry = new Dictionary<TStateEnum, BaseState<TController>>();

    public event Action<TStateEnum, TStateEnum> OnStateChanged;

    public BaseStateMachine(TController controller)
    {
        this.controller = controller;
        InitializeStates();
    }

    protected virtual void InitializeStates()
    {
        // Override in derived classes
    }

    public void Start(TStateEnum initialState)
    {
        ChangeState(initialState);
    }

    public bool ChangeState(TStateEnum newState)
    {
        // Prevent transition to same state
        if (EqualityComparer<TStateEnum>.Default.Equals(currentStateType, newState) && currentState != null)
        {
            return false;
        }

        // Validate state exists
        if (!stateRegistry.TryGetValue(newState, out var nextState))
        {
            Debug.LogError($"State {newState} not registered in state machine!");
            return false;
        }

        // Store previous state
        previousStateType = currentStateType;

        // Exit current state
        currentState?.OnExitState();

        // Transition to new state
        var oldState = currentStateType;
        currentState = nextState;
        currentStateType = newState;

        // Enter new state
        currentState.OnEnterState();

        // Trigger event
        OnStateChanged?.Invoke(oldState, newState);

        return true;
    }

    public void Update() => currentState?.UpdateState();

    public TStateEnum GetCurrentState() => currentStateType;
    public TStateEnum GetPreviousState() => previousStateType;
    public bool IsInState(TStateEnum state) => EqualityComparer<TStateEnum>.Default.Equals(currentStateType, state);

    protected void RegisterState(TStateEnum stateType, BaseState<TController> state)
    {
        if (stateRegistry.ContainsKey(stateType))
        {
            Debug.LogWarning($"State {stateType} already registered, overwriting...");
        }
        stateRegistry[stateType] = state;
    }
}