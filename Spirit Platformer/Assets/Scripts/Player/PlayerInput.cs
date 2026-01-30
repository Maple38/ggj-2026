using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private InputAction _actionMove;
    private InputAction _actionJump;
    
    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        _actionMove = InputSystem.actions.FindAction("Move");
        _actionJump = InputSystem.actions.FindAction("Jump");
    }
    void Update()
    {
        // Vector2 move = _actionMove.ReadValue<Vector2>();
        // if (move > 0)
        // {
        //     _playerMovement.Run(move);
        // }
        // else
        // {
        //     _playerMovement.Decelerate();
        // }

        if (_actionJump.WasPerformedThisFrame())
        {
            _playerMovement.Jump();
        }
        else {_playerMovement.Fall();}
    }
}
