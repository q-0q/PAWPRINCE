using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideInFog : MonoBehaviour
{
    private List<MeshRenderer> _meshRenderers;
    public FogVolume _fogVolume;
    
    void Start()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
    }

    void Update()
    {
        if (_fogVolume is null)
        {
            SetVisible(true);
            return;
        }
        
        foreach (var observer in FogSingleton.Singleton.observers)
        {
            if (Vector3.Distance(transform.position, observer.transform.position) > observer.Range) continue;
            var observerLocalPos = WorldSpaceToVolumePos(observer.transform.position);
            if (observerLocalPos.y > _fogVolume.ObserverHeightToIgnoreOcclusion)
            {
                SetVisible(true);
                return;
            }

            var localPos = WorldSpaceToVolumePos(transform.position);
            if (localPos.y > _fogVolume.ObserverHeightToIgnoreOcclusion)
            {
                SetVisible(false);
                return;
            }
            
            
            
            Vector3 dir = transform.position - observer.transform.position;
            if (Physics.Raycast(observer.transform.position, dir.normalized, out var hit, dir.magnitude - 0.25f, LayerMask.GetMask("Occluders")))
            {
                Debug.DrawLine(observer.transform.position, hit.point, Color.red);
            }
            else
            {
                SetVisible(true);
                return;
            }
        }
        
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = visible;
        }
    }

    Vector4 WorldSpaceToVolumePos(Vector3 position)
    {
        return (_fogVolume.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f));
    }
    
    
}
