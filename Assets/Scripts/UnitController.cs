using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public LayerMask defaultLayer;
    public int maxHealth = 100;

    public int attackDamage = 10;
    public int currentHealth;
    public HealthBar healthBar;


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

    [SerializeField]
    public float speed = 4f;

    private SPUM_Prefabs unitSpumController;
    public bool isFacingRight = true;
    private bool isAnimating = false;
    private bool isStopped = false;
    private bool isRunning = false;
    private Dictionary<AttackType, string> attackTypeToAnimationName;

    private string defaultAttackTypeAnimationName;

    private BoxCollider2D _collider;

    private UnitState currentState = UnitState.Walking;

    private GameObject currentTarget;

    private void Start()
    {
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

        _collider = GetComponent<BoxCollider2D>();
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
        switch (currentState)
        {
            case UnitState.Walking:
                if (!isRunning)
                {
                    Run();
                }
                Move();
                break;
            case UnitState.Attacking:
                // Attacking logic is controlled by animation callbacks
                isRunning = false;
                if (currentTarget == null)
                {
                    currentState = UnitState.Walking;
                    isStopped = false;
                    isAnimating = false;
                    Run();
                    isRunning = false;
                }
                break;
            case UnitState.Idle:
                // check if there are any thing in front
                // if not walk
                // if yes, idle
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
                bool detected = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == oppositeTag || hitCollider.gameObject.tag == gameObject.tag && hitCollider.gameObject != this.gameObject)
                    {
                        detected = true;
                        break;
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
                break;
        }
    }

    void TakeDamage(int damage)
    {
        if (currentHealth <= 0 && this.gameObject.activeInHierarchy) // Check if the unit is still active
        {
            Death();
        }
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0 && this.gameObject.activeInHierarchy) // Check if the unit is still active
        {
            Death();
        }
    }


    void Attack(GameObject target)
    {
        if (target == null || !target.activeInHierarchy || currentHealth <= 0) return; // Check if target is available and if the unit's health is above zero

        UnitController targetController = target.GetComponent<UnitController>();
        Castle castle = target.GetComponent<Castle>();
        if (targetController != null)
        {
            targetController.TakeDamage(attackDamage);
        }
        else if (castle != null)
        {
            castle.TakeDamage(attackDamage);
        }
    }

    void ApplyDamage()
    {
        if (currentTarget == null || !currentTarget.activeInHierarchy) return;

        UnitController targetController = currentTarget.GetComponent<UnitController>();
        if (targetController != null)
        {
            targetController.TakeDamage(attackDamage);
        }
    }


    void Move()
    {
        if (currentState == UnitState.Attacking || isAnimating || isStopped) return;

        float moveDirection = isFacingRight ? 1 : -1;
        transform.position += new Vector3(moveDirection * speed, 0, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

            //return;
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange + 1.3f);
    }

    /*     void DetectAndAttackEnemy()
        {
            if (isAnimating || isAttackOnCooldown) return;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == this.gameObject) continue;

                if (hitCollider.gameObject.tag == gameObject.tag && hitCollider.gameObject != this.gameObject)
                {
                    isAnimating = true;
                    currentState = UnitState.Idle;
                    RunAnimation("idle");
                    return;
                }

                if (hitCollider.gameObject.tag == oppositeTag)
                {
                    isAnimating = true;
                    isStopped = true;  // Stop the unit when attacking
                    currentState = UnitState.Attacking;
                    RunAnimation(defaultAttackTypeAnimationName);
                    return;
                }
            }

            currentState = UnitState.Walking;
        } */

    void RunAnimation(string name)
    {
        Debug.Log(name);
        unitSpumController.PlayAnimation(name);

        float animationLength = GetAnimationLength(name);
        if (animationLength > 0)
        {
            StartCoroutine(AttackCooldown(animationLength));
        }
    }

    private IEnumerator AttackCooldown(float animationLength)
    {
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




    private IEnumerator Idle(float animationLength)
    {
        yield return new WaitForSeconds(animationLength + 0.001f);
        unitSpumController.PlayAnimation("idle");
        isAnimating = false;

        // After the animation finishes, check if we should attack again
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

    private void Death()
    {
        unitSpumController.PlayAnimation("death");
        ResourceController resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        resourceController.UnitDied(gameObject.tag, gameObject.name);
        Destroy(this.gameObject, 0.5f); // Destroy the unit after 1 second
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

public enum AttackType
{
    AttackNormal,
    AttackBow,
    AttackMagic,
    SkillBow,
    SkillMagic,
    SkillNormal,

}

public enum UnitState
{
    Walking,
    Attacking,
    Idle
}