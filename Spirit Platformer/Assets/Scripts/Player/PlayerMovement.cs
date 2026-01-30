using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float gravity;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private int maxJumps;
    [SerializeField] private float timeToPeak;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float coyoteMax;
    [SerializeField] private float jumpBuffer;

    [Header("Run")]
    [SerializeField] private float runSpeedMax;
    [SerializeField] private float runAccel;
    [SerializeField] private float runDecel;
    [SerializeField] private float turnSpeedMult;

    private bool _grounded;
    private int _jumps;
    private float _coyote;
    private float _jumpBufferActive;
    private float _hVelocity;

    public void Run(float input)
    {
        if (!Mathf.Sign(_hVelocity).Equals(Mathf.Sign(input)))
        {
            _hVelocity += input * Time.deltaTime * turnSpeedMult * runAccel;
        }
        else
        {
            _hVelocity += input * Time.deltaTime * runAccel;
        }
    }

    public void Decelerate()
    {
        _hVelocity -= runDecel * Time.deltaTime;
    }

    public void Jump()
    {
        if (_grounded || _coyote > 0f)
        {
            if (_jumps > 0)
            {
                _jumps -= 1;
                ExecuteJump();
            }
        }
        else
        {
            _jumpBufferActive = jumpBuffer;
        }
    }

    // Runs the jump physics and nothing else
    private void ExecuteJump(float height)
    {
    }

    public void Ground()
    {
        _jumps = maxJumps;
        if (_jumpBufferActive > 0f)
        {
            _jumpBufferActive = 0;
            Jump();
        }
    }
}