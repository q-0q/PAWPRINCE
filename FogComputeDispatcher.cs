using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogComputeDispatcher
{
    private Material _material;
    private ComputeShader _computeShader;
    private ComputeBuffer _observerBuffer;
    private int _kernelID;

    public FogComputeDispatcher(ComputeShader computeShader, Material material)
    {
        _computeShader = computeShader;
        _material = material;
        _kernelID = _computeShader.FindKernel("CSMain");
        _observerBuffer = null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
