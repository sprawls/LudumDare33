using UnityEngine;
using System.Collections;

public class FusionSocket : ElementSocket {

    public GameObject FusionSocketParticles;

    public Element subElement_1;
    public Element subElement_2;

    public ElementTri Tri_1;
    public ElementTri Tri_2;

    private Vector3 tri_1_pos = Vector3.zero;
    private Vector3 tri_2_pos = Vector3.zero;

    public override void Start() {
        base.Start();
        
    }

    private void ChangeElement(ElementType newElem) {
        if (element.EType != newElem) {
            MusicManager.Instance.PlaySound_FusionChange();
            element.EType = newElem;
            Instantiate(FusionSocketParticles, element.transform.position, Quaternion.identity);
        }
    }

    private void MixElements() {
        if (subElement_1.EType == subElement_2.EType) ChangeElement(subElement_1.EType);
        else if (subElement_1.EType != ElementType.fire && subElement_2.EType != ElementType.fire) ChangeElement(ElementType.fire);
        else if (subElement_1.EType != ElementType.water && subElement_2.EType != ElementType.water) ChangeElement(ElementType.water);
        else if (subElement_1.EType != ElementType.wind && subElement_2.EType != ElementType.wind) ChangeElement(ElementType.wind);
        element.ResetModel();
        element.ScaleModel(Tri_1.size * 1.5f);
        PositionInMiddle();
    }

    private void PositionInMiddle() {
        Vector3 subDistance = tri_1_pos - tri_2_pos;
        Vector3 middlePos = (subDistance.normalized * (subDistance.magnitude * 0.5f)) + tri_2_pos;
        //Debug.Log("mid : " + middlePos + "   sub1 :" + tri_1_pos + "  sub2 " + tri_2_pos);
        element.transform.position = middlePos + new Vector3(0, 0, -5);
        element.Model.transform.position = element.transform.position;
    }

    public override Element GetElement(ElementTri triRef) {
        if (triRef == Tri_1) return subElement_1;
        else if (triRef == Tri_2) return subElement_2;
        else {
            Debug.Log("Get Element Ref wasn't found ! Returned Null !");
            return null;
        }
    }

    public override ElementType GetEType(ElementTri triRef) {
        MixElements();
        return element.EType;
    }

    public override void TranslateElement(Vector3 newPos, ElementTri triRef) {
        if (triRef == Tri_1) {
            tri_1_pos = newPos;
            subElement_1.TranslateToPosition(newPos);
        } else if (triRef == Tri_2) {
            tri_2_pos = newPos;
            subElement_2.TranslateToPosition(newPos);
        } else {
            Debug.Log("Get Element Ref wasn't found ! Did not Translate Anything !");
        }
        //Now put middle Orb inbetween both
        MixElements();
    }

    public override void ChangeElement(Element newElement, ElementTri triRef) {
        if (triRef == Tri_1) subElement_1 = newElement;
        else if (triRef == Tri_2) subElement_2 = newElement;
        else {
            Debug.Log("Get Element Ref wasn't found ! Did not Change Anything !");
        }
        MixElements();
    }

    //GIZMO DEBUG
    void OnDrawGizmos() {
        if (Application.isPlaying) return;
        Vector3 middlePos = ((Tri_1.transform.position + Tri_2.transform.position) / 2f);
        Vector3 middlePos_1 = (Tri_1.transform.position * 1.3f + Tri_2.transform.position * 0.7f) / 2f;
        Vector3 middlePos_2 = (Tri_1.transform.position * 0.7f + Tri_2.transform.position * 1.3f) / 2f;

        Gizmos.color = GetSocketGizmoColor(subElement_1);
        Gizmos.DrawSphere(middlePos_1, 0.50f * Tri_1.size);
        Gizmos.color = GetSocketGizmoColor(subElement_2);
        Gizmos.DrawSphere(middlePos_2, 0.50f * Tri_1.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(middlePos, 0.60f * Tri_1.size);


    }

    private Color GetSocketGizmoColor(Element el) {
        switch (el.EType) {
            case ElementType.fire:
                return Color.red;
            case ElementType.water:
                return Color.blue;
            case ElementType.wind:
                return Color.cyan;
            case ElementType.all:
                return Color.green;
            default:
                return Color.grey;
        }
    }
    
}
