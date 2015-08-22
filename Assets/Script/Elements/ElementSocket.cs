using UnityEngine;
using System.Collections;

public class ElementSocket : MonoBehaviour {

    public Element element;

    public ElementType GetEType() {
        return element.EType;
    }

    public virtual void TranslateElement(Vector3 newPos) {
        element.TranslateToPosition(newPos);
    }

    public void ChangeElement(Element newElement) {
        element = newElement;
    }
}
