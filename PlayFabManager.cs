using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class PlayFabManager : MonoBehaviour
{

     public GameObject leaderboardEntryPrefab; // 1行分プレハブ
    public Transform contentTransform; // ScrollViewのContent部分

//５位までのタイムを記録する関数
    public void TrySubmitScoreIfTop5(int newScore)
{
    var request = new GetLeaderboardRequest
    {
        StatisticName = "HighScore",
        StartPosition = 0,
        MaxResultsCount = 5
    };

    PlayFabClientAPI.GetLeaderboard(request, result =>
    {
        bool shouldSubmit = false;

        if (result.Leaderboard.Count < 5)
        {
            // ランキングがまだ5人未満の場合、そのまま保存
            shouldSubmit = true;
        }
        else
        {
            // 5人以上いる場合、最下位（5位）より良いスコアかチェック
            int fifthScore = result.Leaderboard[4].StatValue;

            // タイムが小さいほど順位が高いと定義
            if (newScore < fifthScore)
            {
                shouldSubmit = true;
            }
        }

        if (shouldSubmit)
        {
            SubmitScoreAndGetLeaderboard(newScore);
        }
        else
        {
            Debug.Log("スコアが5位以内ではないため送信しません。");
            StartCoroutine(WaitAndGetLeaderboard());
        }

    }, error =>
    {
        Debug.LogError("ランキング取得エラー: " + error.GenerateErrorReport());
    });
}
//ゲームクリア時点のタイム受け取り用(GameManagerから)
void SubmitScoreAndGetLeaderboard(int score)
{
    var request = new UpdatePlayerStatisticsRequest
    {
        Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore",
                Value = score
            }
        }
    };

    PlayFabClientAPI.UpdatePlayerStatistics(request,
        result =>
        {
            Debug.Log("スコア送信成功！");
            StartCoroutine(WaitAndGetLeaderboard());
        },
        error => Debug.LogError("スコア送信失敗: " + error.GenerateErrorReport()));
}
    
   


//ランキング表示にディレイをかける
     IEnumerator WaitAndGetLeaderboard()
    {
        yield return new WaitForSecondsRealtime(3.0f); // ★ 3秒待つ（ここがポイント！）
        GetLeaderboard();
    }



//最終的なランキングを取得
public void GetLeaderboard()
{
    var request = new GetLeaderboardRequest
    {
        StatisticName = "HighScore",
        StartPosition = 0,
        MaxResultsCount = 10
    };
    PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnLeaderboardError);
}
//集計したタイムを早い順に並べ替える関数
void OnLeaderboardGet(GetLeaderboardResult result)
{
    var sortedLeaderboard = new List<PlayerLeaderboardEntry>(result.Leaderboard);
    sortedLeaderboard.Sort((a, b) => a.StatValue.CompareTo(b.StatValue)); // タイムが速い順に並び替え

    int maxDisplayCount = Mathf.Min(5, sortedLeaderboard.Count); // 最大5人まで

    for (int i = 0; i < maxDisplayCount; i++)
    {
        var entry = sortedLeaderboard[i];
        GameObject newEntry = Instantiate(leaderboardEntryPrefab, contentTransform);
        var texts = newEntry.GetComponentsInChildren<UnityEngine.UI.Text>();

        if (texts.Length >= 2)
        {
            texts[0].text = (i + 1).ToString() + "位";
            texts[1].text = $"{entry.DisplayName} - {(entry.StatValue / 100f).ToString("F2")}秒";
        }
    }
    
}
//ランキング取得失敗用
void OnLeaderboardError(PlayFabError error)
{
    Debug.LogError("ランキング取得失敗: " + error.GenerateErrorReport());
}

}

