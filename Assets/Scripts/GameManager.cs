using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/* TODO: Rather than calling it quake (which is a huge AAA title that we cannot 
 * approximate; also a trademark, I am suspecting), let's give this its own name: 
 * maybe a pun on dualPanto... dunno dualPanting (to pant = heavy breathing...)... 
 * not really... how about duelPanto (duel = a contest with deadly weapons arranged 
 * between two people in order to settle a point of honor)? Downside is that, when 
 * merely spoken, no one will even get the pun (I checked and they pronounce exactly the same). 
 * Maybe you have better ideas? Can anyone suggest a few names? 
 */

public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool introduceLevel = true;
    public GameObject player;
    public GameObject enemy;
    public EnemyConfig[] enemyConfigs;
    public Transform playerSpawn;
    public Transform enemySpawn;
    public Text playerScoreText;
    public Text enemyScoreText;
    public int level = 0;
    public int trophyScore = 10000;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    int playerScore = 0;
    int enemyScore = 0;
    int gameScore = 0;
    float levelStartTime = 0;
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();

        if (level < 0 || level >= enemyConfigs.Length)
        {
            Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");
            level = 0;
        }
    }

    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();

        UpdateUI();

        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to Quake Panto Edition");
        // TODO: 1. Introduce obstacles in level 2 (aka 1)
        await Task.Delay(1000);
        RegisterColliders();

        if (introduceLevel)
        {
            await IntroduceLevel();
        }

        await speechOut.Speak("Introduction finished, game starts.");

        await ResetGame();
    }

    async Task IntroduceLevel()
    {
        await speechOut.Speak("There are two obstacles.");
        Level level = GetComponent<Level>();
        await level.PlayIntroduction();

        // TODO: 2. Explain enemy and player with weapons by wiggling and playing shooting sound

        // TODO: 3. Don't ask for a tour, just do it
        await speechOut.Speak("Do you want to explore the room?");
        string response = await speechIn.Listen(commands);

        if (response == "yes")
        {
            await RoomExploration();
        }
    }

    async Task RoomExploration()
    {
        while (true)
        {
            await speechOut.Speak("Say done when you're ready.");
            string response = await speechIn.Listen(commands);
            if (response == "done")
            {
                return;
            }
        }
    }

    void RegisterColliders() {
        PantoCollider[] colliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            Debug.Log(collider);
            collider.CreateObstacle();
            collider.Enable();
        }
    }

    async Task ResetGame()
    {
        await speechOut.Speak("Spawning player");
        player.transform.position = playerSpawn.position;
        await upperHandle.SwitchTo(player, 0.3f);

        await speechOut.Speak("Spawning enemy");
        enemy.transform.position = enemySpawn.position;
        enemy.transform.rotation = enemySpawn.rotation;
        await lowerHandle.SwitchTo(enemy, 0.3f);
        if (level >= enemyConfigs.Length)
            Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");
        enemy.GetComponent<Enemy>().config = enemyConfigs[level];

        upperHandle.Free();

        player.SetActive(true);
        enemy.SetActive(true);
        levelStartTime = Time.time;
    }

    void QuitGame()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();
    }

    async void onRecognized(string message)
    {
        Debug.Log("SpeechIn recognized: " + message);
    }

    public void OnApplicationQuit()
    {
        speechOut.Stop(); //Windows: do not remove this line.
        speechIn.StopListening(); // [macOS] do not delete this line!
    }

    public async void OnDefeat(GameObject defeated)
    {
        player.SetActive(false);
        enemy.SetActive(false);

        bool playerDefeated = defeated.Equals(player);

        if (playerDefeated)
        {
            enemyScore++;
        }
        else
        {
            playerScore++;
        }
        UpdateUI();

        string defeatedPerson = playerDefeated ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");

        gameScore += CalculateGameScore(player, enemy);

        level++;
        if (level >= enemyConfigs.Length)
        {
            await GameOver();
        } else
        {
            // TODO: Evaluate the players performance with game score
            //await speechOut.Speak("Continue?");
            await speechOut.Speak($"Current score is {gameScore}");
            await speechOut.Speak($"Continuing with level {level + 1}");
            await ResetGame();

            //string response = await speechIn.Listen(commands);
            //if (response == "yes")
            //    await ResetGame();
            //if (response == "no")
            //    QuitGame();
        }
    }

    async Task TellCommands()
    {
        await speechOut.Speak("You can say yes or no.");
    }

    void UpdateUI()
    {
        playerScoreText.text = playerScore.ToString();
        enemyScoreText.text = enemyScore.ToString();
    }

    async Task GameOver()
    {
        await speechOut.Speak("Congratulations.");
        await speechOut.Speak($"You achieved a score of {gameScore}");
        // TODO: Estimate a good trophy score
        if (gameScore >= trophyScore)
        {
            // TODO: Return trophy/proof of beating trophy score
        }
    }

    int CalculateGameScore(GameObject player, GameObject enemy)
    {
        Health playerHealth = player.GetComponent<Health>();
        Health enemyHealth = enemy.GetComponent<Health>();

        float levelCompleteTime = Time.time - levelStartTime;
        int timeMultiplier = 1;
        if (levelCompleteTime < 30)
        {
            timeMultiplier = 5;
        } else if (levelCompleteTime < 45)
        {
            timeMultiplier = 3;
        } else if (levelCompleteTime < 60)
        {
            timeMultiplier = 2;
        }


        int levelScore = playerHealth.healthPoints - enemyHealth.healthPoints;
        if (levelScore > 0)
        {
            int levelMultiplier = (int)(Mathf.Pow(2, level) + 1);
            levelScore *= timeMultiplier * levelMultiplier;
        }

        return levelScore;
    }
}
