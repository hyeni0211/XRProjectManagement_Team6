using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    // 공이 Trigger 영역(바구니)에 들어왔을 때 자동으로 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트(other)가 Rigidbody를 가지고 있는지 확인합니다.
        // Rigidbody가 있는 오브젝트는 보통 우리가 만든 '움직이는 공'입니다.
        // 이렇게 하면 바닥이나 다른 고정된 물체와 충돌해도 무시할 수 있습니다.
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Rigidbody를 가진 오브젝트, 즉 공을 제거(소멸)합니다.
            Destroy(other.gameObject);
            Debug.Log(other.gameObject.name + "이(가) 바구니에 들어가 소멸되었습니다.");
        }
    }
}