using UnityEngine;
using System.Collections;

public class ProgressBar3D : MonoBehaviour {

    private Transform _innerBar;
    private Transform _outerBar;

    private float _completionRatio = 0f;

	void Awake () {
        _outerBar = transform.Find("outerBar");
        _innerBar = _innerBar.Find("innerBar");

        if (_innerBar == null || _outerBar == null) Debug.LogError("Components of progress bars not found ! Check Hierarchy !");
        else {
            StartCoroutine(UpdateUIBar());
        }
	}

    IEnumerator UpdateUIBar() {
        //todo with www
        _completionRatio = Random.Range(0f, 1f);
        yield return null;

        UpdateInnerBar();
    }

    private void UpdateInnerBar() {
        Mathf.Clamp(_completionRatio, 0f, 1f);
        //set scale
        _innerBar.transform.localScale = new Vector3(_completionRatio, 1f, 1f);
        //set position
        _innerBar.transform.localPosition = new Vector3((1f - _completionRatio) * -0.5f, 0f, 0f);
    }
}
