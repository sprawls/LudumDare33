using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum ElementType { fire = 0, water = 1, wind = 2 }

public class Element : MonoBehaviour {

    

    public ElementType EType;
    public GameObject Orb_Fire;
    public GameObject Orb_Water;
    public GameObject Orb_Wind;

    public GameObject Model;

    void Start() {
        SetupOrb();
    }

    private void SetupOrb() {
        GameObject GO = null;
        //Get Type
        switch (EType) {
            case ElementType.fire:
                GO = Orb_Fire;
                break;
            case ElementType.water:
                GO = Orb_Water;
                break;
            case ElementType.wind:
                GO = Orb_Wind;
                break;
        }
        //Spawn it
        Model = (GameObject) Instantiate(GO,transform.position,Quaternion.identity);
        Model.transform.localScale = Vector3.one * 1f;
        Model.transform.parent = transform;
        //Debug.Log("mod scale " + Model.transform.localScale + " name : " + Model.name);
    }

    public virtual void TranslateToPosition(Vector3 newPos) {
        Model.transform.DOMove(newPos, 1f, false);
    }

    public void ScaleModel(float scale) {
        Model.transform.parent = null;
        Model.transform.localScale = Vector3.one * scale * (2f/3f);
        Model.transform.parent = transform;
    }

}
