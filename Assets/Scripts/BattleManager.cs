using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    private CapsuleCollider defCol;
    public Collider hit_weapon;

    private void Start()
    {
        defCol = GetComponent<CapsuleCollider>();
        defCol.center = Vector3.up * 1f;
        defCol.height = 2f;
        defCol.radius = 0.6f;
        defCol.isTrigger = true;    

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            hit_weapon = other;
            am.TryApplyDamage();
        }
    }
}
