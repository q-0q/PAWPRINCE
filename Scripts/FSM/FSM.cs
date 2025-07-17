using System;
using UnityEngine;
using Wasp;
public class Fsm<TState, TTrigger>
{
    public Wasp.Machine<TState, TTrigger> machine;
    private float _timeInCurrentState;
    public StateMapConfig<TState> stateMapConfig;


    public Fsm(TState initState)
    {
        machine = new Machine<TState, TTrigger>(initState);
        _timeInCurrentState = 0;
    }
    
    public void Update()
    {
        var behavior = stateMapConfig.Behaviors.Get(machine.State());
        behavior?.Invoke();
        IncrementClockByAmount(Time.deltaTime);
    }
    
    public void SetupStateMaps()
    {
        stateMapConfig = new StateMapConfig<TState>();
        stateMapConfig.Name = new StateMap<TState, string>("No state name provided");
        stateMapConfig.Behaviors = new StateMap<TState, StateMapConfig<TState>.Behavior>(null);
    }

    public void SetupMachine()
    {
        machine.OnTransitioned(OnStateChanged);
    }

    public float TimeInCurrentState()
    {
        return _timeInCurrentState;
    }
    
    private void OnStateChanged(TriggerParams? triggerParams)
    {
        _timeInCurrentState = 0;
    }

    private void IncrementClockByAmount(float amount)
    {
        _timeInCurrentState += amount;
    }
}
