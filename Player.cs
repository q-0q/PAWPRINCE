using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private Camera _camera;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Vector2 moveValue2 = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (moveValue2.magnitude < 0.01f) return;
        Vector3 camToPlayer = transform.position - _camera.transform.position;
        Vector3 projection = Vector3.Dot(camToPlayer, transform.up) * transform.up;
        Vector3 camForward = (camToPlayer - projection).normalized;
        Quaternion camForwardRotation = Quaternion.LookRotation(camForward, transform.up);
        Vector3 moveValue = camForwardRotation * new Vector3(moveValue2.x, 0, moveValue2.y);
        transform.position += moveValue * (Time.deltaTime * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveValue, transform.up), Time.deltaTime * 5f);
    }
}
