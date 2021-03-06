﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_MusicButton : MonoBehaviour {

    public Color ColorOn;
    public Color ColorOff;

    private Image _image;

	void Start () {
        _image = GetComponent<Image>();
        UpdateUI();
	}
	
	public void UpdateUI() {
        if (LevelManager.Instance.optionsData.music_muted) _image.color = ColorOff;
        else _image.color = ColorOn;
	}
}
