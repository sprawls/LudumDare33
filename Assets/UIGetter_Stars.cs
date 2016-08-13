using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGetter_Stars : MonoBehaviour {

    private Text text;

    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {
        if (LevelManager.Instance != null) text.text = LevelManager.Instance.GetTotalAmountStars().ToString();
    }
}
