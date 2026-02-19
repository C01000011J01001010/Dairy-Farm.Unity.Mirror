using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMove : MonoBehaviour
{
    [Header("설정")]
    public float speed = 8f;

    Rigidbody2D rb;
    Vector2 inputVec;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InputVec(Vector2 input)
    {
        inputVec = input;
    }

    void FixedUpdate()
    {
        // 2. 이동 처리 (물리 연산은 FixedUpdate에서 수행)
        // 대각선 이동 속도가 빨라지지 않게 normalized를 사용합니다.
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        // 현재 위치 + 이동량을 더해 부드럽게 이동시킵니다.
        rb.MovePosition(rb.position + nextVec);
    }
}
