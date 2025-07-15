using Unity.VisualScripting;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float forwardSpeed = 5f;   // 앞으로 이동 속도
    public float strafeSpeed = 3f;    // 좌우 이동 속도
    public float jumpForce = 5f;      // 점프 힘
    public int Hp;

    private Rigidbody rb;
    private float horizontalInput;
    private bool isGrounded;

    public Animator animator;
    public Animator Gunner;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다!");
    }

    private void Update()
    {
        // 좌우 입력 처리
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;
        if (Input.GetKeyDown(KeyCode.Mouse0)) Shut();


        MoveMent();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Shut()
    {
        Gunner.SetTrigger("Fire");
    }

    void MoveMent()
    {
        Vector3 forwardVelocity = transform.forward * forwardSpeed;
        Vector3 strafeVelocity = transform.right * horizontalInput * strafeSpeed;
        Vector3 finalVelocity = new Vector3(strafeVelocity.x, rb.linearVelocity.y, forwardVelocity.z);

        rb.linearVelocity = finalVelocity;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 바닥과 충돌했을 때만 점프 가능하도록 처리
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
            }
        }
    }
}
