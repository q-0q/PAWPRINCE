using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMap<T>
{
    private Dictionary<int, List<Binding<T>>> _dictionary;
    private T _default;
    
    public StateMap(T @default)
    {
        _dictionary = new Dictionary<int, List<Binding<T>>>();
        _default = @default;
    }
    
    public T Get(int state)
    {
        if (!_dictionary.TryGetValue(state, out var bindings))
            return _default;

        var heaviestBinding = bindings
            .OrderByDescending(b => b.Weight())
            .FirstOrDefault();

        return heaviestBinding != null ? heaviestBinding.Value() : _default;
    }

    public void Add(int state, T value, int weight = 0)
    {
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