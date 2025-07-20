using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;

    void Awake()
    {
        ac = GetComponent<ActorController>();   
        GameObject sensor = transform.Find("sensor").gameObject;
        GameObject model = ac.model;

        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
    }
    private T Bind<T> (GameObject go) where T: IActorManagerInterface
    {
        T temp = go.GetComponent<T>();
        if (temp == null) temp = go.AddComponent<T>();
        temp.am = this;
        return temp;
    }

    public void TryApplyDamage()
    {
        if (sm.isImmortal)
        {
            //Ignore All Attack
        }
        else if (sm.isDefence)
        {
            //Block Attack
            Blocked();
        }
        else
        {
            //Apply Damage
            if (sm.HP > 0)
            {
                sm.ChangeHP(-30);
                if (sm.HP > 0) Hit();
                else Die();
            }
        }
    }

    public void Blocked()
    {
        ac.IssueTrigger("blocked");
    }

    public void Hit()
    {
        ac.IssueTrigger("hit", bm.hit_weapon);
    }

    public void Die()
    {
        ac.IssueTrigger("die", bm.hit_weapon);
        ac.PI.inputEnabled = false;
        if(ac.cameraController.lockState == true) ac.cameraController.ToggleLock();
        ac.cameraController.enabled = false;
    }
}
