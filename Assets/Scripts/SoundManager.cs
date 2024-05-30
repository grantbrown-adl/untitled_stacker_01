using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _currentVolume;

    private static SoundManager _instance;

    public static SoundManager Instance { get => _instance; private set => _instance = value; }
    public AudioSource AudioSource { get => _audioSource; set => _audioSource = value; }
    public float CurrentVolume { get => _currentVolume; set => _currentVolume = value; }

    private void Awake() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        VolumeLoad();
    }

    public float GetVolume() {
        _currentVolume = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = _currentVolume;
        return _currentVolume;
    }

    public void SetVolume(float value) {
        PlayerPrefs.SetFloat("volume", value);
    }

    void VolumeLoad() {
        if (!PlayerPrefs.HasKey("volume")) {
            PlayerPrefs.SetFloat("volume", 0.5f);
            GetVolume();
        } else {
            GetVolume();
        }
    }

    public void PlayClip(AudioClip[] clips) {
        int index = Random.Range(0, clips.Length);
        AudioClip clip = clips[index];
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}