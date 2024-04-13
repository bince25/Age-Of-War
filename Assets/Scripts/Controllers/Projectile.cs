using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    public float speed = 10f;  // Projectile speed
    public int damage = 10;   // Damage dealt on impact

    [SyncVar]
    private GameObject targetGameObject;  // Target enemy as GameObject to be synchronized over the network
    private Transform target;  // Target enemy

    public void Seek(GameObject _targetGameObject)
    {
        targetGameObject = _targetGameObject;
        target = _targetGameObject.transform;
    }

    void Update()
    {
        if (target == null)
        {
            NetworkServer.Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            // Projectile reached the target
            CmdHitTarget(targetGameObject);
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    [Command]
    void CmdHitTarget(GameObject targetGameObject)
    {
        // Deal damage to the target on the server
        UnitController enemy = targetGameObject.GetComponent<UnitController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);  // Assuming TakeDamage is a server-side method
        }

        NetworkServer.Destroy(gameObject);
    }
}
