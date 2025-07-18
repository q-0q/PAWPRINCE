using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PowerTransformer : MonoBehaviour
{
    private PowerConnector _powerConnector;
    public Vector3 translation;
    public float lerpSpeed = 5f;

    private Vector3 _initPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;
        _powerConnector = GetComponentInChildren<PowerConnector>();
        if (_powerConnector is null) Debug.LogError("No PowerConnector child found in PowerTransformer");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 destination = _powerConnector.IsPowered() ? _initPosition + translation : _initPosition;
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * lerpSpeed);
    }
}
