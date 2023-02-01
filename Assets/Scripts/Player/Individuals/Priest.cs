using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Hero
{
    protected override void Start()
    {
        base.Start();   //need
    }
    protected override void Update()
    {
        base.Update();   //need
        GetAllHero();
    }
    protected override void Attack()
    {
        //deal negative dmg == positive heal
        List<GameObject> targets = GetCurrentTargets();
        foreach (GameObject target in targets)
        {
            target.GetComponent<Hero>().TakeDamage(-GetDmg());
        }

        base.Attack();   //need
    }
}
