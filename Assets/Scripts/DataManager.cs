using UnityEngine;

public class DataManager : MonoBehaviour {

    private static string spawnPoint = "SpawnPoint (1)";
    private static Transform PlayerPosition = null;

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
}
