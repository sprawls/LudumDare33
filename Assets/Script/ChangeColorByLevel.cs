using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChangeColorByLevel : MonoBehaviour {

    private static List<ChangeColorByLevel> colorList;

    public Color StartColor;
    public Color EndColor;

    private MeshRenderer meshRendrer;



	void Awake(){
        if(colorList == null) colorList = new List<ChangeColorByLevel>();
        colorList.Add(this);
    }

    void Start() {
        meshRendrer = GetComponent<MeshRenderer>();
    }

    void OnDestroy(){
        if(colorList.Contains(this)) colorList.Remove(this);
    }

    public static void UpdateAllColor(){
        foreach(ChangeColorByLevel col in colorList){
            col.UpdateColor();
        }
    }

    public void UpdateColor(){
        Color TargetColor = Color.Lerp(StartColor, EndColor, (float)GameManager.Instance.currentLevel / (float)GameManager.Instance.LevelsList.Count);
        meshRendrer.material.DOColor(TargetColor, 2f);
    }
}
