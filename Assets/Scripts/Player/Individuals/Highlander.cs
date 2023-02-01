using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlander : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float aoeSize = 5f;
    [SerializeField] private float aoeSizeVariation = 1f;

    protected override void Start()
    {
        base.Start();   //need
        SpawnPivot();
    }

    protected override void Update()
    {
        base.Update();  //need
        GetNearestEnemy();
    }

    protected override void Attack()
    {
        StartCoroutine(delayAttack());
        base.Attack();   //need
    }

    private IEnumerator delayAttack()
    {
        for (int i = -1; i < 2; i++)
        {
            float newAoeSize = aoeSize + Random.Range(-aoeSizeVariation, aoeSizeVariation);

            GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
            aoe.transform.position = transform.position;
            aoe.transform.rotation = Quaternion.Euler(0, 0, GetPivot().rotation.eulerAngles.z + i * 30f);
            aoe.GetComponent<AoE>().SetVariables(GetDmg(), GetHeroColor(), newAoeSize);
            aoe.SetActive(true);

            yield return new WaitForSeconds(.25f);
        }
    }
}
