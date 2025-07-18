using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerConnector : MonoBehaviour
{

    public bool Source = false;
    
    private HashSet<PowerConnector> _spatialInputs;
    private HashSet<PowerConnector> _inputs;

    public bool Disabled = false;
    private bool _powered;
    
    void Awake()
    {
        _inputs = new HashSet<PowerConnector>();
        _spatialInputs = new HashSet<PowerConnector>();
    }
    
    void Update()
    {
        _spatialInputs = FindSpatialInputs();
        
        var color = IsPowered() ? Color.yellow : Color.red;
        var mat = GetComponent<MeshRenderer>().material;
        mat.color = color;
    }

    public void AddInput(PowerConnector input)
    {
        _inputs.Add(input);
    }
    
    public void RemoveInput(PowerConnector input)
    {
        _inputs.Remove(input);
    }

    public bool IsPowered()
    {
        HashSet<PowerConnector> visited = new HashSet<PowerConnector>();
        return IsPoweredRecursive(this, visited);
    }

    private bool IsPoweredRecursive(PowerConnector current, HashSet<PowerConnector> visited)
    {
        // Avoid infinite loops (cycles)
        if (!visited.Add(current))
            return false;

        // Base case: if current connector is a source
        if (current.Source && !current.Disabled)
            return true;

        // Combine all inputs
        HashSet<PowerConnector> combined = new HashSet<PowerConnector>(current._spatialInputs);
        combined.UnionWith(current._inputs);

        // DFS: check each connected input recursively
        foreach (PowerConnector input in combined)
        {
            if (IsPoweredRecursive(input, visited))
                return true;
        }

        // No path leads to a powered source
        return false;
    }

    private HashSet<PowerConnector> FindSpatialInputs()
    {
        return Physics.OverlapSphere(transform.position, 0.75f, LayerMask.GetMask("PowerConnectors"))
            .Select(hit => hit.GetComponent<PowerConnector>())
            .Where(pc => pc != null && pc != this)
            .ToHashSet();
    }
    
}
