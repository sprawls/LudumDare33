using UnityEngine;
using System.Collections;


public class OffsetTexture : MonoBehaviour {
    float offsetX = 0f;
    float offsetY = 0f;

    public Vector2 offsetRate;

	void Update () {
        offsetX += offsetRate.x * Time.deltaTime;
        offsetY += offsetRate.y * Time.deltaTime;
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        mesh.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
        mesh.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
	}


}
