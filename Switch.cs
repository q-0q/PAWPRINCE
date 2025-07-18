using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private PowerConnector _powerConnector;

    private bool _on;

    private float _previousPlayerDistance = 0;
    private static float _threshholdDistance = 1.5f;
    private Transform _rotator;

    public List<PowerConnector> outputs;
    
    // Start is called before the first frame update
    void Start()
    {
        _on = false;
        _powerConnector = GetComponentInChildren<PowerConnector>();
        if (_powerConnector is null) Debug.LogError("No PowerConnector child found in Switch");
        _powerConnector.Source = true;
        _powerConnector.Disabled = !_on;
        _rotator = transform.Find("Rotator");
        
        foreach (var powerConnector in outputs)
        {
            powerConnector.AddInput(_powerConnector);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var playerDistance = Vector3.Distance(Player.Singleton.transform.position, transform.position);
        if (playerDistance < _previousPlayerDistance && playerDistance < _threshholdDistance &&
            _previousPlayerDistance > _threshholdDistance)
        {
            _on = !_on;
        }
        _previousPlayerDistance = playerDistance;
        _powerConnector.Disabled = !_on;

        float desiredZRot = _on ? 0 : 90f;
        float zRot = Mathf.Lerp(_rotator.localRotation.eulerAngles.z, desiredZRot, Time.deltaTime * 15f);
        _rotator.SetLocalPositionAndRotation(_rotator.localPosition, Quaternion.Euler(0, 0, zRot));
    }
}
