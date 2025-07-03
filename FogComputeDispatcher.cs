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
    
    public void Dispatch(FogVolume volume)
    {
        if (_computeShader is null) return;
        // if (_observerBuffer is null) return;
        
        _computeShader.SetTexture(_kernelID, "OcclusionTexture", volume.OcclusionTexture);
        _computeShader.SetTexture(_kernelID, "VolumeTexture", volume.VolumeTexture);
        _computeShader.SetInts("OcclusionTextureSize", volume.OcclusionTexture.width, volume.OcclusionTexture.height);
        
        var props = new MaterialPropertyBlock();
        props.SetTexture("_VolumeTexture", volume.VolumeTexture);
        volume.SetMeshRendererProps(props);
        
        int groupsX = Mathf.CeilToInt(volume.VolumeTexture.width / 8.0f);
        int groupsY = Mathf.CeilToInt(volume.VolumeTexture.height / 8.0f);
        int groupsZ = volume.VolumeTexture.volumeDepth;
        _computeShader.Dispatch(_kernelID, groupsX, groupsY, groupsZ);
    }

    public Material GetMaterial()
    {
        return _material;
    }
}
