using System;
public class StateMapConfig<TState>
{
    public delegate void Behavior();
        
    public StateMap<TState, string> Name;
    public StateMap<TState, Behavior> Behaviors;
}
