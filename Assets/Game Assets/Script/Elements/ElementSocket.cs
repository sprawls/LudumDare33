using UnityEngine;
using System.Collections;

public class ElementSocket : MonoBehaviour {

    public Element element;


    public virtual void Start() {

    }

    public virtual Element GetElement(ElementTri triRef) {
        return element;
    }

    public virtual ElementType GetEType(ElementTri triRef) {
        return element.EType;
    }

    public virtual void TranslateElement(Vector3 newPos, ElementTri triRef) {
        transform.position = newPos;
        element.TranslateToPosition(transform.position);
    }

    public virtual void ChangeElement(Element newElement, ElementTri triRef) {
        element = newElement;
    }


}
