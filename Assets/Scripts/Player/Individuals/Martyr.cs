using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martyr : Hero
{
    protected override void Start()
    {
        base.Start();   //need
    }
    protected override void Update()
    {
        GetAllHero();
    }

    protected override void OnDmgTaken(float dmgTaken)
    {
        //deal negative dmg == positive heal
        List<GameObject> targets = GetCurrentTargets();
        float dmgHeal = dmgTaken / targets.Count;

        foreach (GameObject target in targets)
        {
            target.GetComponent<Hero>().TakeDamage(-dmgHeal);
        }

        base.Attack();   //need
    }
}
