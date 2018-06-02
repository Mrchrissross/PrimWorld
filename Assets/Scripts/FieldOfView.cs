using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{   
    [Header("Angles:")]
        [Tooltip("This is how wide the agents view angle is."), Range(0, 360)]
        public float viewAngle;

        [Tooltip("This is how far this agent can see."), Range(5, 30)]
        public float viewRadius;

    [Header("Masks:")]
        [Tooltip("These are the layer masks to so that agent can differentiate what is a player and what  is an obstacle.")]
        public LayerMask targetMask;
        public LayerMask obstacleMask;

    [Header("Attacking:")]
        [Tooltip("The number of targets needed to be found before the agent starts attacking."), Range(0, 1)]
        public int targetsNeededToAttack = 1;

        [Tooltip("Enabling this will make the agent always look at the player.")]
        public bool lookAtPlayer = true;

        [Tooltip("This is the speed at which the agent will attack."), Range(1.2f, 3)]
        public float attackSpeed = 2.5f;
        private float tempAttackSpeed;
        private bool shoot;
        private float instantiateBulletTime = 0.66666666666666666666666666666667f;
        private float tempBulletTime;

    [Header("Bullet Settings:")]
        [Tooltip("This is the bullet that will be firing from the agent.")]
        public BulletController bullet;

        [Tooltip("This determines how long the bullet will stay within the game before being destroyed.")]
        public float bulletLifeOverride = 3.0f;

        [Tooltip("This determines how fast the bullet will travel.")]
        public float bulletSpeedOverride = 5.0f;

        [Tooltip("This is where the bullet will be instantiated.")]
        private Transform firePoint;

    [Header("Currently Visible:")]
        [Tooltip("This shows all targets that this agent can see."), HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();

    private float       meshResolution = 1.0f,
                        edgeDstThreshold = 0.5f;

    private int         edgeResolveIterations = 4;

    private MeshFilter   viewMeshFilter;
    Mesh viewMesh;

    void Start()
    {
        viewMeshFilter = transform.Find("ViewVisualisation").GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", .2f);

        tempAttackSpeed = attackSpeed;
        tempBulletTime = instantiateBulletTime;
        firePoint = transform.Find("Animator").Find("BackHand").GetChild(0);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void Update()
    {
        if ((visibleTargets.Count == 1) && (targetsNeededToAttack != 0))
        {
            tempAttackSpeed -= Time.deltaTime;

            if (tempAttackSpeed < 0)
            {
                transform.Find("Animator").GetComponent<Animator>().SetBool("Attacking", true);
                shoot = true;
                tempAttackSpeed = attackSpeed;
            }
            else
                transform.Find("Animator").GetComponent<Animator>().SetBool("Attacking", false);

            if (shoot)
            {
                tempBulletTime -= Time.deltaTime;
                if (tempBulletTime < 0)
                {
                    BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;
                    newBullet.bulletSpeed = bulletSpeedOverride;
                    newBullet.bulletLife = bulletLifeOverride;
                    tempBulletTime = instantiateBulletTime;
                    shoot = false;
                }
            }

            transform.Find("Animator").GetComponent<Animator>().SetBool("Walking", false);
            transform.GetComponent<EnemyPathFinding>().stopped = true;

            if(visibleTargets.Count == 1)
            {
                if(visibleTargets[0].position.x < transform.position.x)
                    transform.Find("Animator").localScale = new Vector3(-1, 1, 1);
                else
                    transform.Find("Animator").localScale = new Vector3(1, 1, 1);
            }
        }
        else if (targetsNeededToAttack == 0)
        {
            tempAttackSpeed -= Time.deltaTime;

            if (tempAttackSpeed < 0)
            {
                transform.Find("Animator").GetComponent<Animator>().SetBool("Attacking", true);
                shoot = true;
                tempAttackSpeed = attackSpeed;
            }
            else
                transform.Find("Animator").GetComponent<Animator>().SetBool("Attacking", false);

            if (shoot)
            {
                tempBulletTime -= Time.deltaTime;
                if (tempBulletTime < 0)
                {
                    BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;
                    newBullet.bulletSpeed = bulletSpeedOverride;
                    newBullet.bulletLife = bulletLifeOverride;
                    tempBulletTime = instantiateBulletTime;
                    shoot = false;
                }
            }

            transform.Find("Animator").GetComponent<Animator>().SetBool("Walking", false);
            transform.GetComponent<EnemyPathFinding>().stopped = true;
        }
        else
        {
            transform.Find("Animator").GetComponent<Animator>().SetBool("Walking", true);
            transform.Find("Animator").GetComponent<Animator>().SetBool("Attacking", false);
            transform.GetComponent<EnemyPathFinding>().stopped = false;
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))                
                    visibleTargets.Add(target);                                                     // <<< Target finding.         
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}