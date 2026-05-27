using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Edge : MonoBehaviour{
    public int value = 5;

    public TextMeshProUGUI valueText;

    public GameManager manager;

    private Image image;

    void Start(){
        UpdateText();
    }

    public void UpdateText(){
        valueText.text = value.ToString();
    }

    public void Select(){
        valueText.color = Color.yellow;
    }

    public void Deselect(){
        valueText.color = Color.white;
    }


    public void OnClick(){
        manager.SelectEdge(this);
        manager.AddScore(value);
    }
}