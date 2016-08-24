using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIGetter_Stars : MonoBehaviour {

    private static List<UIGetter_Stars> currentUIGetters = new List<UIGetter_Stars>();

    private Text text;

    void Awake() {
        text = GetComponent<Text>();
        currentUIGetters.Add(this);
    }

    void OnDestroy() {
        if (currentUIGetters.Contains(this)) {
            currentUIGetters.Remove(this);
        }
    }

    public static void UpdateAll() {
        foreach (UIGetter_Stars uigetter in currentUIGetters) {
            uigetter.UpdateText();
        }
    }

    void Start() {
        UpdateText();
    }

    void UpdateText() {
        if (LevelManager.Instance != null) text.text = LevelManager.Instance.GetTotalAmountStars().ToString();
    }
}
