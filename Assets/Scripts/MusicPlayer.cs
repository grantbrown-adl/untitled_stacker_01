using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    [SerializeField] AudioClip[] songs;
    [SerializeField] TMP_Text songText;
    [SerializeField] TMP_Text volumeLevel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] float songTimer = 0.0f;
    [SerializeField] float songTime = 0.0f;
    [SerializeField] bool paused = false;
    private List<int> songOrder;
    private int currentSongIndex = 0;
    private int volumeInt = 100;

    const string baseText = "Volume: ";
    const string pausedText = "Music Paused";
    const string oneManSymphony = " - Composed by One Man Symphony - https://onemansymphony.bandcamp.com";

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (songs.Length == 0) {
            Debug.LogError("No songs assigned to the Music Player.");
            return;
        }

        foreach (var song in songs) {
            song.LoadAudioData();
        }

        ShuffleSongs();
        PlayNextSong();
    }

    void Update() {
        HandleInput();

        if (paused) return;

        songTimer += Time.deltaTime;

        if (songTimer > audioSource.clip.length) {
            PlayNextSong();
        }
    }

    private void HandleInput() {
        if (Input.GetButtonDown("Jump")) {
            paused = !paused;
            if (paused) audioSource.Pause();
            else audioSource.Play();

            songText.text = paused ? pausedText : audioSource.clip.name;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            volumeInt += 10;
            volumeInt = Mathf.Clamp(volumeInt, 0, 100);
            audioSource.volume = volumeInt / 100.0f;
            volumeLevel.text = $"{baseText}{volumeInt}";
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            volumeInt -= 10;
            volumeInt = Mathf.Clamp(volumeInt, 0, 100);
            audioSource.volume = volumeInt / 100.0f;
            volumeLevel.text = $"{baseText}{volumeInt}";
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            PlayNextSong();
            //paused = !paused;
            //if (paused) audioSource.Pause();
            //else audioSource.Play();

            //songText.text = paused ? pausedText : audioSource.clip.name;
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
        string suffix = audioSource.clip.name.Contains("Far The Days Come") ? "" : oneManSymphony;
        songText.text = paused ? pausedText : audioSource.clip.name + suffix;
        songTime = audioSource.clip.length;
        audioSource.Play();
        currentSongIndex++;
    }

    public void PlayClip(AudioClip clip) {
        sfxAudioSource.clip = clip;
        sfxAudioSource.Play();
    }
}