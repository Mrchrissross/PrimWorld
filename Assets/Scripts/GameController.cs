using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Header("Spawn Settings")]

        [Tooltip("This enables the tool below, allowing the choice of where the player spawns and locking them to that spawn point. This must be turned off while attempting a proper playthrough.")]
        public bool enabledAltSpawning = false;

        [Tooltip("Allows you to choose where you would like the player to spawn.")]
        public int spawnPoint = 1;

        [Tooltip("These are all the spawn locations that have been detected in the scene.")]
        private List<string> Locations = new List<string>();

        [Tooltip("Changes based on what the next spawn location is. (Used for debugging purposes)")]
        public string spawnLocation;

    [Header("GameObjects")]
        private GameObject player;

    void Start ()
    {
        player = transform.Find("Player").gameObject;

        for (int i = 0; i < this.transform.Find("SpawnPoints").childCount; i++)
            Locations.Add(this.transform.Find("SpawnPoints").GetChild(i).name);

        if (enabledAltSpawning)
            DataManager.SpawnLocation = Locations[spawnPoint - 1];

        player.transform.position = this.transform.Find("SpawnPoints").Find(DataManager.SpawnLocation).position;
        player.transform.rotation = this.transform.Find("SpawnPoints").Find(DataManager.SpawnLocation).rotation;
    }

	void Update ()
    {
        spawnLocation = DataManager.SpawnLocation;
    }
}
