using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    // 속도를 높여서 테스트 (문제가 해결되면 원하는 값으로 낮추세요)
    public float speed = 15.0f; 
    public Vector3 direction = new Vector3(0, 0, 1); 

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // ----------------------------------------------------
            // **[진단 코드]** 코드가 실행될 때마다 Console에 메시지 출력
            Debug.Log(other.gameObject.name + " 공 감지 성공! 힘을 가하는 중.");
            // ----------------------------------------------------

            // 공의 Rigidbody가 정지 상태(Sleeping)인 경우 강제로 깨워서 힘을 받을 준비를 시킵니다.
            if (rb.IsSleeping()) 
            {
                rb.WakeUp();
            }

            // Z축 양수(바구니 방향) 월드 방향으로 변환
            Vector3 worldDirection = transform.TransformDirection(direction);

            // AddForce를 사용하여 강력하게 힘을 가합니다.
            rb.AddForce(worldDirection.normalized * speed * 10f, ForceMode.Acceleration);
        }
    }
}