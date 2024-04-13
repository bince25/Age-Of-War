using Mirror;
using UnityEditor;
using UnityEngine;

public class Turret : NetworkBehaviour
{

    [SerializeField]
    private string oppositeTag = "RightTurret";
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 1.0f;
    public float rotationSpeed = 5.0f;
    public float range = 5.0f;
    public bool isFacingRight = true;

    public Transform target;
    private float fireCooldown = 0.0f;
    private void Start()
    {
        this.gameObject.name = GUID.Generate().ToString();
        GameManager.Instance.turretsDictionary.Add(this.gameObject.name, this);

        if (isFacingRight)
        {
            oppositeTag = SpawnSide.Right.ToString();
        }
        else
        {
            oppositeTag = SpawnSide.Left.ToString();
        }
    }
    void Update()
    {
        if (!isServer) return;

        if (target == null)
        {
            FindClosestEnemy();
        }

        if (target != null)
        {
            // Rotate the turret towards the target
            Vector3 direction = target.position - transform.position;

            // Calculate the rotation angle in radians
            float rotationAngle = Mathf.Atan2(direction.y, direction.x);

            // Convert to degrees and apply to the z-axis
            float rotationZ = rotationAngle * Mathf.Rad2Deg;

            // Create a new rotation quaternion, constraining only the z-axis
            Quaternion newRotation = Quaternion.Euler(0, 0, rotationZ);

            if (!isFacingRight)
            {
                newRotation = Quaternion.Euler(0, 0, rotationZ - 180);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);

            if (fireCooldown <= 0)
            {
                Shoot();
                fireCooldown = 1 / fireRate;
            }

            fireCooldown -= Time.deltaTime;
        }
    }

    [Server]
    void Shoot()
    {
        GameManager.Instance.CurrentStrategy.HandleProjectileSpawn(this, projectilePrefab);
        // Set projectile properties (e.g., damage, speed, etc.)
    }

    [Server]
    public void ShootById(string unitId)
    {
        UnitController enemyUnit = GameManager.Instance.unitsDictionary[unitId];
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Projectile>().Seek(enemyUnit.transform.gameObject);
    }

    [Server]
    public GameObject InstantiateProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);
        NetworkServer.Spawn(projectile); // Spawn the projectile for all clients
        return projectile;
    }

    [Server]
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(oppositeTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
    }


}
