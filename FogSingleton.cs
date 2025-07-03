using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogSingleton : MonoBehaviour
{
    [System.NonSerialized]
    public static FogSingleton Singleton;
    private FogComputeDispatcher _computeDispatcher;
    private List<FogVolume> _volumes;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Debug.LogError("Multiple FogSingletons in the same scene");
            return;
        }
        
        Singleton = this;
        var computeShader = Resources.Load<ComputeShader>("Shaders/ComputeShaders/");
        var material = Resources.Load("Tes");
        
        // _computeDispatcher = new FogComputeDispatcher(computeShader, material);
    }

    // Start is called before the first frame update
    void Start()
    {
        _volumes = FindObjectsByType<FogVolume>(FindObjectsSortMode.None).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
