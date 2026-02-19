using System.Collections;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    // 행성 자전 속도 
    [SerializeField] float rotSpeed = 18;

    Coroutine rotationRoutine;



    private void Start()
    {

    }

    private void Update()
    {
        CALLBACK_Update();
    }

    private void CALLBACK_Update()
    {
        RotationInY();
    }

    private void RotationInY()
    {
        // 쿼터니언을 벡터로 바꾸고 덧셈 계산후 다시 쿼터니언으로 바꾸는것은 불필요한 메모리 할당

        Quaternion deltaRotation = Quaternion.Euler(0, rotSpeed * Time.deltaTime, 0);
        transform.rotation *= deltaRotation;
    }

    private void OnDestroy()
    {

    }
}
