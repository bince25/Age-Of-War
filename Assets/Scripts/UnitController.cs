using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{


    [SerializeField]
    private GameObject unitObject;

    [SerializeField]
    private AttackType defaultAttackType;

    private Dictionary<AttackType, string> attackTypeToAnimationName = new Dictionary<AttackType, string>()
    {
        { AttackType.AttackNormal, "attack_normal" },
        { AttackType.AttackBow, "attack_bow" },
        { AttackType.AttackMagic, "attack_magic" },
        { AttackType.SkillBow, "skill_bow" },
        { AttackType.SkillMagic, "skill_magic" },
        { AttackType.SkillNormal, "skill_normal" },
    };

    private string defaultAttackTypeAnimationName;

    private SPUM_Prefabs unitSpumController;
    private bool isAnimating = false;
    // Start is called before the first frame update
    void Start()
    {
        unitSpumController = unitObject.GetComponent<SPUM_Prefabs>();
        unitSpumController.PlayAnimation("idle");
        defaultAttackTypeAnimationName = attackTypeToAnimationName[defaultAttackType];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            RunAnimation(defaultAttackTypeAnimationName);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && !isAnimating)
        {
            Death();
        }
    }

    void RunAnimation(string name)
    {
        Debug.Log(name);
        unitSpumController.PlayAnimation(name);

        float animationLength = GetAnimationLength(name);
        if (animationLength > 0)
        {
            isAnimating = true;
            StartCoroutine(Idle(animationLength));
        }
    }

    private IEnumerator Idle(float animationLength)
    {
        yield return new WaitForSeconds(animationLength + 0.001f);
        unitSpumController.PlayAnimation("idle");
        isAnimating = false;
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
