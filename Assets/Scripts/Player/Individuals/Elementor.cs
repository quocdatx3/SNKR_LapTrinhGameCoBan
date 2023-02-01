using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elementor : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float aoeSize = 15f;

    protected override void Start()
    {
        base.Start();   //need
        SpawnPivot();
    }

    protected override void Update()
    {
        base.Update();  //need
        GetRandomEnemy();
    }

    protected override void Attack()
    {
        List<GameObject> targets = GetCurrentTargets();
        if (targets[0] != null)
        {
            GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
            aoe.transform.position = targets[0].transform.position;
            aoe.transform.rotation = GetPivot().rotation;
            aoe.GetComponent<AoE>().SetVariables(GetDmg(), GetHeroColor(), aoeSize);
            aoe.SetActive(true);

            base.Attack();   //need
        }
    }
}
