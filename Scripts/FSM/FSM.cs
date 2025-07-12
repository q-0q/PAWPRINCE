using System;
using UnityEngine;
using Wasp;
public abstract class Fsm : MonoBehaviour
{
    public Wasp.Machine<int, int> machine;
    private float _timeInCurrentState;
    private StateMapConfig _stateMapConfig;

    void Start()
    {
        _timeInCurrentState = 0;
    }

    private void Update()
    {
        IncrementClockByAmount(Time.deltaTime);
    }

    public class FsmState : InheritableEnum
    {
        public static int Any;
    }
    
    public class Trigger : InheritableEnum
    {
        public static int InputDirection;
        public static int InputJump;
        public static int InputInteract;
    }

    public virtual void SetupStateMaps()
    {
        _stateMapConfig = new StateMapConfig();
    }

    public virtual void SetupMachine()
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
