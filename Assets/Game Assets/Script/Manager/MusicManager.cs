using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MusicManager : MonoBehaviour {

    public static MusicManager Instance;

    public AudioClip clip_music;
    public AudioClip clip_move;
    public AudioClip clip_fusionChange;
    public AudioClip clip_levelComplete;

    public AudioSource audioSource_music;
    public AudioSource audioSource_sounds;
    

    void Awake() {
        if (Instance == null) {
            MusicManager.Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {

        audioSource_music.loop = true;
        audioSource_music.clip = clip_music;
        audioSource_music.Play();
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    void OnLevelWasLoaded(int lvl) {
        if (lvl == 0 && Instance == this) {
            audioSource_music.DOFade(1,2f);
            audioSource_sounds.DOFade(1, 2f);
        }
    }

    public void PlaySound_Move(){
        if (clip_move != null) audioSource_music.PlayOneShot(clip_move);
    }

    public void PlaySound_FusionChange() {
        if (clip_fusionChange != null) audioSource_music.PlayOneShot(clip_fusionChange);
    }

    public void PlaySound_LevelComplete() {
        if (clip_levelComplete != null) audioSource_sounds.PlayOneShot(clip_levelComplete);
    }

    public void StopAllSounds(){
        audioSource_music.DOFade(0, 6f);
        audioSource_sounds.DOFade(0, 8f);
    }
}
