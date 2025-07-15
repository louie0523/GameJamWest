using UnityEngine;

public class Move : MonoBehaviour
{
    public float forwardSpeed = 5f;   // 앞으로 이동 속도
    public float strafeSpeed = 3f;    // 좌우 이동 속도
    public int Hp;

    private Rigidbody rb;
    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다!");
    }

    private void Update()
    {
        // 좌우 입력 처리 (A: -1, D: +1)
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        MoveMent();
    }

   void MoveMent()
    {
        // 앞으로 전진
        Vector3 forwardVelocity = transform.forward * forwardSpeed;

        // 좌우 이동
        Vector3 strafeVelocity = transform.right * horizontalInput * strafeSpeed;

        // 최종 이동 벡터
        Vector3 finalVelocity = new Vector3(strafeVelocity.x, rb.linearVelocity.y, forwardVelocity.z);

        // Rigidbody에 적용
        rb.linearVelocity = finalVelocity;
    }
}
