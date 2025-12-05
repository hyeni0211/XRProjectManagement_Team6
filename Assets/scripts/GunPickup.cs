using UnityEngine;

public class GunPickup : MonoBehaviour
{
    // 인스펙터에서 연결 필수:
    // 1. handAttachmentPoint: 플레이어 카메라 아래의 GunHolder 오브젝트 연결
    // 2. shootingScript: 총 오브젝트 자체에 붙어있는 GunShooting 컴포넌트 연결
    public Transform handAttachmentPoint; 
    public GunShooting shootingScript;

    private bool canPickUp = false; 

    void Update()
    {
        // 1. 플레이어가 총 줍기 영역 안에 있고
        // 2. 'E' 키를 누르면
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            PickUpGun();
        }
    }

    // 플레이어가 총의 Trigger Collider 영역에 들어오면
    private void OnTriggerEnter(Collider other)
    {
        // *** 1단계 진단: 총에 무언가 닿았나요? ***
        Debug.Log("DEBUG: 총 콜라이더에 무언가 닿았습니다: " + other.gameObject.name + " (태그: " + other.tag + ")"); 
        
        // 'Player' 태그를 가진 오브젝트인지 확인
        if (other.CompareTag("Player")) 
        {
            canPickUp = true; // 줍기 가능 상태로 변경
            // *** 2단계 진단: 태그까지 정확히 인식했나요? ***
            Debug.Log("SUCCESS: 총을 주울 수 있는 상태가 되었습니다! (E 키를 누르세요)");
        }
    }

    // 플레이어가 총의 Trigger Collider 영역에서 나가면
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false; // 줍기 불가능 상태로 변경
        }
    }

    void PickUpGun()
    {
        // 1. 총의 물리 컴포넌트 제거 
        // Rigidbody는 인스펙터에서 제거되었는지 확인
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }
        
        canPickUp = false;

        // 2. 총 오브젝트를 플레이어의 손 위치(GunHolder)에 부착
        transform.SetParent(handAttachmentPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 3. 발사 기능 활성화 
        if (shootingScript != null)
        {
            shootingScript.enabled = true;
            Debug.Log("총 획득 완료! 발사 기능 활성화!");
        }
    }
}