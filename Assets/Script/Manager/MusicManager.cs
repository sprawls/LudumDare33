using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public static MusicManager Instance;

    public AudioClip clip_music;
    public AudioClip clip_move;
    public AudioClip clip_fusionChange;
    public AudioClip clip_levelComplete;
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
        audioSource.clip = clip_music;
        audioSource.Play();
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void PlaySound_Move(){
        audioSource.PlayOneShot(clip_move);
    }

    public void PlaySound_FusionChange() {
        audioSource.PlayOneShot(clip_fusionChange);
    }

    public void PlaySound_LevelComplete() {
        audioSource.PlayOneShot(clip_levelComplete);
    }
}
