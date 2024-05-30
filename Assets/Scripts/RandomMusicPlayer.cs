using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour {
    [SerializeField] AudioClip[] songs;
    [SerializeField] TMP_Text text;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource sfxAudioSource;
    private List<int> songOrder;
    private int currentSongIndex = 0;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (songs.Length == 0) {
            Debug.LogError("No songs assigned to the Music Player.");
            return;
        }

        ShuffleSongs();
        PlayNextSong();
    }

    void Update() {

        if (Input.GetButtonDown("Jump")) {
            PlayNextSong();
        }

        if (!audioSource.isPlaying) {
            PlayNextSong();
        }
    }

    void ShuffleSongs() {
        songOrder = new List<int>();
        for (int i = 0; i < songs.Length; i++) {
            songOrder.Add(i);
        }

        for (int i = 0; i < songOrder.Count; i++) {
            int temp = songOrder[i];
            int randomIndex = Random.Range(0, songOrder.Count);
            songOrder[i] = songOrder[randomIndex];
            songOrder[randomIndex] = temp;
        }
    }

    void PlayNextSong() {
        if (currentSongIndex >= songOrder.Count) {
            ShuffleSongs();
            currentSongIndex = 0;
        }

        int songToPlayIndex = songOrder[currentSongIndex];
        audioSource.clip = songs[songToPlayIndex];
        text.text = songs[songToPlayIndex].name;
        audioSource.Play();
        currentSongIndex++;
    }

    public void PlayClip(AudioClip clip) {
        sfxAudioSource.clip = clip;
        sfxAudioSource.Play();
    }
}