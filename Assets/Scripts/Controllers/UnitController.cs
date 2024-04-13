using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitController : NetworkBehaviour
{
    public SpawnSide spawnSide;
    public bool isDead = false;
    public LayerMask defaultLayer;

    [SyncVar]
    public int maxHealth = 100;
    [SyncVar]
    public int attackDamage = 10;
    [SyncVar]
    public int currentHealth;

    public HealthBar healthBar;

    [SerializeField]
    private UnitType unitType;

    [SerializeField]
    private GameObject unitObject;

    [SerializeField]
    private AttackType defaultAttackType;
    [SerializeField]
    private string oppositeTag = "RightSide";

    [SerializeField]
    private string oppositeCastleTag = "RightCastle";

    [SerializeField]
    private float detectionRange = 2f;

    [SerializeField]
    private float attackCooldown = 1f;  // Default recharge time (1 second). Adjust per unit if needed.
    private bool isAttackOnCooldown = false;

    [SyncVar]
    [SerializeField]
    public float speed = 4f;

    private SPUM_Prefabs unitSpumController;
    public bool isFacingRight = true;
    private bool isAnimating = false;
    private bool isStopped = false;
    private bool isRunning = false;
    private Dictionary<AttackType, string> attackTypeToAnimationName;

    private bool delayMovement = false;

    private string defaultAttackTypeAnimationName;

    private BoxCollider2D _collider;

    private UnitState currentState = UnitState.Walking;

    private GameObject currentTarget;
    private ResourceController resourceController;

    private void Start()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        if (isFacingRight)
        {
            oppositeTag = SpawnSide.Right.ToString();
            oppositeCastleTag = SpawnSide.RightCastle.ToString();
        }
        else
        {
            oppositeTag = SpawnSide.Left.ToString();
            oppositeCastleTag = SpawnSide.LeftCastle.ToString();
        }

        unitSpumController = unitObject.GetComponent<SPUM_Prefabs>();
        attackTypeToAnimationName = new Dictionary<AttackType, string>()
        {
            { AttackType.AttackNormal, "attack_normal" },
            { AttackType.AttackBow, "attack_bow" },
            { AttackType.AttackMagic, "attack_magic" },
            { AttackType.SkillBow, "skill_bow" },
            { AttackType.SkillMagic, "skill_magic" },
            { AttackType.SkillNormal, "skill_normal" },
        };

        defaultAttackTypeAnimationName = attackTypeToAnimationName[defaultAttackType];

        _collider = gameObject.GetComponent<BoxCollider2D>();
        if (!_collider.isTrigger)
        {
            _collider.isTrigger = true;
        }

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        Run();
    }

    private void Update()
    {
        if (!isServer) return;

        if (currentHealth <= 0 && !isDead)
        {
            Death();
            return; // Exit the Update method after calling Death
        }
        if (!isDead)
        {
            switch (currentState)
            {
                case UnitState.Walking:
                    if (!isRunning)
                    {
                        Run();
                    }
                    if (delayMovement)
                    {
                        StartCoroutine(NormalizeSpeed(speed, 1f));
                        speed = speed * .9f;
                        delayMovement = false;
                    }
                    Move();
                    break;
                case UnitState.Attacking:
                    // Attacking logic is controlled by animation callbacks
                    isRunning = false;
                    if (currentTarget == null) // Check if target is available and if the unit's health is above zero
                    {
                        if (CheckInRange()) return;
                        currentState = UnitState.Walking;
                        isStopped = false;
                        isAnimating = false;

                        if (CheckFront()) delayMovement = true;

                        Run();
                        isRunning = false;
                        break;
                    }

                    if (!CheckFront())
                    {
                        currentState = UnitState.Walking;
                        isStopped = false;
                        isAnimating = false;
                        Run();
                        isRunning = false;
                        break;
                    }
                    break;
                case UnitState.Idle:
                    // check if there are any thing in front
                    // if not walk
                    // if yes, idle
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
                    bool detected = false;
                    bool enemyDetected = false;
                    foreach (var hitCollider in hitColliders)
                    {
                        if ((hitCollider.gameObject.tag == oppositeTag)
                        || (hitCollider.gameObject.tag == gameObject.tag
                        && hitCollider.gameObject != this.gameObject
                        ))
                        {
                            detected = true;

                            if (hitCollider.gameObject.tag == oppositeTag)
                            {
                                enemyDetected = true;
                                currentTarget = hitCollider.gameObject;
                            }

                            if (!enemyDetected)
                            {
                                if (isFacingRight && hitCollider.gameObject.transform.position.x < transform.position.x ||
                                    !isFacingRight && hitCollider.gameObject.transform.position.x > transform.position.x)
                                {
                                    detected = false;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                    if (!detected)
                    {
                        currentState = UnitState.Walking;
                        isStopped = false;
                        isAnimating = false;
                        Run();
                        isRunning = false;
                    }
                    if (detected && enemyDetected)
                    {
                        currentState = UnitState.Attacking;
                        isStopped = true;
                        isAnimating = true;
                        RunAnimation(defaultAttackTypeAnimationName);
                    }
                    if (isAnimating)
                    {
                        isAnimating = false;
                    }
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        CmdTakeDamage(damage);
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        if (!isDead)
        {
            if (currentHealth <= 0 && gameObject.activeInHierarchy)
            {
                currentHealth = 0;
            }
            currentHealth -= damage;
            RpcUpdateHealth(currentHealth);
        }
    }

    [ClientRpc]
    void RpcUpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        healthBar.SetHealth(newHealth);
    }

    public void AttackUnitByID(string targetID, int damageAmount)
    {
        CmdAttackUnitByID(targetID, damageAmount);
    }

    [Command]
    void CmdAttackUnitByID(string targetID, int damageAmount)
    {
        if (GameManager.Instance.unitsDictionary.ContainsKey(targetID))
        {
            GameManager.Instance.unitsDictionary[targetID].CmdTakeDamage(damageAmount);
        }
        else
        {
            Debug.LogWarning("No unit found with ID: " + targetID);
        }
    }



    void Attack(GameObject target)
    {
        CmdAttack(target);
    }

    [Command]
    void CmdAttack(GameObject target)
    {
        GameManager.Instance.CurrentStrategy.HandleUnitAttack(this, target);
    }

    void Move()
    {
        if (!isServer) return;

        if (currentState == UnitState.Attacking || isAnimating || isStopped) return;

        float moveDirection = isFacingRight ? 1 : -1;
        transform.position += new Vector3(moveDirection * speed, 0, 0) * Time.deltaTime;
    }

    IEnumerator NormalizeSpeed(float initialSpeed, float time)
    {
        yield return new WaitForSeconds(time);
        speed = initialSpeed;
    }

    bool CheckFront()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        bool detected = false;
        foreach (var hitCollider in hitColliders)
        {
            if ((hitCollider.gameObject.tag == oppositeTag || (hitCollider.gameObject.tag == oppositeCastleTag)) || (hitCollider.gameObject.tag == gameObject.tag && hitCollider.gameObject != this.gameObject))
            {
                if (isFacingRight && hitCollider.gameObject.transform.position.x > transform.position.x ||
                    !isFacingRight && hitCollider.gameObject.transform.position.x < transform.position.x)
                {
                    detected = true;
                    break;
                }
            }
        }

        return detected;
    }

    bool CheckInRange()
    {
        Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange + 1.3f);
        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.gameObject.tag == oppositeTag || hitCollider.gameObject.tag == oppositeCastleTag)
            {
                // Handle enemy behavior
                isAnimating = true;
                isStopped = true;  // Stop the unit when attacking
                currentState = UnitState.Attacking;
                currentTarget = hitCollider.gameObject;
                RunAnimation(defaultAttackTypeAnimationName);
                return true; // Exit after handling the first enemy detected
            }
        }

        return false;
    }

    bool CheckForEnemyUnit()
    {
        bool foundEnemyUnit = false; ;
        Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange + 1.3f);
        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.gameObject.tag == oppositeTag)
            {
                foundEnemyUnit = true;
                // Handle enemy behavior
                isAnimating = true;
                isStopped = true;  // Stop the unit when attacking
                currentState = UnitState.Attacking;
                currentTarget = hitCollider.gameObject;
                RunAnimation(defaultAttackTypeAnimationName);
                break; // Exit after handling the first enemy detected
            }
        }

        return foundEnemyUnit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (isServer)
        {
            if (isFacingRight && collision.gameObject.transform.position.x < transform.position.x ||
                        !isFacingRight && collision.gameObject.transform.position.x > transform.position.x)
            {
                return;
            }

            if (collision.gameObject == null || !collision.gameObject.activeInHierarchy) return; // Check if the colliding object is available

            // If the colliding object is an ally and not the current unit itself
            if (collision.gameObject.tag == gameObject.tag && collision.gameObject != this.gameObject)
            {
                // Stop the unit to prevent overlapping with the ally in front
                isAnimating = true;
                isStopped = true;
                currentState = UnitState.Idle;
                unitSpumController.PlayAnimation("idle");

                Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange + 1.3f);
                foreach (var hitCollider in _hitColliders)
                {
                    if (hitCollider.gameObject.tag == oppositeTag || hitCollider.gameObject.tag == oppositeCastleTag)
                    {
                        // Handle enemy behavior
                        isAnimating = true;
                        isStopped = true;  // Stop the unit when attacking
                        currentState = UnitState.Attacking;
                        currentTarget = hitCollider.gameObject;
                        RunAnimation(defaultAttackTypeAnimationName);
                        return; // Exit after handling the first enemy detected
                    }
                }

                return;
            }

            // If the colliding object is an enemy
            if (collision.gameObject.tag == oppositeTag)
            {
                isAnimating = true;
                isStopped = true;  // Stop the unit when attacking
                currentState = UnitState.Attacking;
                currentTarget = collision.gameObject;
                RunAnimation(defaultAttackTypeAnimationName);
                return;
            }

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange + 1.3f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.tag == oppositeTag || hitCollider.gameObject.tag == oppositeCastleTag)
                {
                    // Handle enemy behavior
                    isAnimating = true;
                    isStopped = true;  // Stop the unit when attacking
                    currentState = UnitState.Attacking;
                    currentTarget = hitCollider.gameObject;
                    RunAnimation(defaultAttackTypeAnimationName);
                    return; // Exit after handling the first enemy detected
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange + 1.3f);
    }

    void RunAnimation(string name)
    {
        Debug.Log(name);
        unitSpumController.PlayAnimation(name);

        if (isServer)
        {
            float animationLength = GetAnimationLength(name);
            if (animationLength > 0)
            {
                StartCoroutine(AttackCooldown(animationLength));
            }
        }
    }

    private IEnumerator AttackCooldown(float animationLength)
    {
        if (!isServer) yield break;

        yield return new WaitForSeconds(animationLength); // Wait for the animation to complete
        if (currentTarget != null && currentTarget.activeInHierarchy && currentHealth > 0) // Ensure target is still available and the unit's health is above zero
        {
            Attack(currentTarget); // Attack the target
        }

        isAnimating = false;
        isAttackOnCooldown = true;

        // Introduce a random delay before the next attack
        yield return new WaitForSeconds(attackCooldown - animationLength);

        isAttackOnCooldown = false;

        // If the unit is still in the attacking state, play the attack animation again
        if (currentState == UnitState.Attacking)
        {
            isAnimating = true;
            RunAnimation(defaultAttackTypeAnimationName);
        }
        else
        {
            currentState = UnitState.Walking;
            isStopped = false;
            isAnimating = false;
            isRunning = false;
        }
    }

    private void Run()
    {
        if (!isRunning)
        {
            unitSpumController.PlayAnimation("run");
            isRunning = true;
        }
    }

    private void Stun()
    {
        unitSpumController.PlayAnimation("debuff_stun");
    }

    [Server]
    public void Death()
    {
        if (isDead) return;

        isDead = true;
        RpcHandleDeath();
    }

    [ClientRpc]
    public void RpcHandleDeath()
    {
        Debug.Log("Unit " + gameObject.name + " died!");
        if (isDead) return; // Ensure Death is only called once

        isDead = true;
        unitSpumController.PlayAnimation("death");
        // Disable the collider
        if (_collider) _collider.enabled = false;

        resourceController.UnitDied(gameObject.tag, this.unitType);
        // Deactivate the game object immediately and then destroy it after the death animation
        gameObject.SetActive(false);
        Destroy(gameObject, GetAnimationLength("death"));// Destroy the unit after 1 second
    }

    public float GetAnimationLength(string name)
    {
        foreach (var clip in unitSpumController.AnimationClips)
        {
            if (clip.name.ToLower().Contains(name.ToLower()))
            {
                return clip.length;
            }
        }
        return 0f; // Return 0 if no animation with that name is found.
    }

}
