using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float aoeSize = 4f;

    protected override void Start()
    {
        base.Start();   //need
    }

    protected override void Update()
    {
        base.Update();  //need
        GetRandomEnemy();
    }

    protected override void Attack()
    {
        GameObject bomb = ObjectsPooling.instance.FromAtkPool("bomb");
        bomb.transform.position = transform.position;
        bomb.GetComponent<Bomb>().SetVariables(GetDmg(), GetHeroColor(), aoeSize);
        bomb.SetActive(true);        

        base.Attack();   //need
    }
}
