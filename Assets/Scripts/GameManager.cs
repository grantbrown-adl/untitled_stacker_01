using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] bool canHandleInput = true;
    [SerializeField] PlatformSpawner[] spawners;
    [SerializeField] int spawnerIndex;
    [SerializeField] PlatformSpawner currentSpawner;

    [Header("Score Shit")]
    [SerializeField] float currentScore;
    [SerializeField] float multiplier;
    [SerializeField] TextMeshProUGUI scoreText;

    private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }
    public float Score { get => currentScore; set => UpdateScore(value); }

    private void OnEnable() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else { _instance = this; }
    }

    void UpdateScore(float score) {
        multiplier++;
        currentScore += score * multiplier;
        scoreText.text = $"Score: {this.currentScore}";
    }

    private void Awake() {
        spawners = FindObjectsOfType<PlatformSpawner>();
        scoreText.text = "Click to Play";
    }
    private void Update() {
        HandleInput();
    }

    void HandleInput() {
        if (!canHandleInput) { return; }

        if (Input.GetButtonDown("Fire1")) {
            if (Platform.CurrentPlatform == null) return;

            Platform.CurrentPlatform.StopPlatform();
            currentSpawner = spawners[spawnerIndex];

            if (Platform.CurrentPlatform == null) return;
            if (Platform.CurrentPlatform.SpawnNew) {
                currentSpawner.SpawnPlatform();
                spawnerIndex = ++spawnerIndex % spawners.Length;
            }
        }
    }
}
