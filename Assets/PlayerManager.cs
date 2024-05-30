using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] TMP_InputField nameInput;


    private void Start() {
        StartCoroutine(SetupRoutine());
    }

    public void SetPlayerName() {
        StartCoroutine(SetPlayerNameRoutine());
    }

    public void RefreshLeaderboard() {
        StartCoroutine(FetchScoreRoutine());
    }

    IEnumerator SetupRoutine() {
        GameManager.Instance.Loading = true;

        yield return LoginRoutine();
        yield return FetchScoreRoutine();

        GameManager.Instance.Loading = false;
    }

    IEnumerator FetchScoreRoutine() {
        GameManager.Instance.Loading = true;

        yield return leaderboard.FetchTopHighScores();
        yield return leaderboard.FetchPlayerScore();

        GameManager.Instance.Loading = false;
    }

    IEnumerator SetPlayerNameRoutine() {
        GameManager.Instance.Loading = true;

        bool complete = false;
        LootLockerSDKManager.SetPlayerName(nameInput.text, (response) => {
            if (response.success) {
                Debug.Log("Successfully set player name");
            } else {
                Debug.LogError($"Could not set player name: {response.errorData.message}");
            }
            complete = true;
        });

        yield return new WaitWhile(() => complete == false);
        yield return FetchScoreRoutine();

        GameManager.Instance.Loading = false;
    }

    IEnumerator LoginRoutine() {
        bool complete = false;

        LootLockerSDKManager.StartGuestSession((response) => {
            if (response.success) {
                Debug.Log("Player logged in");
                PlayerPrefs.SetString("ll_id", response.player_id.ToString());
            } else {
                Debug.LogWarning("Could not start session.");
            }
            complete = true;
        });

        yield return new WaitWhile(() => complete == false);
    }
}
