using UnityEngine;
using UnityEngine.UI; // 점수 표시를 위해 UI 라이브러리를 사용합니다.

public class ScoreManager : MonoBehaviour // 1. 클래스 이름 수정 완료
{
    // 1. 싱글톤: 어디서든 쉽게 접근할 수 있도록 인스턴스를 만듭니다.
    // **에러 발생 지점 수정:** GameManager -> ScoreManager로 변경
    public static ScoreManager Instance { get; private set; } 

    [Header("UI 설정")]
    // 인스펙터 창에서 점수 표시용 Text UI를 연결할 변수입니다.
    public Text scoreText; 

    private int currentScore = 0;

    void Awake()
    {
        // GameManager가 하나만 존재하도록 보장합니다.
        if (Instance == null)
        {
            // **에러 발생 지점 수정:** Instance = this;
            // 정확한 타입으로 할당
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI(); // 게임 시작 시 초기 점수(0점)를 표시합니다.
    }

    // GunShooting.cs에서 호출하여 점수를 올리는 함수입니다.
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currentScore.ToString();
        }
    }
}