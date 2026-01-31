using System;
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
    private LayerMask _collisionMask;
    private Rigidbody2D _rb;
    private Vector2 _collisionBox;
    private float _inMove;
    private bool _inJump;

    private void Awake()
    {
        CacheVariables();
    }

    private void Update()
    {
        _velocity.x = Mathf.Clamp(_velocity.x, -runSpeedMax, runSpeedMax);
        _groundedLast = _grounded;
    }

    private void FixedUpdate()
    {
        HandleInput();
        ApplyGravity();
        Move(Time.fixedDeltaTime);
    }

    public void SetInput(float move, bool jump)
    {
        _inMove = move;
        if (jump)
        {
            _inJump = true;
        }
    }

    private void HandleInput()
    {
        if (Mathf.Abs(_inMove) > 0.01f)
        {
            Run(_inMove);
        }
        else
        {
            Decelerate();
        }

        if (_inJump)
        {
            Jump();
            _inJump = false;
        }
    }

    private void CacheVariables()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collisionMask = LayerMask.GetMask("Ground");
        _collider = GetComponent<BoxCollider2D>();
        _collisionBox = new Vector2(_collider.size.x - (2 * skinWidth), _collider.size.y - (2 * skinWidth));
        // g = 2h / t^2
        _gravity = 2 * jumpHeight / (timeToPeak * timeToPeak);
    }

    private void Move(float delta)
    {
        Vector2 deltaMove = _velocity;
        
        if (!Mathf.Approximately(_velocity.x, 0f))
        {
            CollisionHorizontal(ref deltaMove.x);
        }
        if (!Mathf.Approximately(_velocity.y, 0f))
        {
            CollisionVertical(ref deltaMove.y);
        }

        _rb.MovePosition(_rb.position + deltaMove);
    }

    private void CollisionHorizontal(ref float delta)
    {
        var direction = Mathf.Sign(delta);
        var hit = Physics2D.BoxCast(_rb.position, _collisionBox, 0, 
            Vector2.right * direction, Mathf.Abs(delta) + skinWidth,
            _collisionMask);
        if (hit)
        {
            delta = (hit.distance - skinWidth) * direction;
            _velocity.x = 0;
        }
    }

    private void CollisionVertical(ref float delta)
    {
        var direction = Mathf.Sign(delta);
        var hit = Physics2D.BoxCast(_rb.position, _collisionBox, 0, 
            Vector2.up * direction, Mathf.Abs(delta) + skinWidth,
            _collisionMask);
        if (hit)
        {
            delta = (hit.distance - skinWidth) * direction;
            _velocity.y = 0;
            
            if ((int)direction == -1)
            {
                _grounded = true;
                if (_grounded != _groundedLast)
                {
                    Ground();
                }
            }
        }
        else
        {
            _grounded = false;
        }
    }

    private void Run(float input)
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

    private void Decelerate()
    {
        if (_grounded)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, runDecel * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        var mult = (_velocity.y < 0f) ? gravityFallMult : 1f; 
        _velocity.y -= _gravity * mult * Time.deltaTime;
        if (_velocity.y < terminalVelocity)
        {
            _velocity.y = terminalVelocity;
        }
    }

    private void Jump()
    {
        if (_grounded || _coyote > 0f)
        {
            if (_jumps > 0)
            {
                _jumps -= 1;
                // v0 = 2h / t
                _velocity.y = 2 * jumpHeight / timeToPeak;
            }
        }
        else
        {
            _jumpBufferActive = jumpBuffer;
        }
    }
    
    private void Ground()
    {
        // _coyote = coyoteMax;
        _jumps = maxJumps;
        if (_jumpBufferActive > 0f)
        {
            _jumpBufferActive = 0;
            Jump();
        }
    }

    private void OnValidate()
    {
        CacheVariables();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireCube(_rb.position, _collider.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_rb.position, _collisionBox);
    }
}