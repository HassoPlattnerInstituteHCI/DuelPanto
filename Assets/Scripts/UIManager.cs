using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject uiGameObject;
    public GameObject screenBlack;
    public Text playerScoreText;
    public Text enemyScoreText;
    public GameObject highscoreEntryPrefab;
    public Transform contentParent;
    public GameObject leaderboard;
    public GameObject submitUI;

    dreamloLeaderBoard leaderBoard;
    bool nameEntered = false;
    string enteredName = "";
    bool gameEnded = false;

    async void Start()
    {
        uiGameObject.SetActive(false);
        screenBlack.SetActive(true);
        submitUI.SetActive(false);
        leaderboard.SetActive(false);

        leaderBoard = dreamloLeaderBoard.GetSceneDreamloLeaderboard();

        leaderBoard.GetScores();

        await ShowHighscores(10000);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            screenBlack.SetActive(false);
            uiGameObject.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
            screenBlack.SetActive(true);
            uiGameObject.SetActive(false);
        }

        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Tab))
            {
                leaderboard.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Tab))
            {
                leaderboard.SetActive(false);
            }
        }
    }

    public async Task GameOver(int gameScore, int totalTime, int trophyScore)
    {
        gameEnded = true;
        leaderboard.SetActive(true);
        submitUI.SetActive(true);

        while (!nameEntered)
        {
            await Task.Delay(500);
        }

        if (enteredName == null || enteredName == "")
        {
            Debug.LogError($"The entered name {name} is null or empty.");
        }

        leaderBoard.highScores = "";
        leaderBoard.AddScore(enteredName, gameScore, totalTime);

        await ShowHighscores(10000);

        // TODO: Estimate a good trophy score
        if (gameScore >= trophyScore)
        {
            // TODO: Return trophy/proof of beating trophy score
        }
    }

    async Task ShowHighscores(int timeout)
    {
        var task = GetHighscores();
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            List<dreamloLeaderBoard.Score> highscores = await task;
            foreach (Transform child in contentParent)
            {
                child.parent = null;
                Destroy(child.gameObject);
            }
            foreach (dreamloLeaderBoard.Score score in highscores)
            {
                GameObject highscoreEntry = Instantiate(highscoreEntryPrefab, contentParent);
                highscoreEntry.transform.GetChild(0).GetComponent<Text>().text = score.playerName;
                highscoreEntry.transform.GetChild(1).GetComponent<Text>().text = score.score.ToString();
            }
        }
        else
        {
            // timeout/cancellation logic
            Debug.Log($"[GameManager] Timeout of {timeout} ms retrieving highscores");
        }
    }

    async Task<List<dreamloLeaderBoard.Score>> GetHighscores()
    {
        while (leaderBoard.highScores == "")
        {
            await Task.Delay(500);
        }

        return leaderBoard.ToListHighToLow();
    }

    public void UpdateUI(int playerScore, int enemyScore)
    {
        playerScoreText.text = playerScore.ToString();
        enemyScoreText.text = enemyScore.ToString();
    }

    public void OnNameValueChanged(string value)
    {
        enteredName = value;
    }

    public void OnSubmitPressed()
    {
        if (name == "")
        {
            Debug.Log("[GameManager] Entered empty name.");
        }
        else
        {
            nameEntered = true;
        }
    }
}
