using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour {
    static string leaderboard_key = "global_leaderboard"; // 22647
    [Header("Top 10")]
    [SerializeField] TextMeshProUGUI playerRank;
    [SerializeField] TextMeshProUGUI playerNames;
    [SerializeField] TextMeshProUGUI playerScores;

    [Header("Player")]
    [SerializeField] TextMeshProUGUI thisRank;
    [SerializeField] TextMeshProUGUI thisName;
    [SerializeField] TextMeshProUGUI thisScore;

    private void Start() {

    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload) {
        bool done = false;

        string playerId = PlayerPrefs.GetString("ll_id");
        LootLockerSDKManager.SubmitScore(playerId, scoreToUpload, leaderboard_key, (response) => {
            if (response.success) {
                Debug.Log("uploaded score");
                done = true;
            } else {
                Debug.LogWarning($"Failed Submit: {response.errorData.message}");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }
    public IEnumerator FetchTopHighScores() {
        bool isComplete = false;
        string playerId = PlayerPrefs.GetString("ll_id");

        LootLockerSDKManager.GetScoreList(leaderboard_key, 10, 0, (response) => {
            string tempRank = "Rank\n";
            string tempNames = "Player\n";
            string tempScores = "Score\n";
            if (response.success) {
                Debug.Log("Fetched score list");

                LootLockerLeaderboardMember[] members = response.items;

                foreach (var member in members) {
                    string displayName = member.player.name != "" ? member.player.name : member.player.id.ToString();
                    tempRank += $"{member.rank}. \n";
                    tempNames += $"{displayName}\n";
                    tempScores += $"{member.score / 100.0f}\n";
                }

            } else {
                Debug.LogWarning($"Unable to fetch score list: {response.errorData}");
            }

            isComplete = true;
            playerRank.text = tempRank;
            playerNames.text = tempNames;
            playerScores.text = tempScores;
        });

        yield return new WaitWhile(() => isComplete != true);
    }

    public IEnumerator FetchPlayerScore() {
        bool isComplete = false;
        string playerId = PlayerPrefs.GetString("ll_id");

        LootLockerSDKManager.GetMemberRank(leaderboard_key, playerId, (response) => {
            string tempRank = "Your Rank\n";
            string tempName = "Your Name\n";
            string tempScore = "Your Score\n";

            if (response.success) {
                Debug.Log($"Fetched score for player: {playerId}");


                string displayName = response.player.name != "" ? response.player.name : response.player.id.ToString();
                tempRank += $"{response.rank}. ";
                tempName += $"{displayName}";
                tempScore += $"{response.score / 100.0f}";

            } else {
                Debug.LogWarning($"Unable to fetch score for player: {response.errorData}");
            }

            isComplete = true;

            thisRank.text = tempRank;
            thisName.text = tempName;
            thisScore.text = tempScore;
        });

        yield return new WaitWhile(() => isComplete != true);
    }
}
