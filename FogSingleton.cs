using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class FogSingleton : MonoBehaviour
{

    public Mesh VolumeMesh;
    private float _resolution = 16f;
    
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
        var computeShader = Resources.Load<ComputeShader>("Shaders/ComputeShaders/FogCompute");
        var material = Resources.Load<Material>("Shaders/Materials/FogMaterial");
        _computeDispatcher = new FogComputeDispatcher(computeShader, material);
    }
    
    void Start()
    {
        _volumes = FindObjectsByType<FogVolume>(FindObjectsSortMode.None).ToList();
    }

    public Material GetMaterial()
    {
        return _computeDispatcher.GetMaterial();
    }

    public float GetResolution()
    {
        return _resolution;
    }
    
    // Dispatch in LateUpdate so FogVolumes can update
    // their OcclusionTextures first
    void LateUpdate()
    {
        foreach (var volume in _volumes)
        {
            _computeDispatcher.Dispatch(volume);
        }
    }
}
