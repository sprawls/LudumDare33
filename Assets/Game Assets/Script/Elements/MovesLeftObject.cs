using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovesLeftObject : MonoBehaviour {

    private TextMesh _textMesh;
    private ParticleSystem _sparkEmitter;
    private float _startScale;
    private float _scaleTime = 0.16f;
    

    public float scaleFactor = 1.8f;

	// Use this for initialization
	void Awake () {
        _textMesh = GetComponent<TextMesh>();
        _sparkEmitter = transform.parent.GetComponent<ParticleSystem>();
        _startScale = transform.localScale.x;

        if (_textMesh == null || _sparkEmitter == null) {
            Debug.Log("Missing components !");
        }
	}

    public void UpdateText(string newText) {
        StartCoroutine(UpdateVisuals(newText));
    }

    private IEnumerator UpdateVisuals(string newText) {
        _sparkEmitter.Emit(Random.Range(10, 16));

        transform.DOScale(_startScale * scaleFactor, _scaleTime);

        yield return new WaitForSeconds(0.1f);

        transform.DOScale(_startScale, _scaleTime * 2f);
        _textMesh.text = newText;
    }
}
