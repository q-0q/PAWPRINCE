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
        var heaviestBinding = _dictionary
            .Where(kv => fsm.machine.IsInState(kv.Key))
            .SelectMany(kv => kv.Value)
            .OrderByDescending(b => b.Weight())
            .FirstOrDefault();

        return heaviestBinding != null ? heaviestBinding.Value() : _default;
    }

    public void Add(TState state, T value, int weight = 0)
    {
        Debug.Log("Adding to state: " + state);
        if (!_dictionary.ContainsKey(state)) _dictionary[state] = new List<Binding<T>>();
        foreach (var _ in _dictionary[state].Where(binding => binding.Weight() == weight))
        {
            Debug.LogError("StateMap binding weight collision on weight " + weight);
        }
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