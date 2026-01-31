using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float skinWidth;
    
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
    
    private BoxCollider2D _collider;
    private float _gravity;
    private bool _grounded;
    private bool _groundedLast;
    private int _jumps;
    private float _coyote;
    private float _jumpBufferActive;
    private Vector2 _velocity;
    private int _collisionMask;
    private Rigidbody2D _rb;
    private Vector2 _collisionBox;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collisionMask = LayerMask.NameToLayer("Ground");
        _collider = GetComponent<BoxCollider2D>();
        _collisionBox = new Vector2(_collider.size.x - 2 * skinWidth, _collider.size.y - 2 * skinWidth);
        // g = 2h / t^2
        _gravity = 2 * jumpHeight / (timeToPeak * timeToPeak);
    }

    private void Update()
    {
        _velocity.x = Mathf.Clamp(_velocity.x, -runSpeedMax, runSpeedMax);
        
        _groundedLast = _grounded;
        ApplyFall();
        Move();
    }

    private void Move()
    {
        if (!Mathf.Approximately(_velocity.x, 0f))
        {
            CollisionHorizontal(ref _velocity.x);
        }      
        if (!Mathf.Approximately(_velocity.y, 0f))
        {
            CollisionHorizontal(ref _velocity.y);
        }
    }

    private void CollisionHorizontal(ref float velocity)
    {
        var hit = Physics2D.BoxCast(_rb.position, _collisionBox, 0, Vector2.right * velocity, velocity + skinWidth,
            _collisionMask);
        if (hit)
        {
            velocity = hit.distance;
        }
    }    
    private void CollisionVertical(ref float velocity)
    {
        var hit = Physics2D.BoxCast(_rb.position, _collisionBox, 0, Vector2.up * velocity, velocity + skinWidth,
            _collisionMask);
        if (hit)
        {
            velocity = hit.distance;
            _grounded = true;
            if (_grounded != _groundedLast)
            {
                Ground();
            }
        }
        else
        {
            _grounded = false;
        }
    }

    public void Run(float input)
    {
        float mult = 1;
        if (!Mathf.Sign(_velocity.x).Equals(Mathf.Sign(input)))
        {
            mult *= turnSpeedMult;
        }

        if (!_grounded)
        {
            mult *= airControlMult;
        }

        _velocity.x += input * Time.deltaTime * runAccel * mult;
    }

    public void Decelerate()
    {
        if (_grounded)
        {
            _velocity.x -= runDecel * Time.deltaTime;
        }
    }

    private void ApplyFall()
    {
            _velocity.y -= _gravity * gravityFallMult * Time.deltaTime;
            if (_velocity.y < terminalVelocity)
            {
                _velocity.y = terminalVelocity;
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

    // Runs the jump physics and nothing else
    private IEnumerator JumpCoroutine(float height, float maxTime)
    {
        // v0 = 2h / t
        _velocity.y = 2 * height / maxTime;
        if (_velocity.y > 0)
        {
            _velocity.y -= Time.deltaTime * _gravity;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Ground()
    {
        // _coyote = coyoteMax;
        _jumps = maxJumps;
        if (_jumpBufferActive > 0f)
        {
            _jumpBufferActive = 0;
            Jump();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireCube(_rb.position, _collider.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_rb.position, _collisionBox);
    }
}