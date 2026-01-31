using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private InputAction _actionMove;
    private InputAction _actionJump;

    private float _stateMove;
    
    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        _playerMovement.SetInput(_actionMove.ReadValue<Vector2>().x, _actionJump.WasPerformedThisFrame());
    }
}
