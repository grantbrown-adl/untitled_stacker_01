using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] bool canHandleInput = true;
    [SerializeField] bool loading = false;
    [SerializeField] bool initialClick = true;
    [SerializeField] PlatformSpawner[] spawners;
    [SerializeField] int spawnerIndex;
    [SerializeField] PlatformSpawner currentSpawner;
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] GameObject leaderboardPanel;

    [Header("Score Shit")]
    [SerializeField] float currentScore;
    [SerializeField] float multiplier;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject loadingText;

    private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }
    public float Score { get => currentScore; set => UpdateScore(value); }
    public bool Loading { get => loading; set => SetLoading(value); }

    private void OnEnable() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else { _instance = this; }
    }

    void UpdateScore(float score) {
        multiplier++;
        currentScore += score * multiplier;
        scoreText.text = $"Score: {currentScore:F2}";
    }

    void SetLoading(bool loading) {
        this.loading = loading;

        loadingText.SetActive(this.loading);
    }

    private void Awake() {
        spawners = FindObjectsOfType<PlatformSpawner>();
        scoreText.text = "Click to Play";
        leaderboardPanel.SetActive(false);
    }
    private void Update() {
        HandleInput();
    }

    public IEnumerator HandleGameOver() {
        Time.timeScale = 0.0f;
        currentScore *= 100;
        canHandleInput = false;
        yield return StartCoroutine(leaderboard.SubmitScoreRoutine((int)currentScore));
        leaderboardPanel.SetActive(true);
    }

    public void StartNewGame() {
        canHandleInput = true;
        initialClick = true;
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    void HandleInput() {
        if (!canHandleInput) { return; }

        if (Input.GetButtonDown("Fire1")) {
            if (initialClick) { scoreText.text = $"Score: {currentScore:F2}"; }
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
