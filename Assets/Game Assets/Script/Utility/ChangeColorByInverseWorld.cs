using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChangeColorByInverseWorld : MonoBehaviour {

    public bool ChangeText = false;
    public bool ChangeMeshRenderer = false;
    public bool ChangeImage = false;
    public bool ChangeSprite = false;
    public bool ChangeTextMesh = false;

    private static List<ChangeColorByInverseWorld> colorList;

    public Color normalColor;
    public Color InverseColor;

    private MeshRenderer meshRendrer;
    private Text text;
    private Image image;
    private SpriteRenderer sprite;
    private TextMesh textMesh;

	void Awake(){
        if (colorList == null) colorList = new List<ChangeColorByInverseWorld>();
        colorList.Add(this);
    }

    void Start() {
        if (ChangeText)         text = GetComponent<Text>();
        if (ChangeMeshRenderer) meshRendrer = GetComponent<MeshRenderer>();
        if (ChangeImage)        image = GetComponent<Image>();
        if (ChangeSprite)       sprite = GetComponent<SpriteRenderer>();
        if (ChangeTextMesh)     textMesh = GetComponent<TextMesh>();
        UpdateColor(0f);
    }

    void OnDestroy(){
        if(colorList.Contains(this)) colorList.Remove(this);
    }

    public static void UpdateAllColor(){
        foreach (ChangeColorByInverseWorld col in colorList) {
            col.UpdateColor(2f);
        }
    }

    public void UpdateColor(float time){
        Color TargetColor = LevelManager.Instance.isInverseWorld() ? InverseColor : normalColor;

        if (meshRendrer != null)    meshRendrer.material.DOColor(TargetColor, time);
        if (text != null)           text.DOColor(TargetColor, time);
        if (image != null)          image.DOColor(TargetColor, time);
        if (sprite != null)         sprite.DOColor(TargetColor, time);
        if (textMesh != null)       SimpleTextMeshColorLerp(TargetColor, time);
    }

    IEnumerator SimpleTextMeshColorLerp(Color targetColor, float time) {
        Color startColor = textMesh.color;
        for (float i = 0f; i < 1f; i += Time.deltaTime / time) {
            textMesh.color = Color.Lerp(startColor, targetColor, i);
            yield return null;
        }
        textMesh.color = targetColor;
    }
}
