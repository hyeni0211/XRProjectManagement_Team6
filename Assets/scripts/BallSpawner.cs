using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    // 유니티 인스펙터 창에서 공 프리팹들을 할당할 배열입니다.
    public GameObject[] ballPrefabs; 

    // 공이 생성될 간격입니다. (초)
    public float spawnInterval = 2.0f;

    // 공이 생성될 위치 (기계의 배출구)
    public Transform spawnPoint; 

    // Start 메서드에서 코루틴을 시작하여 반복적인 생성 작업을 수행합니다.
    void Start()
    {
        StartCoroutine(SpawnBallRoutine());
    }

    // IEnumerator를 사용하여 일정 시간 간격으로 실행되는 함수를 만듭니다.
    IEnumerator SpawnBallRoutine()
    {
        // 무한 루프
        while (true) 
        {
            // 지정된 시간만큼 기다립니다.
            yield return new WaitForSeconds(spawnInterval); 
            
            SpawnRandomBall();
        }
    }

    void SpawnRandomBall()
    {
        if (ballPrefabs.Length == 0)
        {
            Debug.LogError("Ball Prefabs 배열이 비어있습니다. 프리팹을 할당하세요.");
            return;
        }

        // 1. ballPrefabs 배열에서 무작위 인덱스를 선택합니다.
        int randomIndex = Random.Range(0, ballPrefabs.Length);
        GameObject ballToSpawn = ballPrefabs[randomIndex];

        // 2. 선택된 공을 spawnPoint의 위치와 회전으로 생성합니다.
        // Quaternion.identity는 회전을 주지 않음을 의미합니다.
        Instantiate(ballToSpawn, spawnPoint.position, Quaternion.identity);
    }
}