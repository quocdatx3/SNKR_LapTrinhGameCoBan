using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBoss : Enemy
{
    [Header("Boss Specific")]
    [SerializeField] private float SpeedIncrease = 250f;
    [SerializeField] private float Duration = 2f;
    [SerializeField] private bool IsDurationStackable = true;
    [SerializeField] private int BuffNumber = 4;
    protected override void Update()
    {
        base.Update();      //need
    }

    protected override void Attack()
    {
        if (EnemiesManager.enemies.Count > 0)
        {
            for (int i = BuffNumber; i > 0; i--)
            {
                GameObject enemy = EnemiesManager.enemies[Random.Range(0, EnemiesManager.enemies.Count)];
                if (!enemy.GetComponent<TimedSpeedBuff>())
                {
                    TimedSpeedBuff buff = enemy.AddComponent<TimedSpeedBuff>();
                    buff.SetBuff(IsDurationStackable, Duration, SpeedIncrease);
                }
                enemy.GetComponent<TimedSpeedBuff>().Activate();
            }
        }
    }

    protected override void OnDestroy()
    {
        if (EnemiesManager.enemies.Count > 0)
        {
            foreach (GameObject enemy in EnemiesManager.enemies)
            {
                if (!enemy.GetComponent<TimedSpeedBuff>())
                {
                    TimedSpeedBuff buff = enemy.AddComponent<TimedSpeedBuff>();
                    buff.SetBuff(IsDurationStackable, Duration, SpeedIncrease);
                }
                enemy.GetComponent<TimedSpeedBuff>().Activate();
            }
        }
    }
}