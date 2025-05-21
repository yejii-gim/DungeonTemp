using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate; // 쿨타임
    private bool attacking;
    public float attackDistance; // 공격 가능 거리
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResource;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;
    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    
    public override void OnAttackInput()
    {
        if(!attacking)
        {
            if (CharcterManager.Instance.player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate); // 쿨타임 기다리도록
            }
        }
    }

    // 쿨타임 해제
    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,attackDistance))
        {
            // 자원 채집
            if (doesGatherResource && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
            // 적 때리기
            if (doesDealDamage && hit.collider.TryGetComponent(out NPC npc))
            {
                Debug.Log(npc.health);
                npc.TakePhysicalDamage(damage);
            }
        }
    }
}
