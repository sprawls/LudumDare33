using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGetter_ParMoves : MonoBehaviour {

    private Text text;

    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {
        if(GameManager.Instance != null) text.text = GameManager.Instance.GetParMoves().ToString();
    }
}
