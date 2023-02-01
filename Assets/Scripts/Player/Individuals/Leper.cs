using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leper : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float aoeSize = 5f;

    protected override void Start()
    {
        base.Start();   //need
    }

    protected override void Update()
    {
        base.Update();  //need
        GetNearestEnemy();
    }

    protected override void Attack()
    {
        GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
        aoe.transform.position = transform.position;
        aoe.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 90f));
        aoe.GetComponent<AoE>().SetVariables(GetDmg(), GetHeroColor(), aoeSize);
        aoe.SetActive(true);

        base.Attack();   //need
    }
}
