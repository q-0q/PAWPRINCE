using System;
using UnityEngine;
using Wasp;
public abstract class Fsm : MonoBehaviour
{
    public Wasp.Machine<int, int> machine;
    private float _timeInCurrentState;
    protected StateMapConfig stateMapConfig;

    protected virtual void Start()
    {
        _timeInCurrentState = 0;
    }

    protected virtual void Update()
    {
        var behavior = stateMapConfig.Behaviors.Get(machine.State());
        behavior();
        IncrementClockByAmount(Time.deltaTime);
    }

    public class FsmState : InheritableEnum
    {
        public static int Any;
    }
    
    public class FsmTrigger : InheritableEnum
    {
        public static int InputDirection;
        public static int InputNoDirection;
        public static int InputJump;
        public static int InputInteract;
    }

    protected virtual void SetupStateMaps()
    {
        stateMapConfig = new StateMapConfig();
    }

    protected virtual void SetupMachine()
    {
        machine.OnTransitioned(OnStateChanged);
    }

    public float TimeInCurrentState()
    {
        return _timeInCurrentState;
    }

    
    protected virtual void OnStateChanged(TriggerParams? triggerParams)
    {
        _timeInCurrentState = 0;
    }
    
    public virtual void IncrementClockByAmount(float amount)
    {
        _timeInCurrentState += amount;
    }
}
