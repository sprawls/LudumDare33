using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PopAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(AnimateScale());
        StartCoroutine(AnimateRotation());

        if (LevelManager.Instance.isInverseWorld()) gameObject.SetActive(false);
	} 
	
	
    IEnumerator AnimateScale() {
        Tweener t;
        while (true) {
            t = transform.DOScale(1.25f, 2f);
            t.SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(1f);
            transform.DOScale(1f, 2f);
            t.SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(1.5f);
        }
	}

    IEnumerator AnimateRotation() {
        Tweener t;
        while (true) {
            t = transform.DORotate(new Vector3(0f,0f,0f), 3f);
            t.SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(3f);
            t = transform.DORotate(new Vector3(0f, 0f, -6f), 3f);
            t.SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(3f);
        }
    }
}
