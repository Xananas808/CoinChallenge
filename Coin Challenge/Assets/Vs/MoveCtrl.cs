using UnityEngine;
public class MoveCtrl : MonoBehaviour
{
    Vector3 playerInput;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] public Transform foot;
    [SerializeField] LayerMask mask;
    bool jumpButtonDown;
    [SerializeField] float speed, jumpForce, sensivity;
    float xRot;
    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f * 3, 0);
    }
    void Update()
    {
        playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        PlayerMove();
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
    bool IsGrounded()
    {
        return Physics.Raycast(foot.position, Vector3.down, 0.5f, mask);
    }
}