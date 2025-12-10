using UnityEngine;

public class GunShooting : MonoBehaviour
{
    [Header("발사 설정")]
    public float maxDistance = 100f; // 레이캐스트(총알) 최대 사거리
    public int scorePerHit = 1;      // 맞췄을 때 얻는 점수

    void Start()
    {
        // GunPickup 스크립트에서 활성화되기 전까지는 비활성화되어야 합니다.
        // (인스펙터 창에서 체크 해제하는 것을 잊지 마세요!)
        enabled = false; 
    }

    void Update()
    {
        // 마우스 왼쪽 버튼(Fire1) 클릭 감지
        if (enabled && Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 총 오브젝트의 위치와 앞 방향(transform.forward)을 기준으로 레이(광선)를 생성합니다.
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 레이캐스트 실행: 광선이 무언가에 맞았는지 확인합니다.
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // 맞은 오브젝트의 태그가 "Ball"인지 확인
            if (hit.collider.CompareTag("Ball"))
            {
                // 1. 점수 올리기: GameManager에게 1점을 추가하도록 요청합니다.
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(scorePerHit);
                }

                // 2. 공 파괴
                Destroy(hit.collider.gameObject);
                
                Debug.Log("공 명중! 점수 획득!");
            }
        }
    }
}