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
    [SerializeField] Camera mainCamera;
    [SerializeField] int spawnedPlatforms;

    [SerializeField] bool cameraTransitioning = false;

    [Header("Score Shit")]
    [SerializeField] float currentScore;
    [SerializeField] int currentHeight;
    [SerializeField] float multiplier;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] GameObject loadingText;
    [SerializeField] MusicPlayer audioPlayer;
    [SerializeField] AudioClip audioClip;
    [SerializeField] CameraController cameraController;
    [SerializeField] float offsetReductionAmount = 0.5f;

    private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }
    public float Score { get => currentScore; set => UpdateScore(value); }
    public bool Loading { get => loading; set => SetLoading(value); }
    public Camera MainCamera { get => mainCamera; set => mainCamera = value; }
    public bool CameraTransitioning { get => cameraTransitioning; set => cameraTransitioning = value; }
    public int CurrentHeight { get => currentHeight; set => UpdateHeight(value); }

    private void OnEnable() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else { _instance = this; }
    }

    void UpdateScore(float score) {
        print($"Score increase: {score}");
        multiplier++;
        currentScore += score * multiplier;
        scoreText.text = $"Score: {currentScore:F2}";
    }

    void UpdateHeight(int value) {
        currentHeight++;
        heightText.text = $"Height: {currentHeight}";
    }

    void SetLoading(bool loading) {
        this.loading = loading;

        loadingText.SetActive(this.loading);
    }

    private void Awake() {
        spawners = FindObjectsOfType<PlatformSpawner>();
        scoreText.text = "Click to Play";
        heightText.text = "good luck";
        leaderboardPanel.SetActive(false);
        mainCamera = Camera.main;
    }
    private void Update() {
        HandleInput();
    }

    public IEnumerator HandleGameOver() {
        Time.timeScale = 0.0f;
        currentScore *= 100;
        canHandleInput = false;
        yield return StartCoroutine(leaderboard.SubmitScoreRoutine((int)currentScore));
        yield return StartCoroutine(leaderboard.FetchTopHighScores());
        yield return StartCoroutine(leaderboard.FetchPlayerScore());
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
            if (loading) return;
            if (Platform.CurrentPlatform == null) return;
            if (cameraTransitioning) return;

            if (!initialClick) audioPlayer.PlayClip(audioClip);
            if (initialClick) {
                scoreText.text = $"Score: {currentScore:F2}";
                initialClick = false;
            }

            spawnedPlatforms++;
            //if (spawnedPlatforms % 3 == 0) mainCamera.fieldOfView++;
            if (!initialClick) {
                cameraController.Offset -= Vector3.right * offsetReductionAmount;
                cameraController.LookAtOrigin();
            }



            Platform.CurrentPlatform.StopPlatform();
            currentSpawner = spawners[spawnerIndex];

            if (Platform.CurrentPlatform == null) return;
            if (Platform.CurrentPlatform.SpawnNew) {
                currentSpawner.SpawnPlatform();
                //spawnerIndex = ++spawnerIndex % spawners.Length;
                spawnerIndex = RandomIndex();
            }
        }
    }

    int RandomIndex() {
        int length = spawners.Length;
        int randomIndex = Random.Range(0, length - 1);
        if (randomIndex != spawnerIndex) return randomIndex;

        float random = Random.Range(0.0f, 1.0f);
        return WrapIndex(random > 0.5f ? randomIndex + 1 : randomIndex - 1, length);
    }

    private int WrapIndex(int value, int modulo) {
        return (value % modulo + modulo) % modulo;
    }
}
