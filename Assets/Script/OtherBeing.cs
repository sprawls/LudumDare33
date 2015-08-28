using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OtherBeing : MonoBehaviour {

    public void Kill() {
        ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem sys in systems) {
            sys.enableEmission = false;
        }

        SpriteRenderer SR = gameObject.GetComponentInChildren<SpriteRenderer>();
        SR.DOFade(0, 3f);
    }
}
