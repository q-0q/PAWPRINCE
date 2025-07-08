using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float _moveSpeed;
    public float _rotationSpeed;
    
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
        var moveValue = ComputeMoveValue();
        if (moveValue.magnitude < 0.01f) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveValue, transform.up), Time.deltaTime * _rotationSpeed);
        transform.position += moveValue;
    }

    private Vector3 ComputeMoveValue()
    {
        Vector2 moveValue2 = _playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 camToPlayer = transform.position - _camera.transform.position;
        Vector3 projection = Vector3.Dot(camToPlayer, transform.up) * transform.up;
        Vector3 camForward = (camToPlayer - projection).normalized;
        Quaternion camForwardRotation = Quaternion.LookRotation(camForward, transform.up);

        Vector3 moveInput = new Vector3(moveValue2.x, 0, moveValue2.y);
        Vector3 desiredMove = camForwardRotation * moveInput * (Time.deltaTime * _moveSpeed);

        // Radius of your character (adjust as needed)
        float radius = 0.3f;
        float castDistance = desiredMove.magnitude;

        Vector3 position = transform.position;
        Vector3 direction = desiredMove.normalized;

        // SphereCast to account for player volume
        if (Physics.SphereCast(position, radius, direction, out RaycastHit hit, castDistance))
        {
            // First collision: slide along the surface
            Vector3 firstNormal = hit.normal;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, Vector3.ProjectOnPlane(firstNormal, transform.up));

            // Cast again in the new direction to handle corner (second surface)
            if (Physics.SphereCast(position, radius, desiredMove.normalized, out RaycastHit secondHit, desiredMove.magnitude))
            {
                Vector3 secondNormal = secondHit.normal;

                // Slide again
                desiredMove = Vector3.ProjectOnPlane(desiredMove, Vector3.ProjectOnPlane(secondNormal, transform.up));

                // Optional: stop completely if movement is almost gone
                if (desiredMove.magnitude < 0.01f)
                    desiredMove = Vector3.zero;
            }
        }

        return desiredMove;
    }

    
}
