using UnityEngine;
using System.Collections;


public class OffsetTexture : MonoBehaviour {
    private bool UseUVMethod = false;

    public float offsetX = 0f;
    public float offsetY = 0f;

    public Vector2 offsetRate;
    public bool randomizeStartOffset = false;

    private MeshRenderer _meshRenderer;
    private Mesh _mesh;
    private Vector2[] _meshUVs;

    void Start() {

        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _mesh = gameObject.GetComponent<MeshFilter>().mesh;
        _meshUVs = _mesh.uv;

        if (randomizeStartOffset) {
            if (UseUVMethod) {
                float randomOffsetX = Random.Range(0f, 1f);
                float randomOffsetY = Random.Range(0f, 1f);
                for (int i = 0; i < _meshUVs.Length; ++i) {
                    _meshUVs[i] = new Vector2(_meshUVs[i].x + randomOffsetY, _meshUVs[i].y + randomOffsetY);
                }
                _mesh.uv = _meshUVs;
            } else {
                offsetX = Random.Range(0f, 1f);
                offsetY = Random.Range(0f, 1f);
                _meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
                _meshRenderer.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
            }
        }
    }

	void Update () {
        if (UseUVMethod) {
            for (int i = 0; i < _meshUVs.Length; ++i) {
                _meshUVs[i] = new Vector2(_meshUVs[i].x + (offsetRate.x * Time.deltaTime), _meshUVs[i].y + (offsetRate.y * Time.deltaTime));
            }
            _mesh.uv = _meshUVs;
        } else {
            offsetX += offsetRate.x * Time.deltaTime;
            offsetY += offsetRate.y * Time.deltaTime;    
            _meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            _meshRenderer.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
        }
	}
}
