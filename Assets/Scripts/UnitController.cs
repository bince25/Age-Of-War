using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public LayerMask defaultLayer;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [SerializeField]
    private GameObject unitObject;

    [SerializeField]
    private AttackType defaultAttackType;
    [SerializeField]
    private string oppositeTag = "RightSide";
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
    private Dictionary<AttackType, string> attackTypeToAnimationName;

    private string defaultAttackTypeAnimationName;

    private BoxCollider2D _collider;

    private UnitState currentState = UnitState.Walking;

    private void Start()
    {
        unitSpumController = unitObject.GetComponent<SPUM_Prefabs>();
        Debug.Log(unitSpumController);
        Debug.Log(unitObject);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
        switch (currentState)
        {
            case UnitState.Walking:
                Move();
                break;
            case UnitState.Attacking:
                // Attacking logic is controlled by animation callbacks
                break;
            case UnitState.Idle:
                // check if there are any thing in front
                // if not walk
                // if yes, idle

                break;
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void Move()
    {
        if (isAnimating || isStopped) return;  // Add a check for the isStopped flag

        float moveDirection = isFacingRight ? 1 : -1;
        transform.position += new Vector3(moveDirection * speed, 0, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DetectAndAttackEnemy();
    }

    void DetectAndAttackEnemy()
    {
        if (isAnimating || isAttackOnCooldown) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == this.gameObject) continue;  // Ignore self

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
                currentState = UnitState.Attacking;
                RunAnimation(defaultAttackTypeAnimationName);
                return;  // Attack the first enemy found and exit
            }
        }

        // If no enemies found, set the unit to walking
        currentState = UnitState.Walking;
    }


    /*    void DetectAndAttackEnemy()
       {
           if (isAnimating) return;

           Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

           _collider = GetComponent<BoxCollider2D>();
           Debug.Log(_collider.size);
           Vector2 rayStartPoint = transform.position + new Vector3(_collider.size.x * 3, _collider.size.y / 2, 0);
           RaycastHit2D hit = Physics2D.Raycast(rayStartPoint, direction, detectionRange);

           Debug.DrawRay(rayStartPoint, direction * detectionRange, Color.red, 1.0f);

           if (hit.collider != null)
           {
               Debug.Log("Hit object: " + hit.collider.gameObject.tag);

               // If detected unit is an enemy, stop and attack
               if (hit.collider.gameObject.tag == oppositeTag)
               {
                   isAnimating = true;
                   RunAnimation(defaultAttackTypeAnimationName);
               }
               // If detected unit is another friendly unit (or obstacle), just stop
               else
               {
                   isAnimating = true;
                   unitSpumController.PlayAnimation("idle");
               }
           }
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
        yield return new WaitForSeconds(animationLength);
        isAnimating = false;
        isAttackOnCooldown = true;

        unitSpumController.PlayAnimation("idle");  // Transition to idle during cooldown

        yield return new WaitForSeconds(attackCooldown - animationLength);  // Wait for the remaining cooldown time

        isAttackOnCooldown = false;
        DetectAndAttackEnemy();  // After cooldown, check if we should attack again
    }

    private IEnumerator Idle(float animationLength)
    {
        yield return new WaitForSeconds(animationLength + 0.001f);
        unitSpumController.PlayAnimation("idle");
        isAnimating = false;

        // After the animation finishes, check if we should attack again
        DetectAndAttackEnemy();
    }

    private void Run()
    {
        unitSpumController.PlayAnimation("run");
    }

    private void Stun()
    {
        unitSpumController.PlayAnimation("debuff_stun");
    }

    private void Death()
    {
        unitSpumController.PlayAnimation("death");
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