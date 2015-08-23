using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public static MusicManager Instance;

    public AudioClip musicClip;
    private AudioSource audioSource;
    

    void Awake() {
        if (Instance == null) {
            MusicManager.Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.clip = musicClip;
        audioSource.Play();
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }
}
