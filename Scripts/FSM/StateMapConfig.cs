using System;
public class StateMapConfig
{
    public delegate void Behavior();
        
    public StateMap<string> Name;
    public StateMap<Behavior> Behaviors;
}
