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
        _computeShader.SetInts("VolumeTextureSize", volume.VolumeTexture.width, volume.VolumeTexture.height, volume.VolumeTexture.volumeDepth);
        
        var props = new MaterialPropertyBlock();
        props.SetTexture("_VolumeTexture", volume.VolumeTexture);
        props.SetVector("_VolumeTextureSize", new Vector4(volume.VolumeTexture.width, volume.VolumeTexture.height, volume.VolumeTexture.volumeDepth));
        volume.SetMeshRendererProps(props);
        
        int groupsX = Mathf.CeilToInt(volume.VolumeTexture.width / 8.0f);
        int groupsY = Mathf.CeilToInt(volume.VolumeTexture.height / 8.0f);
        int groupsZ = volume.VolumeTexture.volumeDepth;
        _computeShader.Dispatch(_kernelID, groupsX, groupsY, groupsZ);
    }
    
    public void SetObservers(List<Observer> observers)
    {
        if (_observerBuffer != null)
            _observerBuffer.Release();
    
        _computeShader.SetInt("ObserverCount", observers.Count);
    
        _observerBuffer = new ComputeBuffer(observers.Count, sizeof(float) * 4);
        Vector4[] data = new Vector4[observers.Count];
        for (int i = 0; i < observers.Count; i++)
        {
            var pos = observers[i].transform.position;
            data[i] = new Vector4(pos.x, pos.y, pos.z, observers[i].Range); // x, z, y, radius
        }
    
        _observerBuffer.SetData(data);
        
    }

    public Material GetMaterial()
    {
        return _material;
    }
}
