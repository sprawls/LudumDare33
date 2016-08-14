using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ElementSocket : MonoBehaviour {

    public Element element;

    public virtual Color OrbBackgroundColor { get; protected set; }

    public virtual void Start() {
        OrbBackgroundColor = new Color(0f, 0f, 0f, 1f);
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

    public virtual void SetupElement(Vector2 newPos, ElementTri triRef) {
        transform.position = newPos;
        element.TranslateToPosition(transform.position, 0f);
    }

    public virtual void ChangeElement(Element newElement, ElementTri triRef) {
        element = newElement;
    }

    public virtual void ColorElements() {
        SpriteRenderer spriteRend = element.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer spriteRend_Foreground = spriteRend.transform.FindChild("Orb_Foreground").GetComponent<SpriteRenderer>();

        spriteRend.DOColor(OrbBackgroundColor, 1f);
        spriteRend_Foreground.DOColor(new Color(0, 0, 0, 0), 1f);
    }


}
