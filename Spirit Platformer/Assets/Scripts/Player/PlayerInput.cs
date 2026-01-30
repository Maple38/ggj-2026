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
        if (_actionMove.IsPressed())
        {
            _playerMovement.Run(_actionMove.GetControlMagnitude());
        }
        else
        {
            _playerMovement.Decelerate();
        }

        if (_actionJump.WasPressedThisFrame())
        {
            _playerMovement.Jump();
        }
        else {_playerMovement.Fall();}
    }
}
