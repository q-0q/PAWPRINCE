using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

public class FogVolume : MonoBehaviour
{
    public float OcclusionFarClipPlane = 1000f;
    public float OcclusionNearClipPlane = -1000f;
    
    // [System.NonSerialized]
    public RenderTexture VolumeTexture;
    // [System.NonSerialized]
    public RenderTexture OcclusionTexture;

    public Vector3 OcclusionCameraRotationEulers = new Vector3(90, 0, 0);
    
    private Camera _occlusionCamera;
    private MeshRenderer _meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _occlusionCamera = CreateOcclusionCamera();
        VolumeTexture = CreateVolumeTexture();
        OcclusionTexture = CreateOcclusionTexture();
        _meshRenderer = CreateMeshRenderer();
        
        _occlusionCamera.targetTexture = OcclusionTexture;
    }
    private RenderTexture CreateVolumeTexture()
    {
        var size = transform.localScale * FogSingleton.Singleton.GetResolution();
        
        var rt = new RenderTexture((int)size.x, (int)size.y, 0, GraphicsFormat.R8G8B8A8_UNorm)
        {
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            volumeDepth = (int)size.z,
            enableRandomWrite = true,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        rt.Create();
        return rt;
    }
    
    private RenderTexture CreateOcclusionTexture()
    {
        var size = transform.localScale * FogSingleton.Singleton.GetResolution();
        Debug.Log(transform.localScale + " : " + FogSingleton.Singleton.GetResolution());
        
        var rt = new RenderTexture((int)size.x, (int)size.z, 0, GraphicsFormat.R8G8B8A8_UNorm)
        {
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            enableRandomWrite = true,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
        };
        rt.Create();
        return rt;
    }

    private Camera CreateOcclusionCamera()
    {

        var occlusionCameraGameObject = new GameObject("OcclusionCamera");
        var occlusionCamera = occlusionCameraGameObject.AddComponent<Camera>();
        
        occlusionCameraGameObject.transform.SetParent(transform);
        occlusionCameraGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(OcclusionCameraRotationEulers));

        occlusionCamera.orthographic = true;
        occlusionCamera.orthographicSize = 5;
        occlusionCamera.nearClipPlane = OcclusionNearClipPlane;
        occlusionCamera.farClipPlane = OcclusionFarClipPlane;
        occlusionCamera.targetTexture = OcclusionTexture;
        occlusionCamera.cullingMask = LayerMask.GetMask("Occluders");

        return occlusionCamera;
    }

    private MeshRenderer CreateMeshRenderer()
    {

        foreach (var c in GetComponentsInChildren<MeshRenderer>())
        {
            CleanUpComponent(c);
        }
        
        foreach (var c in GetComponentsInChildren<MeshFilter>())
        {
            CleanUpComponent(c);
        }
        
        var meshRendererGameObject = new GameObject("MeshRenderer");
        var meshRenderer = meshRendererGameObject.AddComponent<MeshRenderer>();
        
        meshRendererGameObject.transform.SetParent(transform);
        meshRendererGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        meshRendererGameObject.transform.localScale = Vector3.one;
        
        
        var meshFilter = meshRendererGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = FogSingleton.Singleton.VolumeMesh;
        meshRenderer.SetMaterials(new List<Material>(){FogSingleton.Singleton.GetMaterial()});
        
        return meshRenderer;
    }
    
    public void SetMeshRendererProps(MaterialPropertyBlock props)
    {
        _meshRenderer.SetPropertyBlock(props);
    }

    private void CleanUpComponent(Component component)
    {
        Debug.Log("Removing Editor-created " + component.GetType() + " from FogVolume");
        if (component.gameObject != gameObject) Destroy(component.gameObject);
        Destroy(component);
    }
}