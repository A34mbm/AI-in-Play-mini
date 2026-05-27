using UnityEngine;
using TMPro;
public class PlayerMove: MonoBehaviour
{
     // 現在いるブロック番号
    public int currentIndex = 0;

    // ブロック座標
    public Transform[] blocks;

    // カウント値
    public int score = 0;

    // UI表示
    public TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreUI();
    }

    // 移動処理
    public void MoveTo(int nextIndex, int value)
    {
        // 範囲チェック
        if (nextIndex < 0 || nextIndex >= blocks.Length)
            return;

        // 現在位置更新
        currentIndex = nextIndex;

        // 丸を移動
        transform.position = blocks[currentIndex].position;

        // 数字加算
        score += value;

        // UI更新
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score : " + score;
    }
}

