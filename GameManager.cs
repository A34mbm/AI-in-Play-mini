using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour{
    public Edge selectedEdge;
    public TextMeshProUGUI scoreText;
    private int score = 0;


    public void SelectEdge(Edge edge){
        // まだ何も選択されていない
        if (selectedEdge == null){
            selectedEdge = edge;
            selectedEdge.Select();

            Debug.Log(edge.name + " を選択");
            }else{
            // 数字交換
            int temp = selectedEdge.value;
            selectedEdge.value = edge.value;
            edge.value = temp;

            // 表示更新
            selectedEdge.UpdateText();
            edge.UpdateText();

           // 色を戻す
           selectedEdge.Deselect();
           edge.Deselect();

           Debug.Log("交換した！");

           // 選択解除
           selectedEdge = null;
            }
    }
    public void AddScore(int value){
        score += value;
        scoreText.text = "Score : " + score;
        }
}