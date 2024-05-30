using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour {
    public AudioClip[] songs; // Array to hold the songs
    private AudioSource audioSource;
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
            ShuffleSongs(); // Reshuffle if we reached the end of the playlist
            currentSongIndex = 0;
        }

        int songToPlayIndex = songOrder[currentSongIndex];
        audioSource.clip = songs[songToPlayIndex];
        audioSource.Play();
        currentSongIndex++;
    }
}