using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arcanist : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float summonSpd = 2f;
    [SerializeField] private float summonAtkCD = 2f;

    protected override void Start()
    {
        base.Start();   //need
        SpawnPivot();   //if need to aim
    }
    protected override void Update()
    {
        base.Update();   //need
        GetNearestEnemy();
    }

    protected override void Attack()
    {
        GameObject summon = ObjectsPooling.instance.FromAtkPool("summon");
        summon.transform.position = transform.position;
        summon.transform.rotation = GetPivot().rotation;
        summon.GetComponent<Summon>().SetVariables(GetDmg(), GetHeroColor(), summonSpd, summonAtkCD, "fire_bullet");
        summon.SetActive(true);

        base.Attack();   //need
    }
}
