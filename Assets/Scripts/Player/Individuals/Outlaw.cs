using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outlaw : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float angleBtwBullet = 10f;

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
        for (int i = -2; i < 3; i++)
        {
            GameObject bullet = ObjectsPooling.instance.FromAtkPool("bullet");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, GetPivot().rotation.eulerAngles.z + i * angleBtwBullet);
            bullet.GetComponent<Bullet>().SetVariables(GetDmg(), GetHeroColor());
            bullet.SetActive(true);
        }

        base.Attack();   //need
    }
}
