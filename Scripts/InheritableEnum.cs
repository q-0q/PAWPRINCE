using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public abstract class InheritableEnum
{
    // Static dictionary to track the next available value for each lineage (each branch of the hierarchy)
    private static readonly Dictionary<Type, int> lineageCounters = new Dictionary<Type, int>();

    static InheritableEnum()
    {
        
        // Get all subclasses of InheritableEnum (excluding abstract classes)
        var subclasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(InheritableEnum)) && !t.IsAbstract)
            .ToList();

        // Sort subclasses (optional, here just sorting alphabetically)
        subclasses.Sort((t1, t2) => string.Compare(t1.Name, t2.Name, StringComparison.Ordinal));

        // Collect the field info for each class in the correct order (base class first, then subclasses)
        foreach (var subclass in subclasses)
        {
            AssignFieldValues(subclass);
        }
    }

    // Assign values to all fields in the class (including inherited fields)
    private static void AssignFieldValues(Type subclass)
    {
        if (!lineageCounters.ContainsKey(subclass))
        {
            lineageCounters[subclass] = 0; // Start counting from 0 for each lineage
        }

        // Find all fields (including inherited ones) and order them by class hierarchy (base class first, then subclasses)
        var fields = subclass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.FieldType == typeof(int))
            .OrderBy(f => GetClassHierarchyIndex(f.DeclaringType))
            .ToList();

        foreach (var field in fields)
        {
            // Get the current counter for this subclass lineage
            int currentValue = lineageCounters[subclass];

            // Assign the current value to the field
            field.SetValue(null, currentValue);
            // Debug.Log($"{subclass.Name}.{field.Name}: {currentValue}");

            // Increment the counter for this particular subclass lineage
            lineageCounters[subclass]++;
        }
    }

    // Get the index of a class in the hierarchy (base class gets 0, subclasses get increasing indices)
    private static int GetClassHierarchyIndex(Type type)
    {
        int index = 0;
        // Traverse the class hierarchy to determine the "depth" of this class in the inheritance tree
        while (type != typeof(InheritableEnum) && type != null)
        {
            index++;
            type = type.BaseType;
        }
        return index;
    }

    // Static method to ensure initialization is triggered
    public static void Initialize()
    {
        // This will force the static constructor of BaseState to run
        var _ = Fsm.FsmState.Any;  // Trigger initialization by accessing a static member
    }

    // Method to retrieve the name of the field for a specific value
    public static string GetFieldNameByValue(int value, Type subclassType)
    {
        // Get all the fields of the given subclass type
        var fields = subclassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.FieldType == typeof(int))
            .ToList();

        // Search through fields to find the one that matches the value
        foreach (var field in fields)
        {
            int fieldValue = (int)field.GetValue(null);
            if (fieldValue == value)
            {
                return field.Name;
            }
        }

        return null;  // Return null if no field matches the value
    }
}

