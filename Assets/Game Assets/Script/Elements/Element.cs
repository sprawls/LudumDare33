using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum ElementType { fire = 0, water = 1, wind = 2, all = 3, none = -1 }

public class Element : MonoBehaviour {

    

    public ElementType EType;
    public GameObject Orb_Fire;
    public GameObject Orb_Water;
    public GameObject Orb_Wind;
    private GameObject Orb_All;

    public GameObject Model;

    void Awake() {
        transform.position = Vector3.zero;
        Orb_All = (GameObject) Resources.Load("Orbs/Orb_All"); //Load it here to avoid setting it in inspector.
    }

    void Start() {
        SetupOrb();
    }

    void OnValidate() {
        switch (EType) {
            case ElementType.fire:
                gameObject.name = "Fire Element";
                break;
            case ElementType.water:
                gameObject.name = "Water Element";
                break;
            case ElementType.wind:
                gameObject.name = "Wind Element";
                break;
            case ElementType.all:
                gameObject.name = "All Element";
                break;
            default :
                gameObject.name = "None Element";
                break;
        }
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
            case ElementType.all:
                GO = Orb_All;
                break;
        }
        if (GO == null) {
            Debug.Log("No model for Element Type : " + EType.ToString());
            return;
        }
        //Spawn it
        Model = (GameObject) Instantiate(GO,Vector3.zero,Quaternion.identity);
        Model.transform.localScale = Vector3.one * 1f;
        Model.transform.parent = transform;
        //Debug.Log("mod scale " + Model.transform.localScale + " name : " + Model.name);
    }

    public virtual void TranslateToPosition(Vector3 newPos) {
        Model.transform.DOMove(newPos, 1f, false);
    }

    public virtual void TranslateToPosition(Vector3 newPos, float time) {
        Model.transform.DOMove(newPos, time, false);
    }

    public void ScaleModel(float scale) {
        Model.transform.parent = null;
        Model.transform.localScale = Vector3.one * scale * (2f/3f);
        Model.transform.parent = transform;
    }

    public void ResetModel() {
        Destroy(Model);
        SetupOrb();
    }

}
