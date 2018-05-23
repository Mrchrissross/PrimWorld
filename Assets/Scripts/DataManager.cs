using UnityEngine;

public class DataManager : MonoBehaviour {

    private static string spawnPoint = "SpawnPoint (1)";
    private static Transform PlayerPosition = null;
    private static bool gameOver;
    private static int score = 0;

    private void Start()
    {
        gameOver = false;
    }

    public static string SpawnLocation
    {
        get
        {
            return spawnPoint;
        }
        set
        {
            spawnPoint = value;
        }
    }

    public static Transform PlayerSpawnPosition
    {
        get
        { 
            return PlayerPosition;
        }
        set
        { 
            PlayerPosition = value;
        }
    }

    public static bool GameOver
    {
        get
        {
            return gameOver;
        }
        set
        {
            gameOver = value;
        }
    }

    public static int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
        }
    }
}
