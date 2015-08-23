﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGetter_Score : MonoBehaviour {

    private Text text;

    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {
        text.text = "+" + GameManager.Instance.Score.ToString();
    }

}
