using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBoss : Enemy
{
    [Header("Boss Specific")]
    [SerializeField] private float aoeSize = 1.5f;

    protected override void Update()
    {
        base.Update();      //need
    }
    protected override void Attack()
    {
        GameObject bullet = ObjectsPooling.instance.FromAtkPool("bullet");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);

        Bullet _bullet = bullet.GetComponent<Bullet>();
        _bullet.SetVariables(GetDmg(), Color.blue , true, "aoe_on_contact", aoeSize);
        _bullet.SetIsEnemy();

        bullet.SetActive(true);
    }

    protected override void OnDestroy()
    {
        Quaternion postRotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        
        for(int i = -2; i <= 3; i++){
            GameObject bullet = ObjectsPooling.instance.FromAtkPool("bullet");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = postRotation * Quaternion.Euler(0, 0, i * 60);

            Bullet _bullet = bullet.GetComponent<Bullet>();
            _bullet.SetVariables(GetDmg(), GetColor(), true, "aoe_on_contact", aoeSize);
            _bullet.SetIsEnemy();

            bullet.SetActive(true);
        }
    }
}
