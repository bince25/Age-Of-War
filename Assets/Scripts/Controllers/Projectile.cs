using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;  // Projectile speed
    public int damage = 10;   // Damage dealt on impact

    private Transform target;  // Target enemy

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            // Projectile reached the target
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        // Deal damage to the target
        UnitController enemy = target.GetComponent<UnitController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
