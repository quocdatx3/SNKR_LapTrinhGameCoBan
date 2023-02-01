using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Hero
{
    [Header("Hero Specific")]
    [SerializeField] private float aoeSize = 1.5f;

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
        GameObject bullet = ObjectsPooling.instance.FromAtkPool("bullet");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = GetPivot().rotation;
        bullet.GetComponent<Bullet>().SetVariables(GetDmg(), GetHeroColor(), true, "aoe_on_contact", aoeSize);
        bullet.SetActive(true);

        base.Attack();   //need
    }

    /* Functions Availables */
    /*
     * Start() {
     *      SpawnPivot();           // sinh ra 1 trục, cần để hero này ngắm bắn
     * }
     * 
     * Update() {
     *      GetNearestEnemy();      // chọn 1 enemy, gần với hero này nhất
     *      GetRandomEnemy();       // chọn 1 enemy, bất kỳ
     *      GetLowestHPHero();      // chọn 1 hero, có thấp máu nhất
     * }
     * 
     * Attack() {
     *      GetAllHero();           // chọn tất cả hero trong đội
     * }
     * 
     * 
     * universal
     *      GetHp();                // trả lại máu hiện tại của hero
     *      GetDmg();               // trả lại dmg của hero
     *      GetCurrentTargets();    // trả lại 1 list<GameObject> chứa mục tiêu của hero
     *      GetPivot();             // trả lại Transform của trục dùng để ngắm bắn 
     *      TakeDamage(float dmg);  // hero mất máu = dmg
     */
}
