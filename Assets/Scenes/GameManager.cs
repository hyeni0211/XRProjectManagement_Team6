using UnityEngine;
using TMPro; // TextMeshPro를 쓰려면 이게 꼭 필요합니다.
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // [수정된 부분] Text -> TMP_Text로 변경!
    // TMP_Text는 3D 텍스트든 UI 텍스트든 다 받아주는 만능형입니다.
    public TMP_Text scoreText; 
    
    public int score = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        // 점수 표시 (숫자를 2자리로 예쁘게: 00, 01, 10...)
        scoreText.text = "럭비공만 맞추세요! 점수: " + score.ToString("D2");
    }
}