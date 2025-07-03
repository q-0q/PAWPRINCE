using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

public class FogVolume : MonoBehaviour
{
    public float OcclusionFarClipPlane = 1000f;
    public float OcclusionNearClipPlane = -1000f;
    
    [System.NonSerialized]
    public RenderTexture VolumeTexture;
    [System.NonSerialized]
    public RenderTexture OcclusionTexture;

    public Vector3 OcclusionCameraRotationEulers = new Vector3(90, 0, 0);
    
    private Camera _occlusionCamera;
    private MeshRenderer _meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _occlusionCamera = CreateOcclusionCamera();
        VolumeTexture = CreateVolumeTexture(256);
        OcclusionTexture = CreateOcclusionTexture(256);
        
        _occlusionCamera.targetTexture = OcclusionTexture;
        
        // TODO: make meshRenderer as code
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (_meshRenderer == null)
        {
            Debug.LogError("FogVolume has no MeshRenderer in its children");
            return;
        }
        
        _meshRenderer.SetMaterials(new List<Material>(){FogSingleton.Singleton.GetMaterial()});
    }
    
    private RenderTexture CreateVolumeTexture(int size)
    {
        var rt = new RenderTexture(size, size, 0, GraphicsFormat.R8G8B8A8_UNorm)
        {
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            volumeDepth = size,
            enableRandomWrite = true,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        rt.Create();
        return rt;
    }
    
    private RenderTexture CreateOcclusionTexture(int size)
    {
        var rt = new RenderTexture(size, size, 0, GraphicsFormat.R8G8B8A8_UNorm)
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
        occlusionCamera.orthographicSize = 15;
        occlusionCamera.nearClipPlane = OcclusionNearClipPlane;
        occlusionCamera.farClipPlane = OcclusionFarClipPlane;
        occlusionCamera.targetTexture = OcclusionTexture;
        occlusionCamera.cullingMask = LayerMask.GetMask("Occluders");

        return occlusionCamera;
    }

    public Bounds GetWorldBounds()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Bounds worldBounds = meshRenderer.bounds;
        return worldBounds;
    }

    public void SetMeshRendererProps(MaterialPropertyBlock props)
    {
        _meshRenderer.SetPropertyBlock(props);
    }
}