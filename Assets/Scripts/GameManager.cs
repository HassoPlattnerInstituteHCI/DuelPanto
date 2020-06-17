using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool introduceLevel = true;
    public GameObject player;
    public GameObject enemy;
    public Transform playerSpawn;
    public Transform enemySpawn;
    public Text playerScoreText;
    public Text enemyScoreText;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    bool gameEnded = false;
    int playerScore = 0;
    int enemyScore = 0;
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.Y }
    };

    void Start()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();

        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();

        if (!GetComponent<DualPantoSync>().debug)
            RegisterColliders();

        UpdateUI();

        Introduction();
    }

    // TODO: Check for forces at start
    async void Introduction()
    {
        await speechOut.Speak("Welcome to Quake Panto Edition");

        if (introduceLevel)
        {
            await IntroduceLevel();
        }

        await speechOut.Speak("Introduction finished, game starts.");

        await ResetGame();
    }

    async Task IntroduceLevel()
    {
        Level level = GetComponent<Level>();
        await level.PlayIntroduction();

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
                speechIn.StopListening();
                return;
            }
        }
    }

    void RegisterColliders() {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            collider.CreateObstacle();
            collider.Enable();
        }
    }

    async Task ResetGame()
    {
        speechIn.StopListening();
        gameEnded = false;

        await speechOut.Speak("Spawning player");
        player.transform.position = playerSpawn.position;
        await upperHandle.SwitchTo(player, 0.3f);

        await speechOut.Speak("Spawning enemy");
        enemy.transform.position = enemySpawn.position;
        await lowerHandle.SwitchTo(enemy, 0.3f);

        upperHandle.Free();

        player.SetActive(true);
        enemy.SetActive(true);
    }

    async void Update()
    {
        if (gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                await ResetGame();
            } else if (Input.GetKeyDown(KeyCode.N))
            {
                QuitGame();
            }
        }
    }

    void QuitGame()
    {
        Debug.Log("Quitting Application...");
        speechIn.StopListening();
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
        } else
        {
            playerScore++;
        }
        UpdateUI();

        // TODO: Increase enemy difficulty

        string defeatedPerson = playerDefeated ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");
        await speechOut.Speak("Continue?");

        gameEnded = true;

        string response = await speechIn.Listen(commands);
        if (response == "yes")
            await ResetGame();
        if (response == "no")
            QuitGame();
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
}
