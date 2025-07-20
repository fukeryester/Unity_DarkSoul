using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    public Collider weapon_colL;
    public Collider weapon_colR;
    public GameObject whL;
    public GameObject whR;

    private void Start()
    {
        whL = transform.DeepFind("WeaponHandleL").gameObject;
        whR = transform.DeepFind("WeaponHandleR").gameObject;
        weapon_colL = whL.GetComponentInChildren<Collider>();
        weapon_colR = whR.GetComponentInChildren<Collider>();
        WeaponDisable();
    }

    public void WeaponEnable()
    {
        if (am.ac.CheckStateTag("attackL"))
        {
            weapon_colL.enabled = true;
        }
        else
        {
            weapon_colR.enabled = true;
        }
    }

    public void WeaponDisable()
    {
        weapon_colL.enabled = false;
        weapon_colR.enabled = false;
    }
}
