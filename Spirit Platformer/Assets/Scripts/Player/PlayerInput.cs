using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private float _stateMove;
    private bool _stateJump;
    
    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        _playerMovement.SetInput(_stateMove, _stateJump);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _stateMove = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _stateJump = context.performed;
    }
}
