using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float _moveSpeed;
    public float _rotationSpeed;
    
    private Camera _camera;
    private PlayerInput _playerInput;

    private Fsm<State, Trigger> _fsm;
    private Animator _animator;

    public enum State
    {
        Idle,
        Walking,
    }
    
    public enum Trigger
    {
        InputForward,
        InputNoDirection
    }
    
    void Start()
    {
        _camera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();

        _fsm = new Fsm<State, Trigger>(State.Idle);
        SetupStateMaps();
        SetupMachine();
    }

    void Update()
    {
        FireTriggers();
        _fsm.Update();
        
    }

    void SetupStateMaps()
    {
        _fsm.SetupStateMaps();
        _fsm.stateMapConfig.Name.Add(State.Idle, "Idle");
        _fsm.stateMapConfig.Behaviors.Add(State.Walking, WalkingBehavior);
    }
    
    void SetupMachine()
    {
        _fsm.SetupMachine();
        _fsm.machine.Configure(State.Idle)
            .Permit(Trigger.InputForward, State.Walking)
            .OnEntry(_ =>
            {
                _animator.SetTrigger("StartIdle");
                _animator.ResetTrigger("StartWalk");
            });
        
        _fsm.machine.Configure(State.Walking)
            .Permit(Trigger.InputNoDirection, State.Idle)
            .OnEntry(_ =>
            {
                _animator.ResetTrigger("StartIdle");
                _animator.SetTrigger("StartWalk");
            });
    }

    void WalkingBehavior()
    {
        var moveValue = ComputeMoveValue();
        if (moveValue.magnitude < 0.01f) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveValue, transform.up), Time.deltaTime * _rotationSpeed);
        transform.position += moveValue;
        
        var angle = Vector3.SignedAngle(transform.forward, moveValue, transform.up);
        var desiredTurnAmount = Mathf.InverseLerp(-80, 80, angle);
        var f = _animator.GetFloat("TurnAmount");
        var speed = Mathf.Abs(desiredTurnAmount - 0.5f) < Mathf.Abs(f - 0.5f) ? 25f : 8f;
        var turnAmount = Mathf.Lerp(f, desiredTurnAmount, Time.deltaTime * speed);
        _animator.SetFloat("TurnAmount", turnAmount);
    }

    
    private Vector3 ComputeMoveValue()
    {
        var desiredMove = DesiredMove();

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

    private Vector3 DesiredMove()
    {
        Vector2 moveValue2 = _playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 camToPlayer = transform.position - _camera.transform.position;
        Vector3 projection = Vector3.Dot(camToPlayer, transform.up) * transform.up;
        Vector3 camForward = (camToPlayer - projection).normalized;
        Quaternion camForwardRotation = Quaternion.LookRotation(camForward, transform.up);

        Vector3 moveInput = new Vector3(moveValue2.x, 0, moveValue2.y);
        Vector3 desiredMove = camForwardRotation * moveInput * (Time.deltaTime * _moveSpeed);
        return desiredMove;
    }

    void FireTriggers()
    {
        Vector2 moveValue2 = _playerInput.actions["Move"].ReadValue<Vector2>();
        _fsm.machine.Fire(moveValue2.magnitude < 0.05f ? Trigger.InputNoDirection : Trigger.InputForward);
    }
}
