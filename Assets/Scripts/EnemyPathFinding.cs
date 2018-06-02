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

        [Tooltip("This is the amount of time that the agent will go idel for at each stop."), Range(0, 4)]
        public float idleTime = 2.0f;
        private float tempIdleTime;

    [Header("Targeting")]
        [Tooltip("This shows all targets with this agents path.")]
        private List<Transform> targets = new List<Transform>();

        [Tooltip("This is what the agent is currently targeting.")]
        private int target;

    public bool stopped;
     
    void Start()
    {
        target = 0;
        for (int i = 0; i < this.transform.parent.Find("Path").childCount; i++)
            targets.Add(this.transform.parent.Find("Path").GetChild(i));
        tempIdleTime = idleTime;
    }

    void Update()
    {
        if (target == targets.Count)
            target = 0;

        if (!stopped)
        {
            Move();

            Target();
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[target].position, speed * Time.deltaTime);

        if (transform.Find("Animator"))
        {
            if (targets[target].position.x < transform.position.x)
                transform.Find("Animator").localScale = new Vector3(-1, 1, 1);
            else
                transform.Find("Animator").localScale = new Vector3(1, 1, 1);
        }
    }

    void Target()
    {
        float distance = Vector3.Distance(targets[target].position, transform.position);
        if (distance <= stoppingDistance)
        {
            if(transform.Find("Animator"))
                if(transform.Find("Animator").GetComponent<Animator>() != null)
                    transform.Find("Animator").GetComponent<Animator>().SetBool("Walking", false);

            tempIdleTime -= Time.deltaTime;
            if (tempIdleTime < 0)
            {
                target++;
                if(transform.Find("Animator"))
                    if (transform.Find("Animator").GetComponent<Animator>() != null)
                        transform.Find("Animator").GetComponent<Animator>().SetBool("Walking", true);
                tempIdleTime = idleTime;
            }
        }
    }
}