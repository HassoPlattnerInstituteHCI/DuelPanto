using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    public Transform playerSpawn;
    public Transform enemySpawn;

    private UpperHandle upperHandle;
    private LowerHandle lowerHandle;
    public float spawnSpeed = 1f;

    private SpeechIn speechIn;
    private SpeechOut speechOut;
    private string[] commands = new string[] { "yes", "no" };

    private bool gameEnded = false;

    public Text playerScoreText;
    public Text enemyScoreText;

    private int playerScore = 0;
    private int enemyScore = 0;

    void Start()
    {
        speechIn = new SpeechIn(onRecognized, commands);
        speechOut = new SpeechOut();

        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();

        UpdateUI();

        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to Quake Panto Edition");

        Level level = GetComponent<Level>();
        await level.playIntroduction();
        await speechOut.Speak("Introduction finished, game starts.");

        await ResetGame();
    }

    async Task ResetGame()
    {
        speechIn.StopListening();
        gameEnded = false;

        await speechOut.Speak("Spawning player");
        player.transform.position = playerSpawn.position;
        await upperHandle.SwitchTo(player, 1);

        await speechOut.Speak("Spawning enemy");
        enemy.transform.position = enemySpawn.position;
        await lowerHandle.SwitchTo(enemy, 1);

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
        Application.Quit();
    }

    async void onRecognized(string message)
    {
        Debug.Log($"[{GetType().Name}]: {message}");
        switch (message)
        {
            case "yes":
                await ResetGame();
                break;
            case "no":
                QuitGame();
                break;
            default:
                speechIn.StopListening();
                await TellCommands();
                speechIn.StartListening();
                break;
        }
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

        string defeatedPerson = playerDefeated ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");
        await speechOut.Speak("Continue?");

        gameEnded = true;

        speechIn.StartListening();
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
