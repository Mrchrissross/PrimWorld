using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [Header("Score")]
        [Tooltip("This is the amount of leaked magic that the player has collected.")]
        public int leakedMagic;

        [Tooltip("This is all the magic that the player has collected over the course of the level.")]
        public List<string> collectedMagic = new List<string>();

    [Header("Spawn Settings")]
        [Tooltip("This enables the tool below, allowing the choice of where the player spawns and locking them to that spawn point. This must be turned off while attempting a proper playthrough.")]
        public bool enabledAltSpawning = false;

        [Tooltip("Allows you to choose where you would like the player to spawn.")]
        public int spawnPoint = 1;

        [Tooltip("Changes based on what the next spawn location is. (Used for debugging purposes)")]
        public string spawnLocation;

        [Tooltip("These are all the spawn locations that have been detected in the scene.")]
        private List<string> Locations = new List<string>();

    [Header("GameObjects")]
        private GameObject player;

    void Start ()
    {   
        #region Spawning
        player = transform.Find("Player").gameObject;

        for (int i = 0; i < this.transform.Find("SpawnPoints").childCount; i++)
            Locations.Add(this.transform.Find("SpawnPoints").GetChild(i).name);

        for (int i = 0; i < transform.Find("MysticalMagic").childCount; i++)
            if (DataManager.CollectedMagic.Contains(transform.Find("MysticalMagic").GetChild(i).name))
                transform.Find("MysticalMagic").GetChild(i).gameObject.SetActive(false);

        if (enabledAltSpawning)
            DataManager.SpawnLocation = Locations[spawnPoint - 1];

        player.transform.position = this.transform.Find("SpawnPoints").Find(DataManager.SpawnLocation).position;
        player.transform.rotation = this.transform.Find("SpawnPoints").Find(DataManager.SpawnLocation).rotation;
        #endregion
    }

    void Update()
    {
        if (spawnLocation != DataManager.SpawnLocation)
            spawnLocation = DataManager.SpawnLocation;

        if (DataManager.GameOver == true)
            GameOver();

        if (leakedMagic != DataManager.Score)
        {
            leakedMagic = DataManager.Score;

            for (int i = 0; i < transform.Find("MysticalMagic").childCount; i++)
                if (DataManager.CollectedMagic.Contains(transform.Find("MysticalMagic").GetChild(i).name))
                    transform.Find("MysticalMagic").GetChild(i).gameObject.SetActive(false);

            collectedMagic = DataManager.CollectedMagic;
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene(0);
    }
}
