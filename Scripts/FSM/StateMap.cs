using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMap<TState, T>
{
    private Dictionary<TState, List<Binding<T>>> _dictionary;
    private T _default;
    
    public StateMap(T @default)
    {
        _dictionary = new Dictionary<TState, List<Binding<T>>>();
        _default = @default;
    }
    
    public T Get<TAny>(Fsm<TState, TAny> fsm)
    {
        var eligibleBindings = _dictionary
            .Where(kv => fsm.machine.IsInState(kv.Key))
            .SelectMany(kv => kv.Value)
            .ToList();

        if (eligibleBindings.Count == 0)
            return _default;

        var maxWeight = eligibleBindings.Max(b => b.Weight());

        var topBindings = eligibleBindings
            .Where(b => b.Weight() == maxWeight)
            .ToList();

        if (topBindings.Count > 1)
            Debug.LogError($"Tie detected: {topBindings.Count} bindings with weight {maxWeight}");

        return topBindings[0].Value();
    }

    public void Add(TState state, T value, int weight = 0)
    {
        if (!_dictionary.ContainsKey(state)) _dictionary[state] = new List<Binding<T>>();
        _dictionary[state].Add(new Binding<T>(value, weight));
    }
}

public class Binding<T>
{
    private readonly T _value;
    private readonly int _weight;
    public Binding(T value, int weight = 0)
    {
        _value = value;
        _weight = weight;
    }

    public int Weight()
    {
        return _weight;
    }

    public T Value()
    {
        return _value;
    }
}