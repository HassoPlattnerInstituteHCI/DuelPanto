using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using UnityEngine.Audio;

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

    void Start()
    {
        speechIn = new SpeechIn(onRecognized, commands);
        speechOut = new SpeechOut();

        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();

        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to Quake Panto Edition");

        Level level = GetComponent<Level>();
        await level.playIntroduction();
        await speechOut.Speak("Introduction finished, game starts.");

        await ResetPositions();

        await upperHandle.SwitchTo(enemy, 1);

        player.SetActive(true);
        enemy.SetActive(true);
    }

    async Task ResetPositions()
    {
        await speechOut.Speak("Spawning player");
        await upperHandle.MoveToPosition(playerSpawn.position, spawnSpeed);

        await speechOut.Speak("Spawning enemy");
        await lowerHandle.MoveToPosition(enemySpawn.position, spawnSpeed);
        enemy.transform.position = enemySpawn.position;
    }

    async void onRecognized(string message)
    {
        Debug.Log($"[{GetType().Name}]: {message}");
        switch (message)
        {
            case "yes":
                speechIn.StopListening();
                await ResetPositions();
                player.SetActive(true);
                enemy.SetActive(true);
                break;
            case "no":
                Application.Quit();
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

        string defeatedPerson = defeated.Equals(player) ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");
        await speechOut.Speak("Continue?");

        speechIn.StartListening();
    }

    async Task TellCommands()
    {
        await speechOut.Speak("You can say yes or no.");
    }
}
