using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLine : MonoBehaviour
{
    private PowerConnector _powerConnectorA;
    private PowerConnector _powerConnectorB;
    private LineRenderer _lineRenderer;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _powerConnectorA = transform.Find("PowerConnectorA").GetComponent<PowerConnector>();
        _powerConnectorB = transform.Find("PowerConnectorB").GetComponent<PowerConnector>();
        _powerConnectorA.AddInput(_powerConnectorB);
        _powerConnectorB.AddInput(_powerConnectorA);
    }

    void Update()
    {
        Color color = _powerConnectorA.IsPowered() || _powerConnectorB.IsPowered() ? Color.cyan : Color.gray;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
    
}
