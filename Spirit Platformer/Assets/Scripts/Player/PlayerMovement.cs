using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float gravityFallMult;
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
    [SerializeField] private float airControlMult;

    private float _gravity;
    private bool _grounded;
    private int _jumps;
    private float _coyote;
    private float _jumpBufferActive;
    private float _velocityH;
    private float _velocityV;

    private void Awake()
    {
        // g = 2h / t^2
        _gravity = 2 * jumpHeight / (timeToPeak * timeToPeak) ;
    }

    private void Update()
    {
        _velocityH = Mathf.Clamp(_velocityH, -runSpeedMax, runSpeedMax);
        if (GroundCheck())
        {
            Ground();
        }
    }
    
    public void Run(float input)
    {
        float mult = 1;
        if (!Mathf.Sign(_velocityH).Equals(Mathf.Sign(input)))
        { 
            mult *= turnSpeedMult;
        }

        if (!_grounded)
        {
            mult *= airControlMult;
        }
        _velocityH += input * Time.deltaTime * runAccel * mult;
    }

    public void Decelerate()
    {
        if (_grounded)
        {
            _velocityH -= runDecel * Time.deltaTime;
        }
    }

    public void Fall()
    {
        if (!_grounded)
        {
            _velocityV -= _gravity * gravityFallMult * Time.deltaTime;
            if (_velocityV < terminalVelocity)
            {
                _velocityV = terminalVelocity;
            }
        }
    }

    public void Jump()
    {
        if (_grounded || _coyote > 0f)
        {
            if (_jumps > 0)
            {
                _jumps -= 1;
                StartCoroutine(JumpCoroutine(jumpHeight, timeToPeak));
            }
        }
        else
        {
            _jumpBufferActive = jumpBuffer;
        }
    }

    private bool GroundCheck()
    {
        // TODO
        return true;
    }

    // Runs the jump physics and nothing else
    private IEnumerator JumpCoroutine(float height, float maxTime)
    {
        // v0 = 2h / t
        _velocityV = 2 * height / maxTime;
        if (_velocityV > 0)
        {
            _velocityV -= Time.deltaTime * _gravity;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Ground()
    {
        _coyote = coyoteMax;
        _jumps = maxJumps;
        if (_jumpBufferActive > 0f)
        {
            _jumpBufferActive = 0;
            Jump();
        }
    }
}