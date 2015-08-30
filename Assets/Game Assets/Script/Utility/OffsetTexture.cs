using UnityEngine;
using System.Collections;


public class OffsetTexture : MonoBehaviour {
    public float offsetX = 0f;
    public float offsetY = 0f;

    public Vector2 offsetRate;
    public bool randomizeStartOffset = false;

    private MeshRenderer mesh;

    void Start() {

        mesh = gameObject.GetComponent<MeshRenderer>();
        if (randomizeStartOffset) {
            offsetX = Random.Range(0f, 1f);
            offsetY = Random.Range(0f, 1f);
            mesh.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            mesh.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
        }
    }

	void Update () {
        offsetX += offsetRate.x * Time.deltaTime;
        offsetY += offsetRate.y * Time.deltaTime;
        mesh.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
        mesh.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
	}


}
