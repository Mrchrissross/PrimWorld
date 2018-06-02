using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 20f;

    public float bulletLife = 2f;

    void Start()
    {
        Destroy(gameObject, bulletLife);
    }

    void Update()
    {
        transform.Translate(Vector3.down * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            Destroy(gameObject);
    }
}
