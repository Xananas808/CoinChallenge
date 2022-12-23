using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class MoveCtrl : MonoBehaviour
{
    Vector3 _direction;
    Vector3 playerInput;
    [SerializeField] Camera cam;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] public Transform foot;
    [SerializeField] LayerMask mask;
    bool jumpButtonDown;
    [SerializeField] float speed, jumpForce, sensivity, turnSmoothVelocity;
    float turnSmoothTime = 0.05f;
    float xRot;
    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f * 3, 0);
    }
    void Update()
    {
        playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MovePlayer();
        if (Input.GetAxis("Jump") != 0)
        {
            if (!jumpButtonDown && IsGrounded())
            {
                Jump();
                jumpButtonDown = true;
            }
        }
        if (Input.GetAxis("Jump") == 0)
        {
            jumpButtonDown = false;
        }
    }
    void PlayerMove()
    {
        Vector3 moveVector = transform.TransformDirection(playerInput) * speed;
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        animator.SetBool("isRunning", moveVector != Vector3.zero);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public bool IsGrounded()
    {
        return Physics.Raycast(foot.position, Vector3.down, 0.5f, mask);
    }
    void MovePlayer()
    {
        _direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _direction = _direction.normalized;
        animator.SetBool("isRunning", _direction.magnitude > 0.1f);
        if (_direction.magnitude < 0.1f)
        {
            return;
        }
        float _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        float _anle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, _anle, 0f);
        Vector3 _moveDir = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        _moveDir = _moveDir.normalized;
        rb.MovePosition(transform.position + (_moveDir * speed * Time.deltaTime));
        
        bool _isGrounded = IsGrounded();

        animator.SetBool("IsJumping", !_isGrounded);


        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

}