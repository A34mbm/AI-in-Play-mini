using UnityEngine;

public class ArrowButton: MonoBehaviour
{
    // どこのブロックへ進むか
    public int targetIndex;

    // 加算値
    public int addValue;

    // プレイヤー参照
    public PlayerMove player;

    void OnMouseDown()
    {
        player.MoveTo(targetIndex, addValue);
    }
}
