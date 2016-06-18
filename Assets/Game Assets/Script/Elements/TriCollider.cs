using UnityEngine;
using System.Collections;

public class TriCollider : MonoBehaviour {

    private ElementTri triCollider;

    void Start() {
        triCollider = GetComponentInParent<ElementTri>();
    }

    void OnMouseDown() {
        //Debug.Log("Clicked");    
        triCollider.RotateTri();
    }
}
