using UnityEngine;
using System.Collections.Generic;

public class EnemyPathFinding : MonoBehaviour
{
    [Header("Speed"), Range(0, 3)]
        [Tooltip("This is the speed at which the enemy will go.")]
        public float speed = 1f;

    [Header("Stopping Distance"), Range(0.05f, 0.5f)]
        [Tooltip("How close the agent will get to its target before moving on to the next one.")]
        public float stoppingDistance = 0.15f;

    [Header("Targeting")]
        [Tooltip("This shows all targets with this agents path.")]
        private List<Transform> targets = new List<Transform>();

        [Tooltip("This is what the agent is currently targeting.")]
        private int target;

    void Start()
    {
        target = 0;
        for (int i = 0; i < this.transform.parent.Find("Path").childCount; i++)
            targets.Add(this.transform.parent.Find("Path").GetChild(i));
    }

    void Update()
    {
        if (target == targets.Count)
            target = 0;

        transform.position = Vector3.MoveTowards(transform.position, targets[target].position, speed * Time.deltaTime);

        float distance = Vector3.Distance(targets[target].position, transform.position);
        if (distance <= stoppingDistance)
            target++;
    }
}